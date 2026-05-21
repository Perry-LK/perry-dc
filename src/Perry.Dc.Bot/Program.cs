namespace Perry.Dc.Bot;

internal static class Program
{
    public static async Task<int> Main()
    {
        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.Error.WriteLine("Set the DISCORD_TOKEN environment variable before running the bot.");
            return 1;
        }

        using var cancellationSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationSource.Cancel();
        };

        await using var bot = new DiscordBotApplication(token);
        await bot.RunAsync(cancellationSource.Token);

        return 0;
    }
}
