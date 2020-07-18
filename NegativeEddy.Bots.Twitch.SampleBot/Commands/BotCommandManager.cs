using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class BotCommandManager
    {
        private readonly IDictionary<string, IBotCommand> _commands = new Dictionary<string, IBotCommand>();

        public IReadOnlyDictionary<string, IBotCommand> Commands =>
            new ReadOnlyDictionary<string, IBotCommand>(_commands);

        public void Add(IBotCommand command)
        {
            _commands.Add(command.Command, command);
        }

        public void Add(IEnumerable<IBotCommand> commands)
        {
            foreach (var command in commands)
            {
                _commands.Add(command.Command, command);
            }
        }

        public void Remove(IBotCommand command)
        {
            _commands.Remove(command.Command);
        }
    }
}
