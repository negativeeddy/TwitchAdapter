using NegativeEddy.Bots.Twitch.SampleBot.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NegativeEddy.Bots.Twitch.BlazorHost.Model
{
    public class CommandTypeManager
    {
        public readonly ReadOnlyDictionary<string, Type> DefaultCommands =
         new ReadOnlyDictionary<string, Type>(new Dictionary<string, Type>
         {
                {"echo", typeof(EchoCommand) },
                {"join", typeof(JoinCommand) },
                {"leave", typeof(LeaveCommand) },
                {"text tesponse", typeof(TextResponseCommand) },
                {"lg response", typeof(LGResponseCommand) },
         });

        public readonly ReadOnlyDictionary<string, Type> DefaultOptions =
         new ReadOnlyDictionary<string, Type>(new Dictionary<string, Type>
         {
                        {"cooldown", typeof(CoolDownDecorator) },
                        {"before and after", typeof(BeforeAndAfterCommandDecorator) },
         });

        public IBotCommand Create(string name, string[]? options = null)
        {
            Type t = DefaultCommands[name] ?? throw new ArgumentException(nameof(name));
            return (IBotCommand)t.Assembly.CreateInstance(t.FullName);
        }
    }
}
