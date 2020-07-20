using System;
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

        public void Add(string id, IBotCommand command)
        {
            _commands.Add(id, command);
        }

        public void Add(IEnumerable<(string id, IBotCommand cmd)> commands)
        {
            foreach (var (id, cmd) in commands)
            {
                _commands.Add(id, cmd);
            }
        }

        public void Remove(string id)
        {
            _commands.Remove(id);
        }
    }
}
