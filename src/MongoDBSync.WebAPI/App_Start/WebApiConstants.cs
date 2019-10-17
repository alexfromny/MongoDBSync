using System;

namespace MongoDBSync.WebAPI.App_Start
{
    internal static class WebApiConstants
    {
        internal static class MongoDB
        {
            internal const String ConnectionString = "";
            internal const String Name = "";
        }

        internal static class Api
        {
            internal const Int32 SyncPeriod = 0;
            internal const Boolean RunSyncService = false;
            internal const String CloudUrl = "";
            internal const String LocalhostUrl = "";
        }
    }
}
