using System.Collections;
using System.Collections.Generic;
using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepo
    {
        //Platforms
        bool SaveChanges();
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        bool PlatformExist(int platformId);
        
        //Commands
        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}