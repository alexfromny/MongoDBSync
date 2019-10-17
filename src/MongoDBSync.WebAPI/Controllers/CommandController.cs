using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDBSync.WebAPI.App_Start;
using MongoDBSync.WebAPI.Domain;
using MongoDBSync.WebAPI.Helpers;
using MongoDBSync.WebAPI.Repositories;
using MongoDBSync.WebAPI.ViewModels.Command;

namespace MongoDBSync.WebAPI.Controllers
{
    [Route(Endpoints.Command.Base)]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommandRepository _commandRepository;

        public CommandController(IMapper mapper, ICommandRepository commandRepository)
        {
            _mapper = mapper;
            _commandRepository = commandRepository;
        }

        [HttpGet(Endpoints.Command.Sync)]
        public async Task<ActionResult> SyncCommands()
        {
            return Ok((await _commandRepository.GetAllNotSynced()).Select(_mapper.Map<CommandViewModel>));
        }

        [HttpPost(Endpoints.Command.Sync)]
        public async Task<ActionResult> SyncCommands([FromBody] IEnumerable<CommandViewModel> commands)
        {
            await _commandRepository.SaveManyAsync(commands.Select(_mapper.Map<Command>));

            return Ok();
        }

        [HttpPost(Endpoints.Command.Execute)]
        public async Task<ActionResult> ExecuteCommands()
        {
            var guids = MongoDBHelper.RunCommands(_commandRepository, await _commandRepository.GetAllNotSynced());

            return Ok(guids);
        }
    }
}