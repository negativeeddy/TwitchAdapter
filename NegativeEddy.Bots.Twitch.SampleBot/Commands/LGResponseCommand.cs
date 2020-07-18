using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.LanguageGeneration;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    /// <summary>
    /// A Bot command that uses Language Generation to create a response.
    /// 
    /// When invoked, the command will evaluate the template with the id "response".
    /// 
    /// e.g. a simple template with three options would look like
    /// 
    /// # response
    /// - option 1
    /// - option 2
    /// - option 3
    /// </summary>
    public class LGResponseCommand : IBotCommand
    {
        public const string ResponseTemplateId = "response";

        public string Template { get; set; } 

        public string Command { get; set; }

        public string Description { get; set; } = string.Empty;

        public async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            var t = Templates.ParseText(Template);
            object response = t.Evaluate(ResponseTemplateId);
            await context.SendActivityAsync(response.ToString());
        }
    }
}
