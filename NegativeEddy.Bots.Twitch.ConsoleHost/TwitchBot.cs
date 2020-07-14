using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace NegativeEddy.Bots.Twitch.Sample
{
    public class TwitchBot : IBot
    {
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            // Handle Message activity type, which is the main activity type within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message && !string.IsNullOrEmpty(turnContext.Activity.Text))
            {
                string message = turnContext.Activity.Text;
                if (message[0] != '!')
                {
                    // only respond to ! commands
                    return;
                }

                string cmd = message.Substring(1);

                switch (cmd)
                {
                    case "hello":
                        await turnContext.SendActivityAsync($"Hi {turnContext.Activity.From.Name}!", cancellationToken: cancellationToken);
                        break;
                    case "quit":
                        await turnContext.SendActivityAsync($"Bye {turnContext.Activity.From.Name}!", cancellationToken: cancellationToken);
                        break;
                    default:
                        await turnContext.SendActivityAsync($"unknown command '{cmd}'", cancellationToken: cancellationToken);
                        break;

                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }
    }
}
