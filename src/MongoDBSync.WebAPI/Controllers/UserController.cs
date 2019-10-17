using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDBSync.WebAPI.App_Start;
using MongoDBSync.WebAPI.Domain;
using MongoDBSync.WebAPI.Repositories;

namespace MongoDBSync.WebAPI.Controllers
{
    [Route(Endpoints.User.Base)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet(Endpoints.User.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userRepository.GetAll());
        }

        [HttpPost(Endpoints.User.Add)]
        public async Task<IActionResult> AddUser([FromBody] User newUser)
        {
            await _userRepository.Save(newUser);
            return Ok();
        }

        [HttpGet(Endpoints.User.Add)]
        public async Task<IActionResult> AddUser()
        {
            var random = new Random().Next(Int32.MaxValue);
            await _userRepository.Save(new User()
            {
                Nick = $"newuser_{random}",
                Email = $"newuser_{random}@gmail.com",
                RegistedDate = DateTime.Now
            });

            return Ok();
        }

        [HttpGet(Endpoints.User.Delete)]
        public async Task<IActionResult> DeleteUser(String userId)
        {
            _userRepository.Delete(userId);
            return Ok();
        }
    }
}