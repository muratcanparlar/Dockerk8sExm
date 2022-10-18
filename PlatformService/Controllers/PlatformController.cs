using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController:ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _platformRepo;

        public PlatformController(IPlatformRepo platformRepo, IMapper mapper)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
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
        public ActionResult<IEnumerable<Platform>> Create(Platform platform)
        {
            _platformRepo.CreatePlatform(platform);
            var result=_platformRepo.SaveChanges();
            return result ? (ActionResult<IEnumerable<Platform>>)Ok() : (ActionResult<IEnumerable<Platform>>)BadRequest();
        }

      
    }
}
