using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SampQueryApi;

namespace SampQueryExample
{
    class Tests
    {
        static async Task Main(string[] args)
        {
            string host = "glow-dm.ru";
            ushort port = 7777;

            var api = new SampQuery(host, port);

            Console.WriteLine("Information" + Environment.NewLine);
            SampServerInfoData serverInfo = await api.GetServerInfoAsync();
            PropertyInfo[] sI_properties = serverInfo.GetType().GetProperties();
            foreach (PropertyInfo property in sI_properties)
            {
                Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(serverInfo)}");
            }

            SampServerRulesData sampServerRulesData = await api.GetServerRulesAsync();

            Console.WriteLine(Environment.NewLine + "Rules" + Environment.NewLine);
            PropertyInfo[] sR_properties = sampServerRulesData.GetType().GetProperties();
            foreach (PropertyInfo property in sR_properties)
            {
                Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(sampServerRulesData)}");
            }

            Console.WriteLine(Environment.NewLine + "Players" + Environment.NewLine);

            IEnumerable<SampServerPlayerData> serverPlayersInfo = await api.GetServerPlayersAsync();
            Console.WriteLine("ID | Name | Score | Ping\n");

            foreach (SampServerPlayerData player in serverPlayersInfo)
            {
                Console.WriteLine($"{player.PlayerId} | {player.PlayerName} | {player.PlayerScore} | {player.PlayerPing}");
            }

            var lApi = new SampQuery("localhost", 7777, "hehe");

            string response = lApi.SendRconCommand("echo hi");
            Console.WriteLine(response);
        }
    }
}
