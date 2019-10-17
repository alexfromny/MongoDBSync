using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MongoDBSync.WebAPI.Domain;
using MongoDBSync.WebAPI.Helpers;
using MongoDBSync.WebAPI.Repositories;

namespace MongoDBSync.WebAPI.Services
{
    public class MongoDBCommandService : IMongoDBCommandService
    {
        private const Int32 _sleepTimeInMilliseconds = 5000;

        private readonly IMapper _mapper;
        private readonly ICommandRepository _commandRepository;

        public MongoDBCommandService(ICommandRepository commandRepository, IMapper mapper)
        {
            _mapper = mapper;
            _commandRepository = commandRepository;

            CheckAndSaveCommands();
        }

        private void CheckAndSaveCommands()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(_sleepTimeInMilliseconds);
                    try
                    {
                        var commands = MongoDBHelper.GetLatestCommands(_sleepTimeInMilliseconds).ToList();

                        if (!commands.Any()) continue;

                        _commandRepository.SaveManyAsync(commands.Select(_mapper.Map<Command>)).Wait();

                        MongoDBHelper.RemoveCommands(commands);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"CheckAndSaveCommands: {ex.Message}");
                    }
                }
            });
        }
    }
}
