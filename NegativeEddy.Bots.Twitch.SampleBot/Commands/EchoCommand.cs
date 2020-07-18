using Microsoft.Bot.Builder;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class EchoCommand : IBotCommand
    {
        public string Command { get; set; } = "echo";

        public string Description { get; set; } = "Echo's the input back to the user";

        public async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            await context.SendActivityAsync(string.Join(' ', args));
        }
    }
}
