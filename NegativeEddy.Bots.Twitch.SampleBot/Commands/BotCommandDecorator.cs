using AdaptiveExpressions;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public abstract class BotCommandDecorator : IBotCommand
    {
        public IBotCommand Command { get; private set; }
        protected BotCommandDecorator(IBotCommand command)
        {
            Command = command;
        }

        [JsonIgnore]
        public string Name => Command.Name;
        [JsonIgnore]
        public string Description => Command.Description;
        [JsonIgnore]
        public abstract string DecoratorName { get; }
        public IEnumerable<BotCommandDecorator> Decorators()
        {
            if (Command is BotCommandDecorator decorator)
            {
                return decorator.Decorators().Append(this);
            }
            else
            {
                return new[] { this };
            }
        }

        /// <summary>
        /// Removes a decorator from the decorator chain
        /// </summary>
        /// <param name="decorator">a refrence to the decorator to remove</param>
        public void Remove(BotCommandDecorator decorator)
        {
            // just reparent the next decorator down the chain
            if (Command == decorator)
            {
                Command = decorator.Command;
            }
            else if (Command is BotCommandDecorator childDecorator)
            {
                // pass the responsibility on down the chain
                childDecorator.Remove(decorator);
            }
        }

        public abstract Task ExecuteAsync(ITurnContext context, IList<string> args);
    }
}
