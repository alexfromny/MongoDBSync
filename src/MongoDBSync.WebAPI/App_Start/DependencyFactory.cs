using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDBSync.WebAPI.App_Start.Settings;
using MongoDBSync.WebAPI.Repositories;
using MongoDBSync.WebAPI.Repositories.Implementation;
using MongoDBSync.WebAPI.Services;

namespace MongoDBSync.WebAPI.App_Start
{
    public static class DependencyFactory
    {
        public static IServiceCollection PopulateContainer(IServiceCollection serviceCollection, IAppSettings settings, MongoClientSettings dbSettings)
        {
            InitMapper(serviceCollection);

            serviceCollection.AddSingleton<IAppSettings, AppSettings>();

            serviceCollection.AddSingleton<IUserRepository, UserRepository>(_ => new UserRepository(dbSettings, settings.MongoDBName));
            serviceCollection.AddSingleton<ICommandRepository, CommandRepository>(_ => new CommandRepository(dbSettings, settings.MongoDBName));

            serviceCollection.AddSingleton<IMongoDBCommandService, MongoDBCommandService>(s =>
                        new MongoDBCommandService(s.GetService<ICommandRepository>(), s.GetService<IMapper>()));

            if (settings.ApiRunSyncService)
            {
                serviceCollection.AddSingleton<IHostedService, SyncCommandsService>(s => new SyncCommandsService(settings, s.GetService<ICommandRepository>(), s.GetService<IMapper>()));
                serviceCollection.AddHostedService<SyncCommandsService>();
            }

            return serviceCollection;
        }

        private static void InitMapper(IServiceCollection serviceCollection)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            serviceCollection.AddSingleton(mapper);
        }
    }
}
