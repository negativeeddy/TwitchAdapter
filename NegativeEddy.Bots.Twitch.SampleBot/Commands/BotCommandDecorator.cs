using AdaptiveExpressions;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public abstract class BotCommandDecorator : IBotCommand
    {
        public IBotCommand Command { get; }
        protected BotCommandDecorator(IBotCommand command)
        {
            Command = command;
        }

        public string Name => Command.Name;
        public string Description => Command.Description;
        public string DecoratorName { get; set; }
        public IEnumerable<string> Decorators()
        {
            IEnumerable<string> result = new[] { DecoratorName };
            if (Command is BotCommandDecorator decorator)
            {
                result = result.Concat(decorator.Decorators());
            }
            return result;
        }
        public abstract Task ExecuteAsync(ITurnContext context, IList<string> args);
    }
}
