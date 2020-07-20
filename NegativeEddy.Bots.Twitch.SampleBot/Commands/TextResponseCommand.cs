using Microsoft.Bot.Builder;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class TextResponseCommand : IBotCommand
    {
        public string Name => "text response";

        public string Description => "replies to the user with a simple text response";

        public string Response { get; set; }

        public async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            await context.SendActivityAsync(Response);
        }
    }
}
