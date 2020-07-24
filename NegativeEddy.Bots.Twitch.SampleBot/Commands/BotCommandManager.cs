using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.SampleBot.Commands
{
    public class BotCommandManager
    {
        private IDictionary<string, IBotCommand> _commands = new Dictionary<string, IBotCommand>();

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

        public async Task Save(Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true);
            string json = JsonConvert.SerializeObject(_commands, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            await writer.WriteAsync(json);
            await writer.FlushAsync();
        }

        public async Task Load(Stream stream)
        {
            using var streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
            string json = await streamReader.ReadToEndAsync();
            var loadedCommands = JsonConvert.DeserializeObject<Dictionary<string, IBotCommand>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            _commands = loadedCommands ?? new Dictionary<string, IBotCommand>();
        }
    }
}
