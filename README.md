# Simple class for sending requests to the SA-MP server
Well, this is the same class published by zeelorenc in GitHub 7 years ago, which was also in the wiki samp. After doing a couple of tests with this class, we found critical errors that caused the data not only to be unreadable, but also could lead to a crash (*nonsense, try.. catch would not be allowed*). The code has also been partially rewritten, encoding problems have been fixed, and classes have been added for more convenient use of the received data. I'll tell you more below.

First, declaring and initializing an object

```csharp
var sampQuery = new SampQuery("127.0.0.1", 7777);
```
The constructor also has overloads

```csharp
SampQuery(IPAddress ip, ushort port)
SampQuery(string ip, ushort port, string password)
SampQuery(IPAddress ip, ushort port, string password)
```

There are four methods in the **SampQuery** class, but for now I'll tell you about three:

1. `GetServerInfo` — request basic information about the server

   - **Returns** the `SampServerInfoData` object, which contains all information about the server
   
   - **Usage**
   
   ```csharp
   var sampQuery = new SampQuery("127.0.0.1", 7777);
   SampServerInfoData data = sampQuery.GetServerInfo();

   Console.WriteLine($"Server {data.HostName}. Online: {data.Players}/{data.MaxPlayers}");
   // Server SA-MP Server. Online: 0/50
   ```
   
2. `GetRulesInfo` — request the rules, set by the server

   - **Returns** the `SampServerRulesData` object, which contains all information about the rules of the server.
   
   - **Usage**
   
   ```csharp
    var sampQuery = new SampQuery("127.0.0.1", 7777);
    SampServerInfoData data = sampQuery.GetServerData();

    Console.WriteLine($"Lagcomp {(data.Lagcomp ? "On" : "Off")}. Map: {data.MapName}. SAMPCAC: {data.SAMPCAC_Version ?? "Isn't required"}");
    // Lagcomp On. Map: San-Andreas. SAMPCAC: Isn't required
   ```
   **The maximum value of the player ID is 255. Two-byte identifiers are not supported here (SA-MP limit).**
   
3. `GetPlayersInfo` — request players online with detailed information (works up to 100 online, SA-MP limit)

   - **Returns** a list of the `SampServerPlayerData` objects, which contains all information about players.
   
   - **Usage**
   
   ```csharp
    var sampQuery = new SampQuery("127.0.0.1", 7777);
    List<SampServerPlayerData> datas = sampQuery.GetServerPlayers();
    
    Console.WriteLine("ID | Nick | Score | Ping\n");
    
    foreach(SampServerPlayerData data in datas)
    {
        Console.WriteLine($"{player.PlayerId} | {player.PlayerName} | {player.PlayerScore} | {player.PlayerPing}");
    }
    //ID | Nick | Score | Ping
    //0 | Player1 | 100 | 86
    //...
   ```
Added the **SampServerInfoData, SampServerRulesData, and SampServerPlayerData** classes for easy management of received information:
   
1. **SampServerInfoData**
  - HostName
  - GameMode
  - Language
  - Players
  - MaxPlayers
  - Password
  - ServerPing
  
2. **SampServerRulesData**
  - Lagcomp
  - MapName
  - Version
  - SAMPCAC_Version
  - Weather
  - Weburl
  - WorldTime
  - Gravity
  
3. **SampServerPlayerData**
  - PlayerId
  - PlayerName
  - PlayerScore
  - PlayerPing
  
Now, about the fourth method:

1. `SendRconCommand(command)` — list of lines with the server response

   - **Returns** a list of the `SampServerPlayerData` objects, which contains all information about players.
   
   - **Usage**
   
   ```csharp
    var sampQuery = new SampQuery("127.0.0.1", 7777, "changeme"); // changeme is the password from RCON
    
    foreach(string answer in sampQuery.SendRconCommand("varlist"))
    {
        Console.WriteLine(answer);
    }
   ```

**Gratitude**

  - Separate gratitude to **@continue96** for fixing bugs 
  
Official theme: https://pawn-wiki.ru/index.php?/topic/51733-klass-dlja-otpravki-zaprosov-na-servera-sa-mp/
