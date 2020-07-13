using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace NegativeEddy.Bots.Twitch
{
    public class TwitchAdapter : BotAdapter, IDisposable
    {
        private const int DEFAULT_THROTTLING_PERIOD = 30;
        private const int DEFAULT_THROTTLING_COUNT = 750;

        private TwitchClient _client;
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;
        private readonly string _botId;
        private readonly string[] _initialChannels;
        private readonly char _commandIdentifier = '!';

        public TwitchAdapter(IServiceProvider services, TwitchAdapterSettings settings)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            _logger = (ILogger)loggerFactory.CreateLogger<TwitchAdapter>() ?? NullLogger.Instance;
            _botId = settings?.UserId ?? throw new ArgumentNullException(nameof(TwitchAdapterSettings.UserId));
            
            _logger.LogInformation("initializing twitch adapter");

            string token = settings.OAuthToken ?? throw new ArgumentNullException("OAuthToken");
            ConnectionCredentials credentials = new ConnectionCredentials(_botId, token);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = settings.ThrottlingMessagesAllowedInPeriod ?? DEFAULT_THROTTLING_COUNT,
                ThrottlingPeriod = TimeSpan.FromSeconds(settings.ThrottlingPeriodInSeconds ?? DEFAULT_THROTTLING_PERIOD)
            };

            var customClient = new WebSocketClient(clientOptions);
            _client = new TwitchClient(customClient, ClientProtocol.WebSocket, loggerFactory.CreateLogger<TwitchClient>());
            _client.OnJoinedChannel += async (s, e) => await ProcessBotJoinedChannelAsync(e.BotUsername, e.Channel);
            _client.OnMessageReceived += async (s, e) => await ProcessChatMessageAsync(e.ChatMessage);
            _client.OnChatCommandReceived += async (s, e) => await ProcessChatCommand(e.Command);
            _client.OnWhisperReceived += async (s, e) => await ProcessWhisperAsync(e.WhisperMessage);
            _client.OnWhisperCommandReceived += async (s,e) => await ProcessWhisperCommandAsync(e.Command);
            _client.OnUserJoined += async (s, e) => await ProcessUserJoined(e.Channel, e.Username);
            _client.OnUserLeft += async (s, e) => await ProcessUserLeft(e.Channel, e.Username);
            _client.OnConnected += (s, e) => _logger.LogInformation($"Connected to {e.AutoJoinChannel}");

            _client.Initialize(credentials, chatCommandIdentifier: _commandIdentifier, whisperCommandIdentifier: _commandIdentifier);

            _initialChannels = (string[])settings.InitialChannels.Clone();
        }

        private Activity CreateBaseActivity(string channel, bool isGroup = true, TwitchConversation conversationType = TwitchConversation.Channel)
        {
            return new Activity()
            {
                ChannelId = "Twitch",
                Conversation = new ConversationAccount(id: channel + "|Twitch", isGroup: isGroup, conversationType: conversationType.ToString(), tenantId: channel),
                Timestamp = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                Recipient = new ChannelAccount(id: _botId, name: _botId),
            };
        }

        private async Task ProcessUserLeft(string channel, string username)
        {
            _logger.LogInformation("user left channel {channel}", channel);
            var activity = CreateBaseActivity(channel);
            activity.Type = ActivityTypes.ConversationUpdate;
            activity.MembersRemoved = new[] { new ChannelAccount(id: username, name: username) };

            await ProcessActivityAsync(activity);
        }

        private async Task ProcessUserJoined(string channel, string username)
        {
            _logger.LogInformation("user joined channel {channel}", channel);
            var activity = CreateBaseActivity(channel);
            activity.Type = ActivityTypes.ConversationUpdate;
            activity.MembersAdded = new[] { new ChannelAccount(id: username, name: username) };

            await ProcessActivityAsync(activity);
        }

        private async Task ProcessBotJoinedChannelAsync(string botUsername, string channel)
        {
            Debug.Assert(_botId == botUsername);
            _logger.LogInformation("joined channel {channel}", channel);

            var activity = CreateBaseActivity(channel);
            activity.Type = ActivityTypes.ConversationUpdate;
            activity.MembersAdded = new[] { new ChannelAccount(id: _botId, name: botUsername) };

            await ProcessActivityAsync(activity);
        }

        private async Task ProcessChatMessageAsync(ChatMessage message)
        {
            if (message.Message[0] == _commandIdentifier)
            {
                // this is a command. It will be processed by ProcessChatCommand
                return;
            }

            _logger.LogInformation("received chat message in channel {channel}", message.Channel);
            var activity = CreateBaseActivity(message.Channel);
            activity.Text = message.Message;
            activity.From = new ChannelAccount(id: message.UserId, name: message.Username);
            activity.Type = ActivityTypes.Message;
            activity.ChannelData = System.Text.Json.JsonSerializer.Serialize(message);

            await ProcessActivityAsync(activity);
        }

        private async Task ProcessChatCommand(ChatCommand command)
        {
            var message = command.ChatMessage; 
            _logger.LogInformation("received chat command in channel {channel}", message.Channel);
            var activity = CreateBaseActivity(message.Channel);
            activity.From = new ChannelAccount(id: message.UserId, name: message.Username);
            activity.Type = ActivityTypes.Event;
            activity.Name = command.CommandText;
            activity.Value = command.ArgumentsAsList;
               
            activity.ChannelData = System.Text.Json.JsonSerializer.Serialize(command);

            await ProcessActivityAsync(activity);
        }

        private async Task ProcessWhisperAsync(WhisperMessage whisper)
        {
            if (whisper.Message[0] == _commandIdentifier)
            {
                // this is a command. It will be processed by ProcessChatCommand
                return;
            }

            _logger.LogInformation("received whisper");
            var activity = CreateBaseActivity(channel: whisper.Username, isGroup: false, conversationType: TwitchConversation.Whisper);
            activity.Text = whisper.Message;
            activity.From = new ChannelAccount(id: whisper.UserId, name: whisper.Username);

            // override the default conversation
            activity.Type = ActivityTypes.Message;
            activity.ChannelData = System.Text.Json.JsonSerializer.Serialize(whisper);

            await ProcessActivityAsync(activity);
        }

        private async Task ProcessWhisperCommandAsync(WhisperCommand command)
        {
            var whisper = command.WhisperMessage;
            _logger.LogInformation("received whisper");
            var activity = CreateBaseActivity(channel: whisper.Username, isGroup: false, conversationType: TwitchConversation.Whisper);
            activity.From = new ChannelAccount(id: whisper.UserId, name: whisper.Username);
            activity.Type = ActivityTypes.Event;
            activity.Name = command.CommandText;
            activity.Value = command.ArgumentsAsList;
            activity.ChannelData = System.Text.Json.JsonSerializer.Serialize(command);

            await ProcessActivityAsync(activity);
        }

        public async Task ProcessActivityAsync(Activity activity)
        {
            if (activity == null)
            {
                return;
            }

            var bot = _services.GetRequiredService<IBot>();
            using var context = new TurnContext(this, activity);

            await this.RunPipelineAsync(
                context,
                async (turnContext, cancellationToken) => await bot.OnTurnAsync(turnContext),
                default(CancellationToken)
                );
        }

        // Sends activities to the conversation.
        public override async Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext context, Activity[] activities, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (activities == null)
            {
                throw new ArgumentNullException(nameof(activities));
            }

            if (activities.Length == 0)
            {
                throw new ArgumentException("Expecting one or more activities, but the array was empty.", nameof(activities));
            }

            var responses = new ResourceResponse[activities.Length];

            for (var index = 0; index < activities.Length; index++)
            {
                var activity = activities[index];

                switch (activity.Type)
                {
                    case ActivityTypes.Message:
                        {
                            IMessageActivity message = activity.AsMessageActivity();

                            // A message exchange between user and bot can contain media attachments
                            // (e.g., image, video, audio, file).  In this particular example, we are unable
                            // to create Attachments to messages, but this illustrates processing.
                            string twitchChannel = activity.Conversation.TenantId;

                            if (message.Attachments != null && message.Attachments.Any())
                            {
                                var attachment = message.Attachments.Count == 1 ? "1 attachment" : $"{message.Attachments.Count()} attachments";
                                _client.SendMessage(twitchChannel, $"{message.Text} with {attachment}");
                            }
                            else
                            {
                                if (activity.Conversation.ConversationType == TwitchConversationString.Channel)
                                {
                                    _client.SendMessage(twitchChannel, message.Text);
                                }
                            }
                        }

                        break;

                    case ActivityTypesEx.Delay:
                        {
                            // The Activity Schema doesn't have a delay type build in, so it's simulated
                            // here in the Bot. This matches the behavior in the Node connector.
                            int delayMs = (int)((Activity)activity).Value;
                            await Task.Delay(delayMs).ConfigureAwait(false);
                        }

                        break;

                    case ActivityTypes.Trace:
                        // Do not send trace activities unless you know that the client needs them.
                        // For example: BF protocol only sends Trace Activity when talking to emulator channel.
                        break;

                    default:
                        Console.WriteLine("Bot: activity type: {0}", activity.Type);
                        break;
                }

                responses[index] = new ResourceResponse(activity.Id);
            }

            return responses;
        }

        // Normally, replaces an existing activity in the conversation.
        // Not implemented for this sample.
        public override Task<ResourceResponse> UpdateActivityAsync(ITurnContext turnContext, Activity activity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // Deletes an existing activity in the conversation.
        // Not implemented for this sample.
        public override Task DeleteActivityAsync(ITurnContext turnContext, ConversationReference reference, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            _logger.LogInformation("connecting to Twitch");
            _client.Connect();

            // connect to configured channels
            _logger.LogInformation("connecting to initial channels");
            foreach (var channel in _initialChannels)
            {
                Thread.Sleep(1000); // pause here. TwitchLib sometimes drops channels if joining too fast
                JoinChannel(channel);
            }
        }

        public void JoinChannel(string channel)
        {
            _logger.LogInformation("joining channel {channel}", channel);
            _client.JoinChannel(channel);
        }

        public void LeaveChannel(string channel)
        {
            _logger.LogInformation("leaving channel {channel}", channel);
            _client.LeaveChannel(channel);
        }

        public IReadOnlyList<JoinedChannel> JoinedChannels => _client.JoinedChannels;


        public void Dispose()
        {
            _client?.Disconnect();
        }
    }
}
