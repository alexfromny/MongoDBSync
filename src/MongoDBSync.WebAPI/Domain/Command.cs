using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBSync.WebAPI.Domain
{
    public class Command
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }

        [BsonElement("guid")]
        public String Guid { get; set; } = System.Guid.NewGuid().ToString();

        [BsonElement("collection")]
        public String Collection { get; set; }

        [BsonElement("jCommand")]
        public String JCommand { get; set; }

        [BsonElement("timestamp")]
        public BsonDateTime Timestamp { get; set; }

        [BsonElement("isSynced")]
        public BsonBoolean IsSynced { get; set; }
    }
}
