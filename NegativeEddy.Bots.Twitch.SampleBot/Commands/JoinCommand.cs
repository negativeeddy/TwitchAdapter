using Microsoft.Bot.Builder;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class JoinCommand : IBotCommand
    {
        public string Command { get; set; } = "join";

        public string Description { get; set; } = "makes the bot join your channel";

        public Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            if (context.Activity.Conversation.TenantId != context.Activity.From.Name)
            {
                // only allow leave commands from your own channel
                return Task.CompletedTask;
            }

            TwitchAdapter adapter = (TwitchAdapter)context.Adapter;
            adapter.JoinChannel(context.Activity.From.Name);

            return Task.CompletedTask;
        }
    }
}
