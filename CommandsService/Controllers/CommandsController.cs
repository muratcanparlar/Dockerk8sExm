using Microsoft.AspNetCore.Mvc;
using CommandsService.Data;
using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo,IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatforms(int platformId)
        {
            Console.WriteLine($"-->Hit GetCommandsForPlatForm: {platformId}");
            if (!_repo.PlatformExist(platformId))
            {
                return NotFound("PlatformId does not exist");
            }
            var res=_repo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(res));

        }

        [HttpGet("{commandId}",Name ="GetCommandForPlatform")]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandForPlatform(int platformId,int commandId)
        {
            Console.WriteLine($"-->Hit GetCommandForPlatForm: {platformId} - {commandId}");
            if (!_repo.PlatformExist(platformId))
            {
                return NotFound("PlatformId does not exist");
            }
            var res=_repo.GetCommand(platformId,commandId);
            if (res == null)
            {
                return NotFound("Did not find any comment");
            }
            return Ok(_mapper.Map<CommandReadDto>(res));
        }
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId,CommandCreateDto commandDto)
        {
            Console.WriteLine($"-->Hit CreateCommandForPlatform: {platformId}");
            if (!_repo.PlatformExist(platformId))
            {
                return NotFound("PlatformId does not exist");
            }
            var command = _mapper.Map<Command>(commandDto);
            _repo.CreateCommand(platformId, command);
            var res=_repo.SaveChanges();
            var commandReadDto=_mapper.Map<CommandReadDto>(command);
            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { platformId = platformId, commandId = commandReadDto.Id },commandReadDto);
        }
    }
}
