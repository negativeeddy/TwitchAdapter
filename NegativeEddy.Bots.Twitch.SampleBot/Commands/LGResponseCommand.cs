using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.LanguageGeneration;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class LGResponseCommand : IBotCommand
    {
        public string Template { get; set; } =
            @"# response
- line 1
- line 2
= line 3";
        public LGResponseCommand(string command, string template)
        {
            Command = command;
            Template = template;
        }

        public string Command { get; private set; }

        public string Description => "Responds to the user based on a language generation template";

        public async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            var t = Templates.ParseText(Template, "response");
            object response = t.Evaluate("response");
            await context.SendActivityAsync(response.ToString());
        }
    }
}
