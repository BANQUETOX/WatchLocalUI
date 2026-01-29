using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchLocalUI.WatchLocalLogic.Classes;
using WatchLocalUI.WatchLocalLogic.Managers;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Search;

namespace Watch_Local.Managers
{
    public class ChannelManager
    {
        private static readonly YoutubeClient youtube = new();
        private static  List<ChannelTime>? channels = [];

        public static void SetChannelsSaved(List<ChannelTime> channelsSaved)
        {
            channels = channelsSaved;
        }
        public static List<ChannelTime>? GetChannelsListed()
        {
            return channels;
        }
        private static ChannelTime? SelectChannel()
        {
            if (channels!.Count == 0)
            {
                Console.WriteLine("No channels listed");
                return null;
            }

            for (int i = 0; i < channels.Count; i++)
            {
                Console.WriteLine($"{i + 1}.{channels[i].ChannelPropeties.Title}");
            }
            int selection = MenuManager.ShowMenu("Type the number of the channel: ", 1, channels.Count);
            return channels[selection - 1];
        }

        public static void DeleteChannel()
        {
            ChannelTime channelSelected = SelectChannel()!;
            if (channelSelected == null)
            {
                return;
            }
            int deleteVideosSelection = MenuManager.ShowMenu("Do you want to remove ALL the media downloaded as well? \n1.Yes \n2.No", 1, 2);
            if (deleteVideosSelection == 1)
            {
                DirectoryManager.DeleteMediaFromDirectory(channelSelected.ChannelPropeties.Title, StorageManager.GetMediaDirectoryPath());
                Console.WriteLine("Videos from directory deleted successfully!");
            }
            channels!.Remove(channelSelected);
            Console.WriteLine("Channel removed from tracking list");
        }

        //public static async Task ChannelsSettingsMenu()
        //{
        //    Console.Clear();
        //    string menu = "Channels Settings: \n1.Add Channel \n2.Delete Channel \n3.Toogle Only audio to channel \n4.Toogle auto scann date update \n5.Change Starting scan date \n6.Exit to main menu";
        //    int selection = MenuManager.ShowMenu(menu, 1, 6);
        //    while (selection < 6)
        //    {
        //        Console.Clear();
        //        switch (selection)
        //        {
        //            case 1:
        //                await AddChannelToList();
        //                break;
        //            case 2:
        //                DeleteChannel();
        //                break;
        //            case 3:
        //                ToggleOnlyAudioInChannel();
        //                break;
        //            case 4: 
        //                ToggleAutoScannUpdate();
        //                break;
        //            case 5:
        //                var channel = SelectChannel();
        //                channel?.SetScannAfter(MenuManager.DatePicker());
        //                break;
        //        }
        //        StorageManager.SaveChanges();
        //        selection = MenuManager.ShowMenu(menu, 1, 6);
        //    }
        //}
        public static void ToggleOnlyAudioInChannel()
        {
            ChannelTime channelSelected = SelectChannel()!;
            if (channelSelected == null)
            {
                return;
            }
            channelSelected.ToggleOnlyAudio();
        }
        public static void ToggleAutoScannUpdate()
        {
            ChannelTime channelSelected = SelectChannel()!;
            if (channelSelected == null)
            {
                return;
            }
            channelSelected.ToggleUpdateScannDate();
        }
        public static void ShowChannels()
        {
            try
            {
                if (channels is null || channels.Count == 0)
                {
                    Console.WriteLine("There are no channels listed");
                }
                else
                {
                    Console.WriteLine("Channels listed");
                    foreach (var channel in channels)
                    {
                        string lastVideoMessage;
                        string lastVideoName = GetLastVideoDownloaded(channel.ChannelPropeties);
                        if (lastVideoName != "") { 
                        lastVideoMessage = $" - Last media downloaded: {lastVideoName}";
                        }
                        else
                        {
                            lastVideoMessage = " - No videos have been downloaded for this channel";
                        }


                        Console.WriteLine("\n" + $"* {channel.ChannelPropeties.Title}"+ $" - Start scanning date: {channel.scannAfter.DateTime.ToShortDateString()}\n" + lastVideoMessage + "\n" + $" - OnlyAudio: {channel.onlyAudio} // ScannUpdate: {channel.updateScannDate} \n");

                    }
                }
            }
            catch (Exception e)
            {
                //Log the error (TODO)
                Console.WriteLine(e);
                Console.WriteLine("There are no channels listed");
            }

        }

        private static string GetLastVideoDownloaded(YoutubeExplode.Channels.Channel channel)
        {
            string channelName = channel.Title;
            var mediaDirectoryPath = StorageManager.GetMediaDirectoryPath();
            var dir = new DirectoryInfo(mediaDirectoryPath + $"/{channelName}");
            if (!dir.Exists)
            {
                return "No videos downloaded yet";
            }
            var dirFiles = dir.GetFiles();
            if (dirFiles.Length == 0)
            {
                return "";
            }
            else
            {
                var file = dir.GetFiles().OrderByDescending(f => f.LastWriteTime).Last();
                return file.Name;
            }


        }
        public static async Task AddChannelToList(string inputUrl, DateTimeOffset scanningDate, bool scann)
        {
            YoutubeExplode.Channels.Channel? channel = null;

            try
            {

                if (inputUrl.Contains("/channel/"))
                {
                    await foreach (var result in youtube.Search.GetChannelsAsync(inputUrl))
                    {
                        // result is ChannelSearchResult
                        channel = await youtube.Channels.GetAsync(result.Id);
                        break; // just grabbing one
                    }
                }
                else
                {
                    channel = await youtube.Channels.GetByHandleAsync(inputUrl);
                }
                if (channels!.Any(channelList => channelList.ChannelPropeties.Url == channel.Url))
                {
                    Console.WriteLine("Channel already on list");
                }
                else
                {
                    //Sets OnlyAudio as false and updateScannDate as true in every channel by default
                    var cleanChannelName = DownloadManager.CleanUpTitleName(channel.Title);
                    DownloadManager.UpdateChannelIcon(channel);
                    ChannelTime channelToSave = new(channel, scanningDate, false, true, $"{StorageManager.GetMediaDirectoryPath()}/{cleanChannelName}/{cleanChannelName}.jpg", scann);
                    channels!.Add(channelToSave);
                    if (scann)
                    {
                        Console.WriteLine($"{channel} added for tracking successfully");
                    }
                    StorageManager.SaveChanges();
                }

            }
            catch (Exception e)
            {
                //Send exeption to a kind of LOG (TODO)
                Console.WriteLine("Channel not found \n");
                Console.WriteLine(e);
            }
        }

    }



}
