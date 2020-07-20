using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
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
        public abstract Task ExecuteAsync(ITurnContext context, IList<string> args);
    }
}
