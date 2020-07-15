using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public interface IBotCommand
    {
        string Command { get; }
        string Description { get; }
        Task ExecuteAsync(ITurnContext context, IList<string> args);
    }
}
