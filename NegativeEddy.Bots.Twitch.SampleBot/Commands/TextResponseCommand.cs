using Microsoft.Bot.Builder;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class TextResponseCommand : IBotCommand
    {
        private readonly string _response;

        public TextResponseCommand(string command, string description, string response)
        {
            Command = command;
            Description = description;
            _response = response;
        }

        public string Command { get; }

        public string Description { get; }

        public async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            await context.SendActivityAsync(_response);
        }
    }
}
