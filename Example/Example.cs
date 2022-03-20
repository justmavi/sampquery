using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SAMPQuery;

namespace SampQueryExample
{
    class Tests
    {
        static async Task Main(string[] args)
        {
            string host = "localhost";
            ushort port = 7777;

            var api = new SampQuery(host, port);

            Console.WriteLine("Information" + Environment.NewLine);
            SampServerInfoData serverInfo = await api.GetServerInfoAsync(); // Similar call for synchronus (without await and "Async" postfix)
            PropertyInfo[] sI_properties = serverInfo.GetType().GetProperties();
            foreach (PropertyInfo property in sI_properties)
            {
                Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(serverInfo)}");
            }

            SampServerRulesData sampServerRulesData = await api.GetServerRulesAsync(); // Similar call for synchronus (without await and "Async" postfix)

            Console.WriteLine(Environment.NewLine + "Rules" + Environment.NewLine);
            PropertyInfo[] sR_properties = sampServerRulesData.GetType().GetProperties();
            foreach (PropertyInfo property in sR_properties)
            {
                Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(sampServerRulesData)}");
            }

            Console.WriteLine(Environment.NewLine + "Players" + Environment.NewLine);

            IEnumerable<SampServerPlayerData> serverPlayersInfo = await api.GetServerPlayersAsync(); // Similar call for synchronus (without await and "Async" postfix)
            Console.WriteLine("ID | Name | Score | Ping\n");

            foreach (SampServerPlayerData player in serverPlayersInfo)
            {
                Console.WriteLine($"{player.PlayerId} | {player.PlayerName} | {player.PlayerScore} | {player.PlayerPing}");
            }
            
            // RCON
            var server = new SampQuery("localhost", 7777, "hehe");

            string response = await server.SendRconCommandAsync("echo hi"); // Similar call for synchronus (without await and "Async" postfix)
            Console.WriteLine(response);
        }
    }
}
