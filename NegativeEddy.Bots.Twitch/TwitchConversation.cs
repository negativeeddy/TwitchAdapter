namespace NegativeEddy.Bots.Twitch
{
    public enum TwitchConversation
    {
        Channel,
        Whisper
    }

    // For quick comparisons in Activity properties which are all strings
    public static class TwitchConversationString
    {
        public static string Channel { get; } = TwitchConversation.Channel.ToString();
        public static string Whisper { get; } = TwitchConversation.Whisper.ToString();
    }
}
