using AutoMapper;
using CommandAPI.Data;
using CommandAPI.Dtos;
using CommandAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CommandAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandAPIRepo _repository;
        private readonly IMapper _mapper;
        public CommandsController(ICommandAPIRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> Get()
        {
            var commandItems = _repository.GetAllCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }
        [HttpGet("{id}", Name = "GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);
            if (commandItem == null)
            {
                return NotFound();

            }
            return Ok(_mapper.Map<CommandReadDto>(commandItem));
        }
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(commandModel);
            _repository.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);
            return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDto.id }, commandReadDto);
        }
        [HttpPut("{id}")]
        public ActionResult<CommandReadDto> UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(commandUpdateDto, commandModelFromRepo);
            _repository.UpdateCommand(commandModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, CommandUpdateDto patchDoc)
        {
            var x = JObject.FromObject(patchDoc);

            var commandModelFromRepo = _repository.GetCommandById(id);

            if (commandModelFromRepo == null)
            {
                return NotFound();
            }
            var y = JObject.FromObject(commandModelFromRepo);
            foreach (var item in x)
            {
                if (string.IsNullOrEmpty(item.Value?.ToString()))
                {
                    x[item.Key] = y[item.Key];
                }
            }
            var z = x.ToObject<CommandUpdateDto>();

            // var commandToPatch=_mapper.Map<CommandUpdateDto>(commandModelFromRepo);
            // patchDoc.ApplyTo(commandToPatch,ModelState);
            // if(!TryValidateModel(commandToPatch)){
            //     return ValidationProblem(ModelState);

            // }
            // _mapper.Map(commandToPatch,commandModelFromRepo);

            //  _repository.UpdateCommand(commandModelFromRepo);

            _mapper.Map(z, commandModelFromRepo);
            _repository.UpdateCommand(commandModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
        }
[HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id){
            var commandModelfromRepo=_repository.GetCommandById(id);

            if(commandModelfromRepo==null){
                return NotFound();

            }
            _repository.DeleteCommand(commandModelfromRepo);
            _repository.SaveChanges();
            return NoContent();
        }

    }
}