using Microsoft.Bot.Builder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    /// <summary>
    /// An example BotCommandDecorator which sends a message before and after executing
    /// the decorated command
    /// </summary>
    public class CoolDownOption : BotCommandDecorator
    {
        public CoolDownOption(IBotCommand command) : base(command)
        {
        }
        public TimeSpan Cooldown { get; set; } = TimeSpan.FromSeconds(10);

        public string? CooldownMessage { get; set; }

        private readonly ConcurrentDictionary<string, DateTime> _coolDownExpirations = new ConcurrentDictionary<string, DateTime>();
        public override async Task ExecuteAsync(ITurnContext context, IList<string> args)
        {
            bool canExecute = CanExecute(context);
            if (canExecute)
            {
                string currentUser = context.Activity.From.Name;
                var time = DateTime.Now + Cooldown;
                Debug.WriteLine($"adding entry for {currentUser} - {time}");
                _coolDownExpirations.AddOrUpdate(currentUser, time, (u, d) => time);
                await Instance.ExecuteAsync(context, args);
            }
            else if (!string.IsNullOrWhiteSpace(CooldownMessage))
            {
                await context.SendActivityAsync(CooldownMessage);
            }

            CleanupOldEntries();
        }

        private bool CanExecute(ITurnContext context)
        {
            string currentUser = context.Activity.From.Name;
            return UserCanExecute(currentUser);
        }

        private bool UserCanExecute(string user)
        {
            if (_coolDownExpirations.TryGetValue(user, out DateTime expirationTime))
            {
                var now = DateTime.Now;
                return now > expirationTime;
            }
            else
            {
                return true;
            }
        }

        private void CleanupOldEntries()
        {
            foreach(var user in _coolDownExpirations.Keys)
            {
                if (UserCanExecute(user))
                {
                    _coolDownExpirations.TryRemove(user, out _);
                }
            }
        }
    }
}
