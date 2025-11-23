using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SAMPQuery.Utils;

namespace SAMPQuery
{
    /// <summary>
    /// Implements the SAMPQuery interface.
    /// </summary>
    public class SampQuery
    {
        private readonly ushort serverPort;
        private readonly string serverIpString;
        private readonly IPEndPoint serverEndPoint;
        private readonly string? password;
        private DateTime transmitMs;

        /// <summary>
        /// Initialize SAMPQuery
        /// </summary>
        /// <param name="host">Server hostname or IP address</param>
        /// <param name="port">Server port</param>
        public SampQuery(string host, ushort port)
        {
            var serverIp1 = ResolveHostToIpAddress(host);
            serverEndPoint = new IPEndPoint(serverIp1, port);
            serverIpString = serverIp1.ToString();
            serverPort = port;
        }

        /// <summary>
        /// Initialize SAMPQuery
        /// </summary>
        /// <param name="ip">Server IP address</param>
        /// <param name="port">Server port</param>
        /// <returns>SampQuery instance</returns>
        public SampQuery(IPAddress ip, ushort port) : this(ip.ToString(), port)
        {
        }

        /// <summary>
        /// Initialize SAMPQuery with default 7777 port
        /// </summary>
        /// <param name="ip">Server IP address</param>
        /// <returns>SampQuery instance</returns>
        public SampQuery(IPAddress ip) : this(ip.ToString(), SampQueryConstants.DefaultServerPort)
        {
        }

        /// <summary>
        /// Initialize SAMPQuery with default 7777 port or with port from given string (ip:port)
        /// </summary>
        /// <param name="ip">Server IP address</param>
        /// <returns>SampQuery instance</returns>
        public SampQuery(string ip) : this(ip.Split(':')[0], GetPortFromStringOrDefault(ip))
        {
        }

        /// <summary>
        /// Initialize SAMPQuery
        /// </summary>
        /// <param name="host">Server hostname or IP address</param>
        /// <param name="port">Server port</param>
        /// <param name="password">Server password</param>
        /// <returns>SampQuery instance</returns>
        public SampQuery(string host, ushort port, string password) : this(host, port)
        {
            this.password = password;
        }

        /// <summary>
        /// Initialize SAMPQuery
        /// </summary>
        /// <param name="ip">Server IP address</param>
        /// <param name="port">Server port</param>
        /// <param name="password">Server password</param>
        /// <returns>SampQuery instance</returns>
        public SampQuery(IPAddress ip, ushort port, string password) : this(ip.ToString(), port, password)
        {
        }

        private static ushort GetPortFromStringOrDefault(string ip)
        {
            var parts = ip.Split(':');
            return parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1])
                ? ushort.Parse(parts[1])
                : SampQueryConstants.DefaultServerPort;
        }

        private static IPAddress ResolveHostToIpAddress(string host)
        {
            if (IPAddress.TryParse(host, out var ipAddress))
            {
                return ipAddress;
            }

            return Dns.GetHostEntry(host).AddressList
                .First(a => a.AddressFamily == AddressFamily.InterNetwork);
        }

        private byte[] BuildPackage(char packetType, string? command = null)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(SampQueryConstants.SocketHeader);
            WriteIpAddress(writer, serverIpString);
            writer.Write(serverPort);
            writer.Write(packetType);

            if (packetType == ServerPacketTypes.Rcon && command != null)
            {
                WriteRconCommand(writer, command);
            }

            return stream.ToArray();
        }

        private static void WriteIpAddress(BinaryWriter writer, string ipAddress)
        {
            var ipParts = ipAddress.Split('.');
            foreach (var part in ipParts)
            {
                writer.Write(Convert.ToByte(Convert.ToInt16(part)));
            }
        }

        private void WriteRconCommand(BinaryWriter writer, string command)
        {
            writer.Write((ushort)password.Length);
            writer.Write(password.ToCharArray());
            writer.Write((ushort)command.Length);
            writer.Write(command.ToCharArray());
        }

        private async Task<byte[]> SendSocketToServerAsync(char packetType, string? command = null, CancellationToken ct = default)
        {
            var packet = BuildPackage(packetType, command);
            transmitMs = DateTime.UtcNow;

            using var socket = new Socket(serverEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            await socket.ConnectAsync(serverEndPoint, ct);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            linkedCts.CancelAfter(SampQueryConstants.TimeoutMilliseconds);

            // send
            await socket.SendAsync(packet, SocketFlags.None, linkedCts.Token);

            // receive
            var buf = ArrayPool<byte>.Shared.Rent(SampQueryConstants.ReceiveBufferSize);
            try
            {
                var received = await socket.ReceiveAsync(buf, SocketFlags.None, linkedCts.Token);
                return buf.AsSpan(0, received).ToArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buf);
            }
        }

        private byte[] SendSocketToServer(char packetType, string command = null)
        {
            var packet = BuildPackage(packetType, command);
            transmitMs = DateTime.Now;

            using var socket = new Socket(serverEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            
            socket.SendTimeout = SampQueryConstants.TimeoutMilliseconds;
            socket.ReceiveTimeout = SampQueryConstants.TimeoutMilliseconds;
            
            socket.SendTo(packet, SocketFlags.None, serverEndPoint);

            var receiveBuffer = new byte[SampQueryConstants.ReceiveBufferSize];
            EndPoint endPoint = serverEndPoint;
            socket.ReceiveFrom(receiveBuffer, SocketFlags.None, ref endPoint);

            return receiveBuffer;
        }

        /// <summary>
        /// Execute RCON command
        /// </summary>
        /// <param name="command">Command name. See https://sampwiki.blast.hk/wiki/Controlling_Your_Server#RCON_Commands</param>
        /// <returns>Server response</returns>
        /// <exception cref="System.ArgumentException">Thrown when command or RCON password is an empty string</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when command or RCON password is null</exception>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown when operation timed out</exception>
        /// <exception cref="SAMPQuery.RconPasswordException">Thrown when RCON password is invalid (changeme or incorrect)</exception>
        public string SendRconCommand(string command)
        {
            ValidateRconCommand(command);
            var data = SendSocketToServer(ServerPacketTypes.Rcon, command);
            return ProcessRconResponse(data);
        }

        /// <summary>
        /// Execute RCON command
        /// </summary>
        /// <param name="command">Command name. See https://sampwiki.blast.hk/wiki/Controlling_Your_Server#RCON_Commands</param>
        /// <returns>An asynchronous task that completes with the server response</returns>
        /// <exception cref="System.ArgumentException">Thrown when command or RCON password is an empty string</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when command or RCON password is null</exception>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown when operation timed out</exception>
        /// <exception cref="SAMPQuery.RconPasswordException">Thrown when RCON password is invalid (changeme or incorrect)</exception>
        public async Task<string> SendRconCommandAsync(string command)
        {
            ValidateRconCommand(command);
            var data = await SendSocketToServerAsync(ServerPacketTypes.Rcon, command);
            return ProcessRconResponse(data);
        }

        private void ValidateRconCommand(string command)
        {
            Helpers.CheckNullOrEmpty(command, nameof(command));
            Helpers.CheckNullOrEmpty(password, nameof(password));

            if (password == SampQueryConstants.DefaultRconPassword)
            {
                throw new RconPasswordException(RconPasswordExceptionMessages.CHANGEME_NOT_ALLOWED);
            }
        }

        private string ProcessRconResponse(byte[] data)
        {
            var response = CollectRconAnswerFromByteArray(data);

            return response == SampQueryConstants.InvalidRconPasswordMessage
                ? throw new RconPasswordException(RconPasswordExceptionMessages.INVALD_RCON_PASSWORD)
                : response;
        }

        /// <summary>
        /// Get server players
        /// </summary>
        /// <returns>An asynchronous task that completes with the collection of ServerPlayer instances</returns>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown when operation timed out</exception>
        public async Task<IEnumerable<ServerPlayer>> GetServerPlayersAsync()
        {
            var data = await SendSocketToServerAsync(ServerPacketTypes.Players);
            return CollectServerPlayersInfoFromByteArray(data);
        }

        /// <summary>
        /// Get server players
        /// </summary>
        /// <returns>Collection of ServerPlayer instances</returns>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown when operation timed out</exception>
        public IEnumerable<ServerPlayer> GetServerPlayers()
        {
            var data = SendSocketToServer(ServerPacketTypes.Players);
            return CollectServerPlayersInfoFromByteArray(data);
        }

        /// <summary>
        /// Get information about server
        /// </summary>
        /// <returns>An asynchronous task that completes with an instance of ServerPlayer</returns>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown when operation timed out</exception>
        public async Task<ServerInfo> GetServerInfoAsync()
        {
            var data = await SendSocketToServerAsync(ServerPacketTypes.Info);
            return CollectServerInfoFromByteArray(data);
        }

        /// <summary>
        /// Get information about server
        /// </summary>
        /// <returns>An instance of ServerPlayer</returns>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown when operation timed out</exception>
        public ServerInfo GetServerInfo()
        {
            var data = SendSocketToServer(ServerPacketTypes.Info);
            return CollectServerInfoFromByteArray(data);
        }

        /// <summary>
        /// Get server rules
        /// </summary>
        /// <returns>An asynchronous task that completes with an instance of ServerRules</returns>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown when operation timed out</exception>
        public async Task<ServerRules> GetServerRulesAsync()
        {
            var data = await SendSocketToServerAsync(ServerPacketTypes.Rules);
            return CollectServerRulesFromByteArray(data);
        }

        /// <summary>
        /// Get server rules
        /// </summary>
        /// <returns>An instance of ServerRules</returns>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown when operation timed out</exception>
        public ServerRules GetServerRules()
        {
            var data = SendSocketToServer(ServerPacketTypes.Rules);
            return CollectServerRulesFromByteArray(data);
        }

        private static string CollectRconAnswerFromByteArray(byte[] data)
        {
            var result = new StringBuilder();

            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream, SampQueryConstants.Windows1251Encoding);
            reader.ReadBytes(SampQueryConstants.PackageHeaderSize);

            short length;
            while ((length = reader.ReadInt16()) != 0)
            {
                result.Append(new string(reader.ReadChars(length)));
                result.Append('\n');
            }

            return result.ToString();
        }

        private static IEnumerable<ServerPlayer> CollectServerPlayersInfoFromByteArray(byte[] data)
        {
            var players = new List<ServerPlayer>();

            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);

            SkipPacketHeader(reader);
            int totalPlayers = reader.ReadInt16();

            for (var i = 0; i < totalPlayers; i++)
            {
                players.Add(new ServerPlayer
                {
                    PlayerId = reader.ReadByte(),
                    PlayerName = new string(reader.ReadChars(reader.ReadByte())),
                    PlayerScore = reader.ReadInt32(),
                    PlayerPing = reader.ReadInt32()
                });
            }

            return players;
        }

        private ServerInfo CollectServerInfoFromByteArray(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream, SampQueryConstants.Windows1251Encoding);

            SkipPacketHeader(reader);

            return new ServerInfo
            {
                Password = Convert.ToBoolean(reader.ReadByte()),
                Players = reader.ReadUInt16(),
                MaxPlayers = reader.ReadUInt16(),
                HostName = new string(reader.ReadChars(reader.ReadInt32())),
                GameMode = new string(reader.ReadChars(reader.ReadInt32())),
                Language = new string(reader.ReadChars(reader.ReadInt32())),
                ServerPing = (int)(DateTime.Now - transmitMs).TotalMilliseconds
            };
        }

        private static ServerRules CollectServerRulesFromByteArray(byte[] data)
        {
            var serverRules = new ServerRules();

            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream, SampQueryConstants.Windows1251Encoding);

            SkipPacketHeader(reader);
            int rulesCount = reader.ReadInt16();

            for (int i = 0; i < rulesCount; i++)
            {
                string propertyName = new string(reader.ReadChars(reader.ReadByte())).Replace(' ', '_');
                string propertyValue = new string(reader.ReadChars(reader.ReadByte()));

                var property = serverRules.GetType().GetProperty(
                    propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    object convertedValue = ConvertPropertyValue(property, propertyValue);
                    property.SetValue(serverRules, convertedValue);
                }
            }

            return serverRules;
        }

        private static object ConvertPropertyValue(PropertyInfo property, string value)
        {
            if (property.PropertyType == typeof(bool))
            {
                return value == "On";
            }

            if (property.PropertyType == typeof(Uri))
            {
                return Helpers.ParseWebUrl(value);
            }

            if (property.PropertyType == typeof(DateTime))
            {
                return Helpers.ParseTime(value);
            }

            return Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture);
        }

        private static void SkipPacketHeader(BinaryReader reader)
        {
            reader.ReadBytes(SampQueryConstants.InfoPackageHeaderSize);
            reader.ReadChar();
        }
    }
}