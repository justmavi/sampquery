﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SampQueryApi
{
    class Tests
    {
        static async Task Main(string[] args)
        {
            string host = "glow-dm.ru";
            ushort port = 6666;

            var api = new SampQuery(host, port);

            Console.WriteLine("Information" + Environment.NewLine);
            SampServerInfoData serverInfo = await api.GetServerInfo();
            PropertyInfo[] sI_properties = serverInfo.GetType().GetProperties();
            foreach (PropertyInfo property in sI_properties)
            {
                Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(serverInfo)}");
            }

            SampServerRulesData sampServerRulesData = await api.GetServerRules();

            Console.WriteLine(Environment.NewLine + "Rules" + Environment.NewLine);
            PropertyInfo[] sR_properties = sampServerRulesData.GetType().GetProperties();
            foreach (PropertyInfo property in sR_properties)
            {
                Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(sampServerRulesData)}");
            }

            Console.WriteLine(Environment.NewLine + "Players" + Environment.NewLine);

            List<SampServerPlayerData> serverPlayersInfo = await api.GetServerPlayers();
            Console.WriteLine("ID | Name | Score | Ping\n");

            foreach (SampServerPlayerData player in serverPlayersInfo)
            {
                Console.WriteLine($"{player.PlayerId} | {player.PlayerName} | {player.PlayerScore} | {player.PlayerPing}");
            }

            string lIp = "127.0.0.1";
            ushort lPort = 7777;

            var lApi = new SampQuery(lIp, lPort, "changeme");

            foreach (var items in lApi.SendRconCommand("banip 255.255.255.255"))
            {
                Console.WriteLine(items);
            }
        }
    }
}