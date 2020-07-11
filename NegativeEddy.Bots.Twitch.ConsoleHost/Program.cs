using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NegativeEddy.Bots.Twitch;
using NegativeEddy.Bots.Twitch.Sample;
using System;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IServiceProvider serviceProvider = Initialize();

            // Create the Console Adapter, and add Conversation State
            // to the Bot. The Conversation State will be stored in memory.
            using var adapter = serviceProvider.GetService<TwitchAdapter>();
            adapter.OnTurnError += (ctx, e) =>
            {
                Console.WriteLine(e);
                return Task.CompletedTask;
            };
            adapter.Connect();

            string commandLine;
            do
            {
                commandLine = Console.ReadLine();
                var cmd = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                switch(cmd[0])
                {
                    case "join":
                        // join channel
                        adapter.JoinChannel(cmd[1]);
                        Console.WriteLine("joined");
                        break;
                    case "leave":
                        // leave channel
                        adapter.LeaveChannel(cmd[1]);
                        Console.WriteLine("left");
                        break;
                    case "channels":
                        foreach(var c in adapter.JoinedChannels)
                        {
                            Console.WriteLine(c);
                        }
                        break;
                    case "q":
                    default:
                        break;
                }
            }
            while (commandLine != "q");
        }

        static private IServiceProvider Initialize()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets<Program>()
                .Build();

            var twitchSettings = new TwitchAdapterSettings();
            config.GetSection("twitchBot").Bind(twitchSettings);

            var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(p => config)
            .AddLogging(loggin =>
            {
                loggin.SetMinimumLevel(LogLevel.Trace);
                loggin.AddConsole();
                loggin.AddDebug();
            })
            .AddScoped<IBot, TwitchBot>()
            .AddScoped<TwitchAdapter>(sp => new TwitchAdapter(sp, twitchSettings));

            services.AddHttpClient("ShoutoutCommand", c =>
            {
                c.BaseAddress = new Uri("https://api.twitch.tv/kraken/channels/");
                c.DefaultRequestHeaders.Add("client-id", config["twitchBot:ClientId"]);
            });

            services.Configure<TwitchAdapterSettings>(config.GetSection("twitchBot"));

            return services.BuildServiceProvider();
        }

    }
}
