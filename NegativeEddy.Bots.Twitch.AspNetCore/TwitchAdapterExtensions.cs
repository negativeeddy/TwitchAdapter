using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace NegativeEddy.Bots.Twitch.AspNetCore
{
    public static class TwitchAdapterExtensions
    {
        public static IServiceCollection AddTwitchBotAdapter(this IServiceCollection services, TwitchAdapterSettings settings)
        {
            services.AddSingleton<TwitchAdapter>(sp => new TwitchAdapter(sp, settings))
                    .AddHostedService<TwitchBotWorker>();
            return services;
        }
    }
}
