using Microsoft.Bot.Builder;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class EchoCommand : IBotCommand
    {
        public string Name => "echo";

        public string Description { get; set; } = "Echos the input back to the user";

        public async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            await context.SendActivityAsync(string.Join(' ', args));
        }
    }
}
