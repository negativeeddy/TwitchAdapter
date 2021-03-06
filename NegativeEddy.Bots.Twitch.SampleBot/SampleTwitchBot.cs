﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using NegativeEddy.Bots.Twitch.SampleBot.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch
{
    public class SampleTwitchBot : ActivityHandler
    {
        private readonly BotState _userState;
        private readonly IReadOnlyDictionary<string, IBotCommand> _commands;

        public SampleTwitchBot(UserState userState, BotCommandManager cmdMgr)
        {
            _userState = userState;
            _commands =  cmdMgr.Commands;
        }

        protected override async Task OnMembersRemovedAsync(IList<ChannelAccount> membersRemoved, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersRemoved)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //await turnContext.SendActivityAsync($"See ya later @{member.Name}!", cancellationToken: cancellationToken);
                }
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //await turnContext.SendActivityAsync($"Hi there - @{member.Name}. Welcome to the channel", cancellationToken: cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync("I have arrived!");
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeUserStateAccessor = _userState.CreateProperty<WelcomeUserState>(nameof(WelcomeUserState));
            var didBotWelcomeUser = await welcomeUserStateAccessor.GetAsync(turnContext, () => new WelcomeUserState());
            var userName = turnContext.Activity.From.Name;

            if (didBotWelcomeUser.DidBotWelcomeUser == false)
            {
                didBotWelcomeUser.DidBotWelcomeUser = true;
                await turnContext.SendActivityAsync($"Hi @{userName}, welcome to the channel!", cancellationToken: cancellationToken);
            }
            else
            {
                var text = turnContext.Activity.Text.ToLowerInvariant();
                switch (text)
                {
                    case "hello":
                    case "hi":
                        await turnContext.SendActivityAsync($"Hi again, @{userName}!", cancellationToken: cancellationToken);
                        break;
                    default:
                        // do nothing
                        break;
                }
            }

            // Save any state changes.
            await _userState.SaveChangesAsync(turnContext);
        }

        protected override async Task OnEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext.Activity.AsEventActivity();

            switch (activity.Name)
            {
                case TwitchEvents.Command:
                    await OnCommand(turnContext);
                    break;
                case TwitchEvents.ModeratorJoined:
                    await OnModeratorJoinedArgs(turnContext);
                    break;
            }
        }

        private async Task OnModeratorJoinedArgs(ITurnContext<IEventActivity> turnContext)
        {
            await turnContext.SendActivityAsync($"Everyone be cool. Mod {turnContext.Activity.Value} just arrived.");
        }

        private async Task OnCommand(ITurnContext<IEventActivity> turnContext)
        {
            (string command, List<string> args) = ((string, List<string>))turnContext.Activity.Value;
            if (_commands.TryGetValue(command, out IBotCommand botCommand))
            {
                await botCommand.ExecuteAsync(turnContext, args);
            }
        }
    }
}
