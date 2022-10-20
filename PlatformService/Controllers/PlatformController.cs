using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController:ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _platformRepo;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformController(
            IPlatformRepo platformRepo,
            IMapper mapper,
            ICommandDataClient commandDataClient)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Platform>> Get()
        {
            var platformList = _platformRepo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformList));
        }
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Platform>> GetById(int id)
        {
            var platform = _platformRepo.GetPlatformById(id);
            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Platform>>> Create(PlatformCreateDto  platformCreateDto)
        {
            var platformModel=_mapper.Map<Platform>(platformCreateDto);  
            _platformRepo.CreatePlatform(platformModel);
            var result=_platformRepo.SaveChanges();
            var platFormReadDto=_mapper.Map<PlatformReadDto>(platformModel);
            try
            {
                await _commandDataClient.SendPlatformToCommand(platFormReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"-->Could not Send syncrously {ex.Message}");
            }
            return result ? (ActionResult<IEnumerable<Platform>>)Ok() : (ActionResult<IEnumerable<Platform>>)BadRequest();
        }

      
    }
}
