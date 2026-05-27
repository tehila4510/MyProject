using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Interfaces;

public class HeartResetWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public HeartResetWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            int hoursToWait = 2 - (now.Hour % 2);
            var nextRunTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(hoursToWait);

            var delay = nextRunTime - now;
            if (delay <= TimeSpan.Zero) delay = TimeSpan.FromMinutes(1); 

            await Task.Delay(delay, stoppingToken);

            using (var scope = _scopeFactory.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                await userService.ResetAllHeartsAsync();
            }
        }
    }
}