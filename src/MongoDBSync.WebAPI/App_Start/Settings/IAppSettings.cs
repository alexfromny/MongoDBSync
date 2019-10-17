using System;

namespace MongoDBSync.WebAPI.App_Start.Settings
{
    public interface IAppSettings
    {
        Int32 ApiSyncPeriod { get; }
        String ApiCloudUrl { get; }
        Boolean ApiRunSyncService { get; }

        String MongoDBConnectionString { get; }
        String MongoDBName { get; }
    }
}
