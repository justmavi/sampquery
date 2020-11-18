/*
 * Well I didn't like the 'simplicity' in the original SA-MP query for C#.
 * This is intended for those seeking a nice and simple way to query a server.
 * 
 * Anyways, I was bored. Have fun coding! :D
 * 
 * Coded by Lorenc (zeelorenc)
 * Updated by Mavi (pawn-wiki.ru) & $continue$ 
*/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace SampQueryApi
{
    sealed class SampQuery
    {
        private const char serverInfoPacketType      =    'i';
        private const char serverRulesPacketType     =    'r';
        private const char serverPlayersPacketType   =    'd';
        private const int szReceiveArraySize = 2048;
        private const int timeoutMilliseconds = 5000;

        private readonly string szIP;
        private readonly ushort iPort;
        private readonly IPAddress serverIp;
        private readonly IPEndPoint serverEndPoint;
        private readonly string password;
        private readonly char[] socketHeader;

        private Socket connect2Server;
        private DateTime ReceiveMS;
        private DateTime TransmitMS;

        public SampQuery(in string ip, in ushort port)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // If you get an error at here, please install "System.Text.Encodings.CodePages" package from NuGet

            ReceiveMS = new DateTime();

            serverIp = new IPAddress(IPAddress.Parse(ip).GetAddressBytes());
            serverEndPoint = new IPEndPoint(serverIp, port);

            szIP = ip;
            iPort = port;

            socketHeader = "SAMP".ToCharArray();
        }
        public SampQuery(in IPAddress ip, in ushort port) : this(ip.ToString(), port) { }
        public SampQuery(in string ip, in ushort port, in string password) : this(ip, port)
        {
            this.password = password;
        }
        public SampQuery(in IPAddress ip, in ushort port, in string password) : this(ip.ToString(), port, password)

        private byte[] SendSocketToServer(in char packetType)
        {
            connect2Server = new Socket(serverEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                SendTimeout = timeoutMilliseconds,
                ReceiveTimeout = timeoutMilliseconds
            };

            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            
            string[] szSplitIP = szIP.Split('.');

            writer.Write(socketHeader);

            for (sbyte i = 0; i < szSplitIP.Length; i++)
            {
                writer.Write(Convert.ToByte(Convert.ToInt16(szSplitIP[i])));
            }

            writer.Write(iPort);
            writer.Write(packetType);

            TransmitMS = DateTime.Now; // To get ping (ms to reach back & forth to the svr)


            _ = connect2Server.SendTo(stream.ToArray(), serverEndPoint);


            EndPoint rawPoint = serverEndPoint;
            var szReceive = new byte[szReceiveArraySize];

            _ = connect2Server.ReceiveFrom(szReceive, ref rawPoint);

            connect2Server.Close();
            return szReceive;
        }
        private List<string> ReceiveRconAnswer()
        {
            EndPoint endpoint = new IPEndPoint(serverIp, iPort);

            byte[] rBuffer = new byte[500];
            List<string> results = new List<string>();

            _ = connect2Server.ReceiveFrom(rBuffer, ref endpoint);

            using MemoryStream stream = new MemoryStream(rBuffer);
            using BinaryReader reader = new BinaryReader(stream, Encoding.GetEncoding(1251));
            reader.ReadBytes(11);
            short len;

            while ((len = reader.ReadInt16()) != 0)
                results.Add(new string(reader.ReadChars(len)));

            return results;
        }
        public List<string> SendRconCommand(string cmd)
        {
            connect2Server = new Socket(serverEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                SendTimeout = timeoutMilliseconds,
                ReceiveTimeout = timeoutMilliseconds
            };

            var endpoint = new IPEndPoint(serverIp, iPort);

            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);
            
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

            _ = connect2Server.SendTo(stream.ToArray(), endpoint);
            
            return ReceiveRconAnswer();
        }


        public List<SampServerPlayerData> GetServerPlayers()
        {
            byte[] szReceive = SendSocketToServer(serverPlayersPacketType);

            List<SampServerPlayerData> datas = new List<SampServerPlayerData>();

            using var stream = new MemoryStream(szReceive);
            using BinaryReader read = new BinaryReader(stream);
            _ = read.ReadBytes(10);
            _ = read.ReadChar();

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

            return datas;
        }
        public SampServerInfoData GetServerInfo()
        {
            byte[] szReceive = SendSocketToServer(serverInfoPacketType);

            ReceiveMS = DateTime.Now;
            using var stream = new MemoryStream(szReceive);
            using BinaryReader read = new BinaryReader(stream, Encoding.GetEncoding(1251));

            _ = read.ReadBytes(10);
            _ = read.ReadChar();

            return new SampServerInfoData
            {
                Password = Convert.ToBoolean(read.ReadByte()),
                Players = read.ReadUInt16(),
                MaxPlayers = read.ReadUInt16(),

                HostName = new string(read.ReadChars(read.ReadInt32())),
                GameMode = new string(read.ReadChars(read.ReadInt32())),
                Language = new string(read.ReadChars(read.ReadInt32())),

                ServerPing = ReceiveMS.Subtract(TransmitMS).Milliseconds,
            };
        }
        public SampServerRulesData GetServerRules()
        {
            byte[] szReceive = SendSocketToServer(serverRulesPacketType);
            var sampServerRulesData = new SampServerRulesData();

            using var stream = new MemoryStream(szReceive);
            using BinaryReader read = new BinaryReader(stream, Encoding.GetEncoding(1251));
            
            _ = read.ReadBytes(10);
            _ = read.ReadChar();

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


    class SampServerInfoData
    {
        /// <summary>
        /// Hostname
        /// </summary>
        public string HostName { get; set; } 

        /// <summary>
        /// Gamemode text
        /// </summary>
        public string GameMode { get; set; }

        /// <summary>
        /// Server language 
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Number of players online
        /// </summary>
        public ushort Players { get; set; }

        /// <summary>
        /// Maximum number of players 
        /// </summary>
        public ushort MaxPlayers { get; set; }

        /// <summary>
        /// Password availability
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Ping of server
        /// </summary>
        public int ServerPing { get; set; }
    }

    class SampServerRulesData
    {
        /// <summary>
        /// Lagcomp
        /// </summary>
        public bool Lagcomp { get; set; }

        /// <summary>
        /// Mapname
        /// </summary>
        public string MapName { get; set; }

        /// <summary>
        /// Server version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The version of Client Anti-Cheat, for SAMPCAC-enabled servers
        /// </summary>
        public string SAMPCAC_Version { get; set; } = null;

        /// <summary>
        /// ID of weather in server
        /// </summary>
        public sbyte Weather { get; set; }

        /// <summary>
        /// Link to the server's web page
        /// </summary>
        public Uri Weburl { get; set; }

        /// <summary>
        /// Server time
        /// </summary>
        public DateTime WorldTime { get; set; }

        /// <summary>
        /// Gravity. For CR-MP servers. Default value 0.008000
        /// </summary>
        public decimal Gravity { get; set; } = 0.008000M;
    }
    class SampServerPlayerData
    {
        /// <summary>
        /// Player ID. Max value 255 (SA-MP feature (bug))
        /// </summary>
        public byte PlayerId { get; set; }

        /// <summary>
        /// Player Name. 
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Player Score. 
        /// </summary>
        public int PlayerScore { get; set; }

        /// <summary>
        /// Ping of player. 
        /// </summary>
        public int PlayerPing { get; set; }
    }
}
