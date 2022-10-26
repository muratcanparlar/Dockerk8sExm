using AutoMapper;
using CommandsService.Models;
using CommandsService.Dtos;
namespace CommandsService.Profiles
{
    public class CommandsProfile:Profile
    {
        public CommandsProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
        }
    }
}
