using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Watch_Local.Managers;
using WatchLocalUI.WatchLocalLogic.Classes;

namespace WatchLocalUI.WatchLocalLogic.Managers;

public class StorageManager
{
    private static string storageFilePath = "";
    private static string mediaDirectoryPath = "";

    public static void SetUpStorageFilePath()
    {
        string filePath = Path.Combine(Environment.CurrentDirectory + "/Storage.json");
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
        }
        
        storageFilePath = filePath;
    }

    public static string GetStorageFilePath()
    {
        return storageFilePath;
    }
    public static void SetMediaDirectoryPath(string mediaPath)
    {
        mediaDirectoryPath = mediaPath;
    }
    public static string GetMediaDirectoryPath()
    {
        return mediaDirectoryPath;
    }

    public static void LoadSavedInfo()
    {

        Dictionary<string, string> LoadedInfo = [];
        using (StreamReader file = File.OpenText(storageFilePath))
        {
            JsonSerializer serializer = new();
            LoadedInfo = (Dictionary<string, string>)serializer.Deserialize(file, typeof(Dictionary<string, string>))!;
        }
        if (LoadedInfo != null)
        {
            ChannelManager.SetChannelsSaved(JsonConvert.DeserializeObject<List<ChannelTime>>(LoadedInfo["channels"])!);
            mediaDirectoryPath = LoadedInfo["outputPath"];
        }



    }
    public static void SaveChanges()
    {
        JsonSerializer serializer = new();
        Dictionary<string, string> storage = new()
        {
            { "outputPath", mediaDirectoryPath },
            { "channels", JsonConvert.SerializeObject(ChannelManager.GetChannelsListed(), Formatting.None) }
        };
        using StreamWriter file = File.CreateText(storageFilePath);
        serializer.Serialize(file, storage);
    }
    public static void ClearStorage()
    {
        File.WriteAllText(storageFilePath, string.Empty);
        Console.WriteLine("Json cleared");
    }
}
