namespace SAMPQuery 
{
    /// <summary>
    /// Server Packet Types. See https://sampwiki.blast.hk/wiki/Query_Mechanism#Recieving_the_packets
    /// </summary>
    public static class ServerPacketTypes 
    {
        /// <summary>
        /// Information Packet
        /// </summary>
        /// <value>i</value>
        public static char Info { get => 'i'; }
        /// <summary>
        /// Rule packet
        /// </summary>
        /// <value>r</value>
        public static char Rules { get => 'r'; } 
        /// <summary>
        /// Players packet
        /// </summary>
        /// <value>d</value>    
        public static char Players { get => 'd'; }
        /// <summary>
        /// RCON packet
        /// </summary>
        /// <value>x</value>
        public static char Rcon { get => 'x'; }
    }
}
