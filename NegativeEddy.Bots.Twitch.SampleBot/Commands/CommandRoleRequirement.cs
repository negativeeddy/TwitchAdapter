using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    /// <summary>
    /// An example BotCommandDecorator which sends a message before and after executing
    /// the decorated command
    /// </summary>
    public class CommandRoleRequirement : BotCommandDecorator
    {
        public CommandRoleRequirement(IBotCommand command) : base(command)
        {
        }
        public string? Role { get; set; }

        public string? CooldownMessage { get; set; }

        public override string DecoratorName => "role";

        public override async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            var command = context.Activity.ChannelData as ChatCommand;

            if (Role is null)
            {
                // defining Role is required for this decorator
                throw new InvalidOperationException("Role cannot be null");
            }

            var roles = context.Activity.From.Role;
            if (roles is null)
            {
                // no roles, no access
                return;
            }

            if (roles == Role)
            {
                // if the role matches, run the command
                await Command.ExecuteAsync(context, args);
            }
        }
    }
}
