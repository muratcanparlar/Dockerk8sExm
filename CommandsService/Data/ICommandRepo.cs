using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandRepo
    {
        bool SaveChanges();

        //Platform
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        bool PlatformExist(int platformId);

        //Command
        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Command GetCommand(int platformId,int commandId);
        void CreateCommand(int platformId,Command command);
    }
}
