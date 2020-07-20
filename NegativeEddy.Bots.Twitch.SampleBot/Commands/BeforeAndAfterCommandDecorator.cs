using Microsoft.Bot.Builder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    /// <summary>
    /// An example BotCommandDecorator which sends a message before and after executing
    /// the decorated command
    /// </summary>
    public class BeforeAndAfterCommandDecorator : BotCommandDecorator
    {
        public BeforeAndAfterCommandDecorator(IBotCommand command) : base(command) { }

        public override string DecoratorName => "before and after";

        public string BeforeMessage { get; set; }
        public string AfterMessage { get; set; }

        public override async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            await context.SendActivityAsync($"I'm about to do the {Name} command");
            await Command.ExecuteAsync(context, args);
            await context.SendActivityAsync($"I just did the {Name} command");
        }
    }
}
