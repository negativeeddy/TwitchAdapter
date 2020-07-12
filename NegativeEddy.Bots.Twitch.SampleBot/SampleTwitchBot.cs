using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch
{
    public class SampleTwitchBot : ActivityHandler
    {
        private BotState _userState;

        public SampleTwitchBot(UserState userState)
        {
            _userState = userState;
        }

        protected override async Task OnMembersRemovedAsync(IList<ChannelAccount> membersRemoved, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersRemoved)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync($"See ya later {member.Name}!", cancellationToken: cancellationToken);
                }
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync($"Hi there - {member.Name}. Welcome to the channel", cancellationToken: cancellationToken);
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
                await turnContext.SendActivityAsync($"Hi {userName}, welcome to the channel!", cancellationToken: cancellationToken);
            }
            else
            {
                var text = turnContext.Activity.Text.ToLowerInvariant();
                switch (text)
                {
                    case "hello":
                    case "hi":
                        await turnContext.SendActivityAsync($"Hi again, {userName}!", cancellationToken: cancellationToken);
                        break;
                    case "!intro":
                    case "!help":
                        await turnContext.SendActivityAsync("Right now I cant help with much.");
                        break;
                    default:
                        // do nothing
                        break;
                }
            }

            // Save any state changes.
            await _userState.SaveChangesAsync(turnContext);
        }
    }
}
