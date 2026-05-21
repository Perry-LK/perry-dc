# perry-dc

Discord bot scaffold for Perry built with .NET and C#.

## Requirements

- .NET 8 SDK
- A Discord bot token exposed as `DISCORD_TOKEN`

## Run locally

```bash
dotnet build perry-dc.sln
DISCORD_TOKEN=your-token-here dotnet run --project src/Perry.Dc.Bot/Perry.Dc.Bot.csproj
```

The bot connects with the minimal `Guilds` gateway intent and stays online until you stop it with `Ctrl+C`.
