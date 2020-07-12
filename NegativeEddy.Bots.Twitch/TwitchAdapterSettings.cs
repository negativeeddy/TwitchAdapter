using System;
using System.Collections.Generic;
using System.Text;

namespace NegativeEddy.Bots.Twitch
{
    public class TwitchAdapterSettings
    {
        public virtual string? ClientId { get; set; }
        public virtual string? UserId { get; set; }
        public virtual string? OAuthToken { get; set; }
        public int? ThrottlingPeriodInSeconds { get; set; }
        public int? ThrottlingMessagesAllowedInPeriod { get; set; }
        public string[] InitialChannels { get; set; } = new string[0];
    }
}
