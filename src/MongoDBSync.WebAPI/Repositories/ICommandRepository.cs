using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDBSync.WebAPI.Domain;

namespace MongoDBSync.WebAPI.Repositories
{
    public interface ICommandRepository
    {
        Task<List<Command>> GetAll();

        Task<List<Command>> GetAllNotSynced();

        Task<List<Command>> GetAllWhereIn(IEnumerable<String> guids);

        Task Save(Command command);

        Task SaveManyAsync(IEnumerable<Command> commands);

        Command RunMongoCommand(Command command);

        void UpdateStatusByGuid(Command command);
    }
}
