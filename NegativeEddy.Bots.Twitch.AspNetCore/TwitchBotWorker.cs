using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.AspNetCore
{
    public class TwitchBotWorker : BackgroundService
    {
        private readonly ILogger<TwitchBotWorker> _logger;

        public TwitchAdapter? Adapter { get; private set; }

        public TwitchBotWorker(IServiceProvider services)
        {
            Services = services;
            _logger = services.GetService<ILogger<TwitchBotWorker>>();
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("starting");

            Adapter = Services.GetRequiredService<TwitchAdapter>();

            Adapter.OnTurnError += (ctx, e) =>
            {
                // TODO: expose this to the end client somehow
                _logger.LogError(e, e.Message);
                return Task.CompletedTask;
            };

            Adapter.Connect();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("exiting");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("stopping");
            await Task.CompletedTask;
        }
    }
}
