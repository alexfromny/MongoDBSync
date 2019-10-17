using System;
using Microsoft.Extensions.Configuration;

namespace MongoDBSync.WebAPI.App_Start.Settings
{
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _configuration;

        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Int32 ApiSyncPeriod => Int32.Parse(_configuration.GetSection($"{nameof(WebApiConstants.Api)}:{nameof(WebApiConstants.Api.SyncPeriod)}").Value);
        public Boolean ApiRunSyncService => Boolean.Parse(_configuration.GetSection($"{nameof(WebApiConstants.Api)}:{nameof(WebApiConstants.Api.RunSyncService)}").Value);
        public String ApiCloudUrl => _configuration.GetSection($"{nameof(WebApiConstants.Api)}:{nameof(WebApiConstants.Api.CloudUrl)}").Value;

        public String MongoDBConnectionString => _configuration.GetSection($"{nameof(WebApiConstants.MongoDB)}:{nameof(WebApiConstants.MongoDB.ConnectionString)}").Value;
        public String MongoDBName => _configuration.GetSection($"{nameof(WebApiConstants.MongoDB)}:{nameof(WebApiConstants.MongoDB.Name)}").Value;
    }
}
