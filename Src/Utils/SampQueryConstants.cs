using System.Text;

namespace SAMPQuery.Utils
{
    /// <summary>
    /// Constants used by SampQuery class
    /// </summary>
    public static class SampQueryConstants
    {
        /// <summary>
        /// Default SAMP server port (always 7777)
        /// </summary>
        public const ushort DefaultServerPort = 7777;
        
        /// <summary>
        /// Receive Buffer Size
        /// </summary>
        public const int ReceiveBufferSize = 2048;
        
        /// <summary>
        /// Default Timeout milliseconds
        /// </summary>
        public const int TimeoutMilliseconds = 5000;
        
        /// <summary>
        /// Socket Timeout Error Code
        /// </summary>
        public const int SocketTimeoutErrorCode = 10060;
        
        /// <summary>
        /// Package Header Size
        /// </summary>
        public const int PackageHeaderSize = 11;
        
        /// <summary>
        /// Info package header size
        /// </summary>
        public const int InfoPackageHeaderSize = 10;
        
        /// <summary>
        /// Socket header string
        /// </summary>
        public const string SocketHeaderString = "SAMP";
        
        /// <summary>
        /// Invalid RCON password message
        /// </summary>
        public const string InvalidRconPasswordMessage = "Invalid RCON password.\n";
        
        /// <summary>
        /// Default RCON password
        /// </summary>
        public const string DefaultRconPassword = "changeme";

        /// <summary>
        /// Socket Header as charr array
        /// </summary>
        public static readonly char[] SocketHeader = SocketHeaderString.ToCharArray();
        
        /// <summary>
        /// Cyrillic encoding
        /// </summary>
        public static readonly Encoding Windows1251Encoding;

        static SampQueryConstants()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Windows1251Encoding = Encoding.GetEncoding(1251);
        }
    }
}

