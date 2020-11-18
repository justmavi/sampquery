﻿using System;
using System.Collections.Generic;
using SampQueryApi;
using System.Linq;
using System.Reflection;

namespace samptest
{
    class Tests
    {
        static void Main(string[] args)
        {
            string ip = "164.132.6.130";
            ushort port = 7777;

            var api = new SampQuery(ip, port);

            Console.WriteLine("Information" + Environment.NewLine);
            SampServerInfoData serverInfo = api.GetSampServerInfo();
            PropertyInfo[] sI_properties = serverInfo.GetType().GetProperties();
            foreach (PropertyInfo property in sI_properties)
            {
                Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(serverInfo)}");
            }

            SampServerRulesData sampServerRulesData = api.GetSampServerRules();

            Console.WriteLine(Environment.NewLine + "Rules" + Environment.NewLine);
            PropertyInfo[] sR_properties = sampServerRulesData.GetType().GetProperties();
            foreach (PropertyInfo property in sR_properties)
            {
                Console.WriteLine($"{property.Name}:\t\t\t{property.GetValue(sampServerRulesData)}");
            }

            Console.WriteLine(Environment.NewLine + "Players" + Environment.NewLine);

            List<SampServerPlayerData> serverPlayersInfo = api.GetSampServerPlayersData();
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
