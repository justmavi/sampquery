# SAMPQuery

SAMPQuery is a library that allows you to query SAMP servers for information about it and execute RCON commands. It includes encoding correction, hostname resolution, asynchronous calls and much more.

## Table of Contents

- [SAMPQuery](#sampquery)
  - [Table of Contents](#table-of-contents)
  - [Usage](#usage)
  - [Documentation](#documentation)
    - [Constructor](#constructor)
    - [GetServerInfo](#getserverinfo)
    - [GetServerInfoAsync](#getserverinfoasync)
    - [GetServerRules](#getserverrules)
    - [GetServerRulesAsync](#getserverrulesasync)
    - [GetServerPlayers](#getserverplayers)
    - [GetServerPlayersAsync](#getserverplayersasync)
    - [SendRconCommand](#sendrconcommand)
    - [SendRconCommandAsync](#sendrconcommandasync)
    - [ServerInfo](#serverinfo)
    - [ServerRules](#serverrules)
    - [ServerPlayer](#serverplayer)
  - [Gratitude](#gratitude)
  - [Stay in touch](#stay-in-touch)

## Usage

```csharp
var server = new SampQuery("localhost", 7777);

ServerInfo serverInfo = await server.GetServerInfoAsync();
ServerRules serverRules = await server.GetServerRulesAsync();
IEnumerable<ServerPlayer> serverPlayers = await server.GetServerPlayersAsync();

Console.WriteLine($"Welcome to ${serverInfo.HostName}! Mapname: ${serverRules.MapName}");
Console.WriteLine("Players online:");
serverPlayers.ToList().ForEach(player => Console.WriteLine(player.PlayerName));
```

## Documentation

### Constructor

```csharp
var server = new SampQuery("127.0.0.1", 7777);
```

The constructor also has overloads

```csharp
SampQuery(IPAddress ip, ushort port)
SampQuery(string ip, ushort port, string password)
SampQuery(IPAddress ip, ushort port, string password)
```

Hostname is also allowed

```csharp
var server = new SampQuery("localhost", 7777);
```

### GetServerInfo

Requests basic information about the server

```csharp
var server = new SampQuery("127.0.0.1", 7777);
ServerInfo data =  server.GetServerInfo();

Console.WriteLine($"Server {data.HostName}. Online: {data.Players}/{data.MaxPlayers}");
```

### GetServerInfoAsync

Asynchronously requests basic information about the server

```csharp
var server = new SampQuery("127.0.0.1", 7777);
ServerInfo data = await server.GetServerInfoAsync();

Console.WriteLine($"Server {data.HostName}. Online: {data.Players}/{data.MaxPlayers}");
```

### GetServerRules

Requests the rules, set by the server

```csharp
var server = new SampQuery("127.0.0.1", 7777);
ServerInfo data = server.GetServerRules();

Console.WriteLine($"Lagcomp {(data.Lagcomp ? "On" : "Off")}. Map: {data.MapName}. SAMPCAC: {data.SAMPCAC_Version ?? "Isn't required"}");
```

### GetServerRulesAsync

Asynchronously requests the rules, set by the server

```csharp
var server = new SampQuery("127.0.0.1", 7777);
ServerInfo data = await server.GetServerRulesAsync();

Console.WriteLine($"Lagcomp {(data.Lagcomp ? "On" : "Off")}. Map: {data.MapName}. SAMPCAC: {data.SAMPCAC_Version ?? "Isn't required"}");
```

### GetServerPlayers

Requests players online with detailed information (works up to 100 online, SA-MP limit)

```csharp
 var server = new SampQuery("127.0.0.1", 7777);
 IEnumerable<ServerPlayer> players = server.GetServerPlayers();

 Console.WriteLine("ID | Nick | Score | Ping\n");

 foreach(ServerPlayer player in players)
 {
     Console.WriteLine($"{player.PlayerId} | {player.PlayerName} | {player.PlayerScore} | {player.PlayerPing}");
 }
```

**The maximum value of the player ID is 255. Two-byte identifiers are not supported here (SA-MP limit).**

### GetServerPlayersAsync

Asynchronously requests players online with detailed information (works up to 100 online, SA-MP limit)

```csharp
 var server = new SampQuery("127.0.0.1", 7777);
 IEnumerable<ServerPlayer> players = await server.GetServerPlayersAsync();

 Console.WriteLine("ID | Nick | Score | Ping\n");

 foreach(ServerPlayer player in players)
 {
     Console.WriteLine($"{player.PlayerId} | {player.PlayerName} | {player.PlayerScore} | {player.PlayerPing}");
 }
```

**The maximum value of the player ID is 255. Two-byte identifiers are not supported here (SA-MP limit).**

### SendRconCommand

Executes RCON command

```csharp
 var server = new SampQuery("127.0.0.1", 7777, "helloworld");
 string answer = sampQuery.SendRconCommand("varlist");

 Console.WriteLine($"Server says: {answer}");
```

### SendRconCommandAsync

Asynchronously executes RCON command

```csharp
 var server = new SampQuery("127.0.0.1", 7777, "helloworld");
 string answer = sampQuery.SendRconCommand("varlist");

 Console.WriteLine($"Server says: {answer}");
```

### ServerInfo

A class representing information about the server. Properties:

- HostName
- GameMode
- Language
- Players
- MaxPlayers
- Password
- ServerPing

### ServerRules

A class representing server rules. Properties:

- Lagcomp
- MapName
- Version
- SAMPCAC_Version
- Weather
- Weburl
- WorldTime
- Gravity

### ServerPlayer

A class representing information about the player. Properties:

- PlayerId
- PlayerName
- PlayerScore
- PlayerPing

## Gratitude

- Separate gratitude to **@continue98** for fixing bugs

## Stay in touch

- Author - [Grish Poghosyan](https://www.linkedin.com/in/grishpoghosyan)
