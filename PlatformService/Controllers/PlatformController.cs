using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
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
        private readonly IMessageBusClient _messageBusClient;

        public PlatformController(
            IPlatformRepo platformRepo,
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
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
            //send sync message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platFormReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"-->Could not Send syncrously {ex.Message}");
            }

            //send async message
            try
            {
                var platformPublishDto=_mapper.Map<PlatformPublishedDto>(platFormReadDto);
                platformPublishDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishDto);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"-->Could not Send asyncrously {ex.Message}");
            }
            return result ? (ActionResult<IEnumerable<Platform>>)Ok() : (ActionResult<IEnumerable<Platform>>)BadRequest();
        }

      
    }
}
