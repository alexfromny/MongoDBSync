using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using MongoDBSync.WebAPI.App_Start;
using MongoDBSync.WebAPI.App_Start.Settings;
using MongoDBSync.WebAPI.Domain;
using MongoDBSync.WebAPI.Helpers;
using MongoDBSync.WebAPI.Repositories;
using MongoDBSync.WebAPI.ViewModels.Command;

namespace MongoDBSync.WebAPI.Services
{
    public class SyncCommandsService : IHostedService
    {
        private readonly String _suncBaseUrl = String.Empty;

        private readonly IMapper _mapper;
        private readonly IAppSettings _appSettings;
        private readonly ICommandRepository _commandRepository;

        private CancellationTokenSource _tokenSource;

        public SyncCommandsService(IAppSettings appSettings, ICommandRepository commandRepository, IMapper mapper)
        {
            _mapper = mapper;
            _appSettings = appSettings;
            _commandRepository = commandRepository;

            _suncBaseUrl = $"{_appSettings.ApiCloudUrl}/{Endpoints.Command.Base}/";
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await SyncCommandsWithServer();

                    Thread.Sleep(_appSettings.ApiSyncPeriod);
                }
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
        }

        private async Task SyncCommandsWithServer()
        {
            await SyncCommandLogs();

            var notSyncedCommands = await _commandRepository.GetAllNotSynced();
            if (notSyncedCommands.Any())
            {
                var response = HttpClientHelper.Post<IEnumerable<String>>($"{_suncBaseUrl}{Endpoints.Command.Execute}", null);
                if (response != null && response.Any())
                {
                    MongoDBHelper.RunCommands(_commandRepository, notSyncedCommands.Where(w => response.Contains(w.Guid)).ToList());
                }
            }

            Console.WriteLine($"{DateTime.Now.ToString("s")} commands synced.");
        }

        private async Task SyncCommandLogs()
        {
            try
            {
                var localCommands = await _commandRepository.GetAllNotSynced();

                var cloudCommands = HttpClientHelper.Get<List<CommandViewModel>>($"{_suncBaseUrl}{Endpoints.Command.Sync}");
                if (cloudCommands.Any())
                    await _commandRepository.SaveManyAsync(cloudCommands.Select(_mapper.Map<Command>));

                if(localCommands.Any())
                    HttpClientHelper.Post($"{_suncBaseUrl}{Endpoints.Command.Sync}", localCommands.Select(_mapper.Map<CommandViewModel>));
            }
            catch (Exception e)
            {
            }
        }
    }
}
