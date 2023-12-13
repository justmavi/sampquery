using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SAMPQuery;

namespace SampQueryExample
{
    class Tests
    {
        static async Task Main(string[] args)
        {
            SampQuery.TimeoutMilliseconds = 1000; // -> If you want the change amount of time of waiting for ping. 

            string host = "one.monser.ru";
            ushort port = 7777;

            var api = new SampQuery(host, port);

            try
            {
                Console.WriteLine("Information" + Environment.NewLine);
                ServerInfo serverInfo = await api.GetServerInfoAsync(); // Similar call for synchronus (without await and "Async" postfix)
                PropertyInfo[] sI_properties = serverInfo.GetType().GetProperties();
                foreach (PropertyInfo property in sI_properties)
                {
                    Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(serverInfo)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Could not Get Server info due server issues or internet issues.");
                Console.WriteLine("Exception message: " + ex.Message);
            }

            try
            {
                ServerRules sampServerRulesData = await api.GetServerRulesAsync(); // Similar call for synchronus (without await and "Async" postfix)

                Console.WriteLine(Environment.NewLine + "Rules" + Environment.NewLine);
                PropertyInfo[] sR_properties = sampServerRulesData.GetType().GetProperties();
                foreach (PropertyInfo property in sR_properties)
                {
                    Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(sampServerRulesData)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Could not Get Server Rules due server issues or internet issues.");
                Console.WriteLine("Exception message: " + ex.Message);
            }

            try
            {
                Console.WriteLine(Environment.NewLine + "Players" + Environment.NewLine);

                IEnumerable<ServerPlayer> serverPlayersInfo = await api.GetServerPlayersAsync(); // Similar call for synchronus (without await and "Async" postfix)
                Console.WriteLine("ID | Name | Score | Ping\n");

                foreach (ServerPlayer player in serverPlayersInfo)
                {
                    Console.WriteLine($"{player.PlayerId} | {player.PlayerName} | {player.PlayerScore} | {player.PlayerPing}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Could not Get Players info due server issues or internet issues.");
                Console.WriteLine("Exception message: " + ex.Message);
            }
            Thread.Sleep(3000);
            // RCON
            try
            {
                var server = new SampQuery("localhost", 7777, "hehe");
                string response = await server.SendRconCommandAsync("echo hi"); // Similar call for synchronus (without await and "Async" postfix)
                Console.WriteLine(response);
            }
            catch(Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Could not Get RCON info due server issues or internet issues.");
                Console.WriteLine("Exception message: " + ex.Message);
            }
        }
    }
}
