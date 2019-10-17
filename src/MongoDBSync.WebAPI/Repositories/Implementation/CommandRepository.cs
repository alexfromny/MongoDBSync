using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBSync.WebAPI.Domain;

namespace MongoDBSync.WebAPI.Repositories.Implementation
{
    public class CommandRepository : ICommandRepository
    {
        private IMongoCollection<Command> _commandCollection;
        private IMongoDatabase _db;

        public CommandRepository(MongoClientSettings dbSettings, String dbName)
        {
            var client = new MongoClient(dbSettings);
            _db = client.GetDatabase(dbName);
        }

        private IMongoCollection<Command> CommandCollection =>
            _commandCollection ?? (_commandCollection = _db.GetCollection<Command>(DBCollections.CommandsLog.ToString()));

        public async Task<List<Command>> GetAll()
        {
            return await CommandCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<Command>> GetAllWhereIn(IEnumerable<String> guids)
        {
            return await CommandCollection.Find(f=> guids.Contains(f.Guid)).ToListAsync();
        }

        public async Task<List<Command>> GetAllNotSynced()
        {
            return await CommandCollection
                .Find(com => com.IsSynced == BsonBoolean.False)
                .ToListAsync();
        }

        public async Task Save(Command command)
        {
            await CommandCollection.InsertOneAsync(command);
        }

        public async Task SaveManyAsync(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                var com = CommandCollection.Find(f => f.Guid == command.Guid).FirstOrDefault();
                if (com != null)
                    continue;

                await CommandCollection.InsertOneAsync(command);
            }
        }

        public void UpdateStatusByGuid(Command command)
        {
            var filter = Builders<Command>.Filter.Where(x => x.Guid == command.Guid);
            var update = Builders<Command>.Update.Set(x => x.IsSynced, command.IsSynced);

            CommandCollection.UpdateOne(filter, update);
        }

        public Command RunMongoCommand(Command command)
        {
            var com = new JsonCommand<BsonDocument>(command.JCommand);
            var result = _db.RunCommand(com);

            if (result["ok"].ToString() != "1")
                return command;

            command.IsSynced = BsonBoolean.True;
            return command;
        }
    }
}
