using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBSync.WebAPI.Domain
{
    public class User
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }

        [BsonElement("userId")]
        public String UserId { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("nick")]
        public String Nick { get; set; }

        [BsonElement("email")]
        public String Email { get; set; }

        [BsonElement("registedDate")]
        public BsonDateTime RegistedDate { get; set; }
    }
}
