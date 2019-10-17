using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDBSync.WebAPI.Domain;

namespace MongoDBSync.WebAPI.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private IMongoCollection<User> _userCollection;
        private IMongoDatabase _db;

        public UserRepository(MongoClientSettings dbSettings, String dbName)
        {
            var client = new MongoClient(dbSettings);
            _db = client.GetDatabase(dbName);
        }

        private IMongoCollection<User> UserCollection =>
            _userCollection ?? (_userCollection = _db.GetCollection<User>(DBCollections.Users.ToString()));

        public async Task<List<User>> GetAll()
        {
            return await UserCollection
                .Find(new BsonDocument())
                .ToListAsync();
        }

        public async Task<User> GetById(String id)
        {
            return await UserCollection
                .AsQueryable()
                .FirstOrDefaultAsync(tdi => tdi.UserId == id);
        }

        public async Task<User> GetByEmail(String email)
        {
            return await UserCollection
                .AsQueryable()
                .FirstOrDefaultAsync(tdi => tdi.Email == email);
        }

        public async Task Save(User user)
        {  
            await UserCollection.InsertOneAsync(user);
        }

        public bool Delete(String id)
        {
            var result = UserCollection.DeleteOne(tdi => tdi.UserId == id);

            return result.IsAcknowledged && result.DeletedCount == 1;
        }
    }
}
