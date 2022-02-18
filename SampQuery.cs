using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Threading.Tasks;

namespace SampQueryApi
{
    public sealed class SampQuery
    {
        private static readonly char serverInfoPacketType = 'i';
        private static readonly char serverRulesPacketType = 'r';
        private static readonly char serverPlayersPacketType = 'd';
        private static readonly int szReceiveArraySize = 2048;
        private static readonly int timeoutMilliseconds = 5000;

        private readonly string szIP;
        private readonly ushort iPort;
        private readonly IPAddress serverIp;
        private readonly IPEndPoint serverEndPoint;
        private readonly string password;
        private readonly char[] socketHeader;

        private Socket connect2Server;
        private DateTime receiveMS;
        private DateTime transmitMS;

        public SampQuery(string host, ushort port)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); 

            receiveMS = new DateTime();

            if (!IPAddress.TryParse(host, out serverIp)) {
                serverIp = Dns.GetHostAddresses(host)[0];
            }

            serverEndPoint = new IPEndPoint(serverIp, port);

            szIP = serverIp.ToString();
            iPort = port;

            socketHeader = "SAMP".ToCharArray();
        }
        public SampQuery(IPAddress ip, ushort port) : this(ip.ToString(), port) { }
        public SampQuery(string host, ushort port, string password) : this(host, port)
        {
            this.password = password;
        }
        public SampQuery(IPAddress ip, ushort port, string password) : this(ip.ToString(), port, password) {}

        private async Task<byte[]> SendSocketToServer(char packetType)
        {
            connect2Server = new Socket(serverEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                SendTimeout = timeoutMilliseconds,
                ReceiveTimeout = timeoutMilliseconds
            };

            using(var stream = new MemoryStream())
            {
                using(var writer = new BinaryWriter(stream))
                {
                    string[] szSplitIP = szIP.Split('.');

                    writer.Write(socketHeader);

                    for (sbyte i = 0; i < szSplitIP.Length; i++)
                    {
                        writer.Write(Convert.ToByte(Convert.ToInt16(szSplitIP[i])));
                    }

                    writer.Write(iPort);
                    writer.Write(packetType);

                    transmitMS = DateTime.Now; 

                    await connect2Server.SendToAsync(stream.ToArray(), SocketFlags.None, serverEndPoint);

                    EndPoint rawPoint = serverEndPoint;
                    var szReceive = new byte[szReceiveArraySize];

                    await connect2Server.ReceiveFromAsync(szReceive, SocketFlags.None, rawPoint);

                    connect2Server.Close();
                    return szReceive;
                }
                
            }
            
        }
        private IEnumerable<string> ReceiveRconAnswer()
        {
            EndPoint endpoint = new IPEndPoint(serverIp, iPort);

            byte[] rBuffer = new byte[500];
            List<string> results = new List<string>();

            connect2Server.ReceiveFrom(rBuffer, ref endpoint);

            using (MemoryStream stream = new MemoryStream(rBuffer))
            {
                using (BinaryReader reader = new BinaryReader(stream, Encoding.GetEncoding(1251)))
                {
                    reader.ReadBytes(11);
                    short len;

                    while ((len = reader.ReadInt16()) != 0)
                        results.Add(new string(reader.ReadChars(len)));

                    return results;
                }
            }
        }
        public IEnumerable<string> SendRconCommand(string cmd)
        {
            connect2Server = new Socket(serverEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                SendTimeout = timeoutMilliseconds,
                ReceiveTimeout = timeoutMilliseconds
            };

            var endpoint = new IPEndPoint(serverIp, iPort);

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(socketHeader);

                    string[] SplitIP = szIP.ToString().Split('.');

                    for(int i = 0; i < SplitIP.Length; i++)     
                        writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[i])));

                    writer.Write(iPort);

                    writer.Write('x');

                    writer.Write((ushort)password.Length);
                    writer.Write(password.ToCharArray());

                    writer.Write((ushort)cmd.Length);
                    writer.Write(cmd.ToCharArray());

                    connect2Server.SendTo(stream.ToArray(), endpoint);
                }

            }
            
            return ReceiveRconAnswer();
        }
        public async Task<List<SampServerPlayerData>> GetServerPlayers()
        {
            byte[] szReceive = await SendSocketToServer(serverPlayersPacketType);

            List<SampServerPlayerData> datas = new List<SampServerPlayerData>();

            using(var stream = new MemoryStream(szReceive))
            {
                using(BinaryReader read = new BinaryReader(stream))
                {
                    read.ReadBytes(10);
                    read.ReadChar();

                    for (int i = 0, iTotalPlayers = read.ReadInt16(); i < iTotalPlayers; i++)
                    {
                        datas.Add(new SampServerPlayerData
                        {
                            PlayerId = Convert.ToByte(read.ReadByte()),
                            PlayerName = new string(read.ReadChars(read.ReadByte())),
                            PlayerScore = read.ReadInt32(),
                            PlayerPing = read.ReadInt32()
                        });
                    }
                }
            }

            return datas;
        }
        public async Task<SampServerInfoData> GetServerInfo()
        {
            byte[] szReceive = await SendSocketToServer(serverInfoPacketType);

            receiveMS = DateTime.Now;
            using (var stream = new MemoryStream(szReceive))
            {
                using (BinaryReader read = new BinaryReader(stream, Encoding.GetEncoding(1251)))
                {
                    read.ReadBytes(10);
                    read.ReadChar();

                    return new SampServerInfoData
                    {
                        Password = Convert.ToBoolean(read.ReadByte()),
                        Players = read.ReadUInt16(),
                        MaxPlayers = read.ReadUInt16(),

                        HostName = new string(read.ReadChars(read.ReadInt32())),
                        GameMode = new string(read.ReadChars(read.ReadInt32())),
                        Language = new string(read.ReadChars(read.ReadInt32())),

                        ServerPing = receiveMS.Subtract(transmitMS).Milliseconds,
                    };
                }
            }
        }
        public async Task<SampServerRulesData> GetServerRules()
        {
            byte[] szReceive = await SendSocketToServer(serverRulesPacketType);
            var sampServerRulesData = new SampServerRulesData();

            using (var stream = new MemoryStream(szReceive))
            {
                using (BinaryReader read = new BinaryReader(stream, Encoding.GetEncoding(1251)))
                {
                    read.ReadBytes(10);
                    read.ReadChar();

                    string value;
                    object val;

                    for (int i = 0, iRules = read.ReadInt16(); i < iRules; i++)
                    {
                        PropertyInfo property = sampServerRulesData.GetType().GetProperty(new string(read.ReadChars(read.ReadByte())).Replace(' ', '_'), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        value = new string(read.ReadChars(read.ReadByte()));

                        if (property != null)
                        {
                            if (property.PropertyType == typeof(bool)) val = value == "On";
                            else if (property.PropertyType == typeof(Uri)) val = new Uri("http://" + value, UriKind.Absolute);
                            else if (property.PropertyType == typeof(DateTime)) val = DateTime.Today.Add(TimeSpan.Parse(value));
                            else val = Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture);

                            property.SetValue(sampServerRulesData, val);
                        }
                    }
                    return sampServerRulesData;
                }
            }
        }
    }
}
