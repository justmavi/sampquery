using SAMPQuery;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SampQueryExample
{
    class Tests
    {
        private static async Task Main(string[] args)
        {
            await MainExample();
            await SAMPServerBrowserExample();
        }

        private static async Task MainExample()
        {
            string host = "one.monser.ru";
            ushort port = 7777;

            SampQuery.TimeoutMilliseconds = 500;
            var api = new SampQuery(host, port);

            Console.WriteLine("Information" + Environment.NewLine);
            ServerInfo serverInfo = await api.GetServerInfoAsync(); // Similar call for synchronus (without await and "Async" postfix)
            if (serverInfo != null)
            {
                PropertyInfo[] sI_properties = serverInfo.GetType().GetProperties();
                foreach (PropertyInfo property in sI_properties)
                {
                    Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(serverInfo)}");
                }
            }
            else
            {
                Console.WriteLine("Failed to retrive information of provided server due internet issues or else.");
            }

            ServerRules sampServerRulesData = await api.GetServerRulesAsync(); // Similar call for synchronus (without await and "Async" postfix)
            Console.WriteLine(Environment.NewLine + "Rules" + Environment.NewLine);
            if (sampServerRulesData != null)
            {
                PropertyInfo[] sR_properties = sampServerRulesData.GetType().GetProperties();
                foreach (PropertyInfo property in sR_properties)
                {
                    Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(sampServerRulesData)}");
                }
            }
            else
            {
                Console.WriteLine("Failed to retrive rules of provided server due internet issues or else.");
            }

            Console.WriteLine(Environment.NewLine + "Players" + Environment.NewLine);
            IEnumerable<ServerPlayer> serverPlayersInfo = await api.GetServerPlayersAsync(); // Similar call for synchronus (without await and "Async" postfix)
            if (serverPlayersInfo != null)
            {
                Console.WriteLine("ID | Name | Score | Ping\n");
                foreach (ServerPlayer player in serverPlayersInfo)
                {
                    Console.WriteLine($"{player.PlayerId} | {player.PlayerName} | {player.PlayerScore} | {player.PlayerPing}");
                }
            }
            else
            {
                Console.WriteLine("Failed to retrive players of provided server due internet issues or else.");
            }
            
            Thread.Sleep(3000);

            var server = new SampQuery("localhost", 7777, "hehe");

            string response = await server.SendRconCommandAsync("echo hi"); // Similar call for synchronus (without await and "Async" postfix)
            Console.WriteLine(response);

            Thread.Sleep(3000);
        }

        private static async Task SAMPServerBrowserExample()
        {
            bool infintyLoop = true;
            while (infintyLoop)
            {
                Console.WriteLine("SAMPServerBrowserExample, please press 'y' or 'n' if you want to get list with Ping or not.\nPing can take a little while to process.");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.Clear();
                if (key.Key == ConsoleKey.Y)
                {
                    SampQuery.TimeoutMilliseconds = 500;
                    var serverList = await SampQuery.GetServersAsync();
                    if (serverList == null)
                    {
                        Console.WriteLine("Failed to retrive server list.");
                        continue;
                    }
                    foreach (var server in serverList)
                    {
                        try
                        {
                            SampQuery checkIPOfServer = new(server.IPAddressWithPort);
                            var serverInfoAsync = await checkIPOfServer.GetServerInfoAsync();
                            if (serverInfoAsync == null)
                            {
                                Console.WriteLine($"Is Locked: {server.Password} | Hostname: {server.HostName} | Players: {server.Players} / {server.MaxPlayers} | Ping: timeout | Mode: {server.GameMode} | Language: {server.Language}");
                                continue;
                            }
                            server.ServerPing = serverInfoAsync.ServerPing;
                            Console.WriteLine($"Is Locked: {server.Password} | Hostname: {server.HostName} | Players: {server.Players} / {server.MaxPlayers} | Ping: {server.ServerPing} | Mode: {server.GameMode} | Language: {server.Language}");
                        }
                        catch
                        {
                            Console.WriteLine($"Is Locked: {server.Password} | Hostname: {server.HostName} | Players: {server.Players} / {server.MaxPlayers} | Ping: timeout | Mode: {server.GameMode} | Language: {server.Language}");
                        }
                    }
                }
                else if (key.Key == ConsoleKey.N)
                {
                    SampQuery.TimeoutMilliseconds = 5000;
                    var serverList = await SampQuery.GetServersAsync();
                    if (serverList == null)
                    {
                        Console.WriteLine("Failed to retrive server list.");
                        continue;
                    }
                    foreach (var server in serverList)
                    {
                        Console.WriteLine($"Is Locked: {server.Password} | Hostname: {server.HostName} | Players: {server.Players} / {server.MaxPlayers} | Mode: {server.GameMode} | Language: {server.Language}");
                    }
                }
                else
                {
                    Console.WriteLine($"Try pressing 'y' or 'n' to get ping or not.");
                }
            }
        }
    }
}