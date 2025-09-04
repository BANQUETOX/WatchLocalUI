using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchLocalUI.WatchLocalLogic.Managers;
using WatchLocalUI.WatchLocalLogic.Classes;
using Watch_Local.Managers;

namespace WatchLocalUI.Presentation;

public class ChannelViewModel
{
    private INavigator _navigator;
    public string? ChannelName { get; set; }
    public string? ChannelScannAfter { get; set; }
    public List<VideoItem> VideoList { get; set; }
    public string? ChannelPicturePath { get; set; }
    public ChannelViewModel(ChannelTime Channel, INavigator navigator)
    {
        _navigator = navigator;
        VideoList = new List<VideoItem>();
        ChannelName = Channel.ChannelPropeties.Title;
        ChannelScannAfter = Channel.GetScannAfter().Date.ToShortDateString();
        ChannelPicturePath = Channel.picturePath;   
        getChannelVideos();
        _navigator = navigator;
    }
    public void getChannelVideos()
    {
        var mediaDirectoryPath = StorageManager.GetMediaDirectoryPath();
        var cleanChannelName = DownloadManager.CleanUpTitleName(ChannelName); 
        var channelDirectory = new DirectoryInfo(mediaDirectoryPath + $"/{cleanChannelName}");
        if (!channelDirectory.Exists)
        {
            return;
        }
        else 
        {
           foreach ( var dir in channelDirectory.EnumerateDirectories())
            {
                foreach( var file in dir.EnumerateFiles())
                {
                    if (file.Extension == ".mp4")
                    {
                        var video = new VideoItem();
                        video.Name = file.Name;
                        video.VideoPath = file.FullName;
                        video.TumbNailPath = $"{file.Directory}/480x360.jpg";
                        VideoList.Add(video);
                    }
                }
            }
        }
    }

    public async Task GoBack()
    {
        await _navigator.NavigateViewModelAsync<SecondModel>(this);
    }

}
