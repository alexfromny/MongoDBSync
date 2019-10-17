using System;
using MongoDBSync.WebAPI.Helpers;
using Newtonsoft.Json;

namespace MongoDBSync.WebAPI.ViewModels.Command
{
    public class CommandViewModel
    {
        [JsonProperty("guid")]
        public String Guid { get; set; } = System.Guid.NewGuid().ToString();

        [JsonProperty("operationId")]
        public String OperationId { get; set; }

        [JsonProperty("collection")]
        public String Collection { get; set; }

        [JsonProperty("jCommand")]
        public String JCommand { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("isSynced")]
        public Boolean IsSynced { get; set; } = false;

        public CommandViewModel()
        {
        }

        public CommandViewModel(String id, String method, String collection, String values)
        {
            OperationId = id;
            Collection = collection;
            JCommand = "{" + $"{method}: \"{collection}\", {MongoDBHelper.GetCommandValueKey(method)}: {values}" + "}";
            Timestamp = DateTime.Now;
        }
    }
}
