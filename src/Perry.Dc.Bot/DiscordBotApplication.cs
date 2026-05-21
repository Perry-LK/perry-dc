using Discord;
using Discord.WebSocket;

namespace Perry.Dc.Bot;

internal sealed class DiscordBotApplication : IAsyncDisposable
{
    private readonly DiscordSocketClient _client;
    private readonly string _token;
    private readonly TaskCompletionSource _readySource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public DiscordBotApplication(string token)
    {
        _token = token;
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds
        });

        _client.Log += OnLogAsync;
        _client.Ready += OnReadyAsync;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var cancellationRegistration = cancellationToken.Register(() => _readySource.TrySetCanceled(cancellationToken));
        var loggedIn = false;
        var started = false;

        try
        {
            await _client.LoginAsync(TokenType.Bot, _token);
            loggedIn = true;
            await _client.StartAsync();
            started = true;
            await _readySource.Task;
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            if (started)
            {
                await _client.StopAsync();
            }

            if (loggedIn)
            {
                await _client.LogoutAsync();
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        _client.Log -= OnLogAsync;
        _client.Ready -= OnReadyAsync;
        _client.Dispose();
        return ValueTask.CompletedTask;
    }

    private Task OnReadyAsync()
    {
        Console.WriteLine($"Connected as {_client.CurrentUser.Username}");
        _readySource.TrySetResult();
        return Task.CompletedTask;
    }

    private static Task OnLogAsync(LogMessage message)
    {
        var output = message.Severity >= LogSeverity.Warning ? Console.Error : Console.Out;
        output.WriteLine($"[{DateTimeOffset.UtcNow:u}] {message.Severity,-11} {message.Source}: {message.Message}");

        if (message.Exception is not null)
        {
            output.WriteLine(message.Exception);
        }

        return Task.CompletedTask;
    }
}
