using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDBSync.WebAPI.Domain;
using MongoDBSync.WebAPI.Repositories;
using MongoDBSync.WebAPI.ViewModels.Command;

namespace MongoDBSync.WebAPI.Helpers
{
    public class MongoDBHelper
    {
        private static readonly Dictionary<String, String> BaseCommandValueKeyPair = new Dictionary<String, String>()
        {
            {"insert", "documents"},
            {"update", "updates"},
            {"delete", "deletes"},
        };

        private static List<CommandViewModel> NewCommands = new List<CommandViewModel>();

        private static List<Command> CommandsInProgress = new List<Command>();

        public static bool IsChangeCommand(String command, String collection)
        {
            return BaseCommandValueKeyPair.ContainsKey(command) && collection.ToLower() != DBCollections.CommandsLog.ToString().ToLower();
        }

        public static String GetCommandValueKey(String command)
        {
            return BaseCommandValueKeyPair[command];
        }

        public static void RemoveCommands(IEnumerable<CommandViewModel> commands)
        {
            foreach (var command in commands)
            {
                NewCommands.Remove(command);
            }
        }

        public static IEnumerable<CommandViewModel> GetLatestCommands(Int32 sleepTimeInMilliseconds)
        {
            return NewCommands.Where(w => w.Timestamp < DateTime.Now.AddMilliseconds(-sleepTimeInMilliseconds));
        }

        public static IEnumerable<String> RunCommands(ICommandRepository commandRepository, IList<Command> commands)
        {
            if (!commands.Any())
                return new List<String>();

            ProcessCommands(commands);

            var syncedCommands = commands.OrderBy(o => o.Timestamp)
                .Select(commandRepository.RunMongoCommand)
                .ToList();

            syncedCommands.ForEach(command => commandRepository.UpdateStatusByGuid(command));

            ProcessCommands(null);

            return syncedCommands.Where(w => w.IsSynced == BsonBoolean.True).Select(s => s.Guid);
        }

        public static MongoClientSettings GetMongoDBSettings(String connectionString)
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    var el = e.Command.Elements.FirstOrDefault(f => f.Name == e.CommandName);
                    if (!IsChangeCommand(el.Name.ToString(), el.Value.ToString())) return;

                    SaveCommand(new CommandViewModel(
                        $"{e.OperationId}",
                        el.Name.ToString(),
                        el.Value.ToString(),
                        e.Command.Elements.FirstOrDefault(f => f.Name == GetCommandValueKey(el.Name.ToString())).Value.ToJson()
                    ));
                });
            };

            return settings;
        }

        private static void SaveCommand(CommandViewModel command)
        {
            if (CommandsInProgress.Any() && CommandsInProgress.FirstOrDefault(f => f.JCommand == command.JCommand) != null)
                return;

            NewCommands.Add(command);
        }

        private static void ProcessCommands(IEnumerable<Command> localCommands)
        {
            CommandsInProgress.Clear();

            if (localCommands != null)
                CommandsInProgress.AddRange(localCommands);
        }
    }
}
