using System.Text.Json.Serialization;

namespace SAMPQuery
{
    /// <summary>
    /// Server Information
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// IP and port of the server. 
        /// </summary>
        [JsonPropertyName("ipAddr")]
        public string IPAddressWithPort { get; set; } = string.Empty;
        /// <summary>
        /// Hostname of the server
        /// </summary>
        [JsonPropertyName("name")]
        public string HostName { get; set; }

        /// <summary>
        /// Gamemode of the server
        /// </summary>
        [JsonPropertyName("gameMode")]
        public string GameMode { get; set; }

        /// <summary>
        /// Server language 
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; }

        /// <summary>
        /// Number of players online
        /// </summary>
        [JsonPropertyName("playersOnline")]
        public ushort Players { get; set; }

        /// <summary>
        /// Maximum number of players 
        /// </summary>
        [JsonPropertyName("maxPlayers")]
        public ushort MaxPlayers { get; set; }

        /// <summary>
        /// Password availability
        /// </summary>
        [JsonPropertyName("requiresPassword")]
        public bool Password { get; set; }

        /// <summary>
        /// Ping of server
        /// </summary>
        public int ServerPing { get; set; }
    }
}