using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public abstract class BotCommandDecorator : IBotCommand
    {
        protected IBotCommand Instance { get; }
        protected BotCommandDecorator(IBotCommand command)
        {
            Instance = command;
        }

        public string Command { get => Instance.Command; set => Instance.Command = value; }
        public string Description { get => Instance.Description; set => Instance.Description = value; }
        public abstract Task ExecuteAsync(ITurnContext context, IList<string> args);
    }
}
