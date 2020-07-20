using NegativeEddy.Bots.Twitch.SampleBot.Commands;

namespace NegativeEddy.Bots.Twitch.BlazorHost.Shared
{
    public class BotCommandReplacedEventArgs
    {
        public IBotCommand oldCmd { get; set; }
        public IBotCommand newCmd { get; set; }
    }
}
