using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls.Primitives;
using Watch_Local.Managers;
using WatchLocalUI.WatchLocalLogic.Classes;
using WatchLocalUI.WatchLocalLogic.Managers;
namespace WatchLocalUI.Presentation;

public partial record SecondModel : INotifyPropertyChanged
{
    public List<ChannelTime>? Channels = ChannelManager.GetChannelsListed();
    private string? _youtubeLink;
    private bool _isChannelButtonEnable;
    private YoutubeLinkType _linkType = YoutubeLinkType.Unknown;
    public event PropertyChangedEventHandler PropertyChanged;
    public IState<string> DownloadButtonVisibility => State<string>.Value(this, () => "Collapsed");
    public IState<string> DownloadTypeComboBoxVisibility => State<string>.Value(this, () => "Collapsed");
    public IState<string> AddChannelButtonVisibility => State<string>.Value(this, () => "Collapsed");
    public IState<string> SelectedIndex => State<string>.Value(this, () => "0");
    public IState<DateTimeOffset> SelectedDate => State<DateTimeOffset>.Value(this, () => DateTimeOffset.Now);

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public SecondModel()
    {
    }
    public string YoutubeLink
    {
        get => _youtubeLink;
        set
        {
            if (_youtubeLink != value)
            {
                _youtubeLink = value;
                OnPropertyChanged();
                LinkType = GetYoutubeLinkType(_youtubeLink);
                _ = ModifyVisibility(LinkType);
            }
        }
    }
    public YoutubeLinkType LinkType
    {
        get => _linkType;
        private set
        {
            if (_linkType != value)
            {
                _linkType = value;
                OnPropertyChanged();
            }
        }
    }

    
    public enum YoutubeLinkType
    {
        Unknown,
        Channel,
        Video,
        Playlist
    }

    public static YoutubeLinkType GetYoutubeLinkType(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return YoutubeLinkType.Unknown;

        try
        {
            var uri = new Uri(url);

            // Normalize host
            var host = uri.Host.Replace("www.", "").ToLowerInvariant();

            if (host != "youtube.com" && host != "youtu.be")
                return YoutubeLinkType.Unknown;

            // Check for channel
            if (uri.AbsolutePath.StartsWith("/@", StringComparison.OrdinalIgnoreCase) || uri.AbsolutePath.Contains("/channel/"))
                return YoutubeLinkType.Channel;

            // Playlist check
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (!string.IsNullOrEmpty(query["list"]))
                return YoutubeLinkType.Playlist;

            // Video check
            if (host == "youtu.be" || query["v"] != null)
                return YoutubeLinkType.Video;
        }
        catch
        {
            return YoutubeLinkType.Unknown;
        }

        return YoutubeLinkType.Unknown;
    }

    public async Task ModifyVisibility(YoutubeLinkType linkType)
    {
        if (linkType == YoutubeLinkType.Video || linkType == YoutubeLinkType.Playlist)
        {
            await DownloadTypeComboBoxVisibility.Set("Visible", CancellationToken.None);
            await DownloadButtonVisibility.Set("Visible", CancellationToken.None);

        }
        else if (linkType == YoutubeLinkType.Channel)
        {
            await AddChannelButtonVisibility.Set("Visible", CancellationToken.None);
            await DownloadTypeComboBoxVisibility.Set("Collapsed", CancellationToken.None);
            await DownloadButtonVisibility.Set("Collapsed", CancellationToken.None);

        }
        else if (linkType == YoutubeLinkType.Unknown)
        {
            await DownloadTypeComboBoxVisibility.Set("Collapsed", CancellationToken.None);
            await DownloadButtonVisibility.Set("Collapsed", CancellationToken.None);
            await AddChannelButtonVisibility.Set("Collapsed", CancellationToken.None);
        }


    }

    public async Task AddChannel()
    {
        var selectedDate = await SelectedDate.Value();
        Console.WriteLine(selectedDate.Date);
        await ChannelManager.AddChannelToList(_youtubeLink,selectedDate,true);

    }
    public async Task DownloadMedia()
    {
        var downloadType = await SelectedIndex.Value();
        if (LinkType == YoutubeLinkType.Video)
        { 
            await DownloadManager.DownloadSingleMedia(_youtubeLink,downloadType);
        }
        else
        {
            await DownloadManager.DownloadFromPlaylist(_youtubeLink,downloadType);
        }
           

    }

    public async Task ScannAndDownload()
    {
        await DownloadManager.ScannPendingVideos();
        await DownloadManager.DownloadMedia();

    }

    
}
