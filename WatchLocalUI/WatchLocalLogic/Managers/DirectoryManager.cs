using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchLocalUI.WatchLocalLogic.Managers;

namespace Watch_Local.Managers
{
    public class DirectoryManager
    {
        public static void MoveFilesToNewDirectory(string currentDirectoryPath, string newDirectoryPath)
        {
            var currentdir = new DirectoryInfo(currentDirectoryPath);
            foreach (string pathOfFile in Directory.GetFiles(currentDirectoryPath, "*", SearchOption.AllDirectories))
            {
                var file = new FileInfo(pathOfFile);
                string newPath = newDirectoryPath + $"\\{file.Directory!.Name}\\{file.Name}";
                file.MoveTo(newPath);
            }

        }

        public static void SetDownloadsDirectory()
        {
            Console.WriteLine("Write the path were the downloads should go:");
            string newDirectory = Console.ReadLine()!;
            while (!Directory.Exists(newDirectory))
            {
                Console.WriteLine("The path don't exist, or is invalid, please enter a valid path: ");
                newDirectory = Console.ReadLine()!;
            }
            StorageManager.SetMediaDirectoryPath(newDirectory);
        }

        public static void DeleteMediaFromDirectory(string channelTitle, string mediaDirectoryPath)
        {
            // (TODO)
            string channelPath = $"{mediaDirectoryPath}/{channelTitle}";
            var dir = new DirectoryInfo(channelPath);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            Directory.Delete(channelPath);


        }





    }
}

