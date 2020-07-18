using Microsoft.Bot.Builder;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class EchoCommand : IBotCommand
    {
        public EchoCommand(string command = "echo")
        {
            Command = command;
        }

        public string Command { get; private set; }

        public string Description => "Echo's the input back to the user";

        public async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            await context.SendActivityAsync(string.Join(' ', args));
        }
    }
}
