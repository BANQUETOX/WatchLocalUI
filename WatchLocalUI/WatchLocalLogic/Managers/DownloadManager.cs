using System.Net;
using WatchLocalUI.WatchLocalLogic.Classes;
using WatchLocalUI.WatchLocalLogic.Managers;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;
namespace Watch_Local.Managers
{
    public class DownloadManager
    {
        private static YoutubeClient youtube = new YoutubeClient();
        private static List<YoutubeExplode.Videos.Video> videosPending = new();
        private static List<YoutubeExplode.Videos.Video> audiosPending = new();
        private async static Task DownloadMp4Video(YoutubeExplode.Videos.Video video)
        {
            string videoTitle = CleanUpTitleName(video.Title);
            int downloadAttemps = 0;
            while (downloadAttemps < 3)
            {
                try
                {
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Url);
                    var channel = await youtube.Channels.GetAsync(video.Author.ChannelUrl);
                    UpdateChannelIcon(channel);
                    var cleanChannelName = CleanUpTitleName(channel.Title);
                    var audioStream = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                    var videoStream = streamManifest
                    .GetVideoOnlyStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestVideoQuality();
                    var pathForVideo = Directory.CreateDirectory($"{StorageManager.GetMediaDirectoryPath()}/{cleanChannelName}/{videoTitle}");
                    foreach (var tumbnail in video.Thumbnails)
                    {
                        using (WebClient client = new())
                        {
                            client.DownloadFile(new Uri(tumbnail.Url), $"{pathForVideo}/{tumbnail.Resolution}.jpg");
                        }
                    }
                    await youtube.Videos.DownloadAsync(video.Url, $"{pathForVideo.FullName}/{videoTitle}.mp4", o => o
                    .SetPreset(ConversionPreset.UltraFast));
                    downloadAttemps = 5;
                    Console.WriteLine($"{videoTitle} downloaded successfully! ");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to download ${videoTitle}, trying again...");
                    downloadAttemps++;

                }

            }



        }
        private async static Task DownloadAudioFromVideo(YoutubeExplode.Videos.Video video)
        {
            string audioTitle = CleanUpTitleName(video.Title);
            int downloadAttemps = 0;
            while (downloadAttemps < 5)
            {
                try
                {
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Url);
                    var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                    var channel = await youtube.Channels.GetAsync(video.Author.ChannelUrl);
                    UpdateChannelIcon(channel);
                    var cleanChannelName = CleanUpTitleName(channel.Title);
                    var pathForVideo = Directory.CreateDirectory($"{StorageManager.GetMediaDirectoryPath()}/{cleanChannelName}/{audioTitle}");
                    foreach (var tumbnail in video.Thumbnails)
                    {
                        using (WebClient client = new())
                        {
                            client.DownloadFile(new Uri(tumbnail.Url), $"{pathForVideo}/{tumbnail.Resolution}.jpg");
                        }
                    }
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{pathForVideo}/{audioTitle}.mp3");
                    downloadAttemps = 5;
                    Console.WriteLine($"{audioTitle} downloaded successfully!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to download ${audioTitle}, trying again...");
                    downloadAttemps++;
                }
            }
        }

        public async static Task DownloadFromPlaylist(string playListUrl, string mediaSelection)
        {
            //Validate url (TODO)
            var playListvideos = await youtube.Playlists.GetVideosAsync(playListUrl);
            foreach (var playListvideo in playListvideos)
            {
                var video = await youtube.Videos.GetAsync(playListvideo.Url);
                if (VideoDownloaded(video))
                {
                    Console.WriteLine("Video already downloaded");
                }
                else
                {
                    if (mediaSelection == "0")
                    {
                        await DownloadMp4Video(video);
                    }
                    else
                    {
                        await DownloadAudioFromVideo(video);
                    }
                }

            }


        }
        public static void AddVideoToPendingList(YoutubeExplode.Videos.Video video, bool onlyAudio)
        {
            //Check if videos is a stream (TODO)
            if (onlyAudio)
            {
                audiosPending.Add(video);
                Console.WriteLine($"Audio: {video.Title} ADDED TO PENDING LIST");
            }
            else
            {
                videosPending.Add(video);
                Console.WriteLine($"Videos: {video.Title} ADDED TO PENDING LIST");
            }
        }
        private static bool VideoDownloaded(YoutubeExplode.Videos.Video video)
        {
            string channelName = video.Author.ChannelTitle;
            var mediaDirectoryPath = StorageManager.GetMediaDirectoryPath();
            foreach (string dirPath in Directory.GetDirectories(mediaDirectoryPath))
            {
                var dir = new DirectoryInfo(dirPath);
                string videoTitle = CleanUpTitleName(video.Title);
                if (channelName == dir.Name)
                {
                    foreach (string filePath in Directory.GetFiles(dir.FullName))
                    {
                        var file = new FileInfo(filePath);
                        if (file.Name.Contains(videoTitle))
                        {
                            return true;
                        }

                    }
                }
            }
            //Scan mediaFilePath to get the videos
            //Get video metadata

            return false;
        }
        public static async Task DownloadMedia()
        {
            //EDGE CASE!!! SI EL CANAL ESTA EN VIVO!! falla (TODO)
            if (videosPending.Count > 0)
            {
                Console.WriteLine("Downloading pending videos...");
                foreach (var video in videosPending)
                {
                    await DownloadMp4Video(video);

                }
                Console.WriteLine("All the pending videos have been downloaded! \n");
                videosPending.Clear();
            }
            if (audiosPending.Count > 0)
            {
                Console.WriteLine("Downloading pending audios...");
                foreach (var audio in audiosPending)
                {
                    await DownloadAudioFromVideo(audio);
                }
            }
            Console.WriteLine("All pending audio tracks have been downloaded!");
            audiosPending.Clear();
        }
        public static async Task DownloadSingleMedia(string videoUrl, string downloadType)
        {
            //Download Type: 0-> Video 1->Audio
            //Validate url (TODO)
            var video = await youtube.Videos.GetAsync(videoUrl);
            if (VideoDownloaded(video))
            {
                Console.WriteLine("Video already downloaded");
                return;
            }

            if (downloadType == "0")
            {
                await DownloadMp4Video(video);
            }
            else if (downloadType == "1")
            {
                await DownloadAudioFromVideo(video);
            }


        }
        public static string CleanUpTitleName(string title)
        {
            if (title.Contains('<') || title.Contains('>') || title.Contains("\"") || title.Contains(':') || title.Contains('/') || title.Contains("\\") || title.Contains('|') || title.Contains('?') || title.Contains('*'))
            {
                title = title.Replace("<", " ");
                title = title.Replace(">", " ");
                title = title.Replace("\"", " ");
                title = title.Replace("/", " ");
                title = title.Replace("\\", " ");
                title = title.Replace("|", " ");
                title = title.Replace("?", " ");
                title = title.Replace("*", " ");
                title = title.Replace(":", " ");
            }
            return title;

        }
         public static void UpdateChannelIcon(YoutubeExplode.Channels.Channel channel)
        {
            var cleanChannelName = CleanUpTitleName(channel.Title);
            var pathForChannel = Directory.CreateDirectory($"{StorageManager.GetMediaDirectoryPath()}/{cleanChannelName}");
            using (WebClient client = new())
            {
                client.DownloadFile(new Uri(channel.Thumbnails[0].Url), $"{pathForChannel}/{cleanChannelName}.jpg");
            }

        }

        public static async Task ScannPendingVideos()
        {
            Console.WriteLine("Scanning for pending videos...");
            try
            {
                foreach (ChannelTime channelSaved in ChannelManager.GetChannelsListed()!)
                {
                    var videos = await youtube.Channels.GetUploadsAsync($"https://youtube.com/channel/{channelSaved.ChannelPropeties.Id}");
                    foreach (var video in videos)
                    {
                        var NewURL = video.Url.Split('&');
                        var videoToSave = await youtube.Videos.GetAsync(NewURL[0]);
                        if (videoToSave.UploadDate > channelSaved.GetScannAfter())
                        {
                            if (!VideoDownloaded(videoToSave))
                            {
                                AddVideoToPendingList(videoToSave, channelSaved.GetIsOnlyAudio());

                            }
                        }
                        else
                        {
                            Console.WriteLine($"No more videos on time range for {channelSaved.ChannelPropeties.Title}");
                            break;

                        }

                    }
                    if (channelSaved.updateScannDate)
                    {
                        var lastVideo = await youtube.Videos.GetAsync(videos[0].Url.Split('&')[0]);
                        channelSaved.SetScannAfter(lastVideo.UploadDate);
                        StorageManager.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                //Log Error (TODO)
                Console.WriteLine(e);
            }
        }

    }
}
