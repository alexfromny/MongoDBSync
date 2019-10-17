using System;

namespace MongoDBSync.WebAPI.App_Start
{
    public class Endpoints
    {
        public static class User
        {
            public const String Base = "api/user";

            public const String Add = "add";
            public const String GetAll = "getall";
            public const String Delete = "delete";
        }

        public static class Command
        {
            public const String Base = "api/command";

            public const String Sync = "sync";
            public const String Execute = "execute";
        }
    }
}
