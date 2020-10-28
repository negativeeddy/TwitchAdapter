using NegativeEddy.Bots.Twitch.SampleBot.Commands;
using System;

namespace NegativeEddy.Bots.Twitch.BlazorHost.Shared
{
    public record BotCommandReplacedEventArgs(
        IBotCommand oldCmd,
        IBotCommand newCmd);
}
