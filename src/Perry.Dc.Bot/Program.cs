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
        ConsoleCancelEventHandler cancelKeyPressHandler = (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationSource.Cancel();
        };
        EventHandler processExitHandler = (_, _) => cancellationSource.Cancel();
        Action<System.Runtime.Loader.AssemblyLoadContext> unloadingHandler = _ => cancellationSource.Cancel();

        Console.CancelKeyPress += cancelKeyPressHandler;
        AppDomain.CurrentDomain.ProcessExit += processExitHandler;
        System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += unloadingHandler;

        await using var bot = new DiscordBotApplication(token);
        await bot.RunAsync(cancellationSource.Token);

        return 0;
    }
}
