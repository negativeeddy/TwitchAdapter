using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class BotCommandManager
    {
        public IReadOnlyDictionary<string, IBotCommand> Commands =>
            new ReadOnlyDictionary<string, IBotCommand>(_commands);

        private IDictionary<string, IBotCommand> _commands { get; } = new Dictionary<string, IBotCommand>();

        public void Add(IBotCommand command)
        {
            _commands.Add(command.Command, command);
        }

        public void Remove(IBotCommand command)
        {
            _commands.Remove(command.Command);
        }
    }
}
