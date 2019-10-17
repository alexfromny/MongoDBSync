using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDBSync.WebAPI.Domain;

namespace MongoDBSync.WebAPI.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();

        Task<User> GetById(String id);

        Task<User> GetByEmail(String email);

        Task Save(User user);

        Boolean Delete(String id);
    }
}
