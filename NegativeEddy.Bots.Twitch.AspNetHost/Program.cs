using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NegativeEddy.Bots.Twitch.AspNetCore;
using System;

namespace NegativeEddy.Bots.Twitch.AspNetHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddUserSecrets<Program>()
            .Build();

            var twitchSettings = new TwitchAdapterSettings();
            config.GetSection("twitchBot").Bind(twitchSettings);

            return Host.CreateDefaultBuilder(args)
                       .ConfigureServices((hostContext, services) =>
                       {
                           services.AddTransient<IBot, TwitchBot>()
                           .AddTwitchBotAdapter(twitchSettings);
                       });
        }
    }
}
