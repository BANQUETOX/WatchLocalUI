using System.ComponentModel;
using System.Runtime.CompilerServices;
using WatchLocalUI.WatchLocalLogic.Classes;
using WatchLocalUI.WatchLocalLogic.Managers;
using Windows.Devices.HumanInterfaceDevice;

namespace WatchLocalUI.Presentation;

public class ChannelSettingsModel
{
    public event PropertyChangedEventHandler PropertyChanged;
    private INavigator _navigator;

    public ChannelTime _channel { get; set; }
    public string? ChannelName { get; set; }
    public string? ChannelScannAfter { get; set; }
    public string? ChannelPicturePath { get; set; }

    private bool? _channelOnlyAudio;
    private bool? _channelScann { get; set; }
    private bool? _channelAutoScann { get; set; }
    private DateTimeOffset? _selectedDate { get; set; }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ChannelSettingsModel(ChannelTime Channel, INavigator navigator)
    {
        _navigator = navigator;
        _channel = Channel;
        ChannelName = Channel.ChannelPropeties.Title;
        ChannelScannAfter = Channel.GetScannAfter().Date.ToShortDateString();
        ChannelPicturePath = Channel.picturePath;
        _channelOnlyAudio = Channel.onlyAudio;
        _channelAutoScann = Channel.updateScannDate;
        _channelScann = Channel.scann;
    }

    public bool? ChannelOnlyAudio
    {
        get => _channelOnlyAudio;
        set
        {
            if (_channelOnlyAudio != value)
            {
                _channelOnlyAudio = value;
                OnPropertyChanged();

                // Call your model function here
                _channel.ToggleOnlyAudio();
            }
        }
    }

    public bool? ChannelScann
    {
        get => _channelScann;
        set
        {
            if (_channelScann != value)
            {
                _channelScann = value;
                OnPropertyChanged();

                // Call your model function here
                _channel.ToggleScann();
            }
        }
    }
    public bool? ChannelAutoScann
    {
        get => _channelAutoScann;
        set
        {
            if (_channelAutoScann != value)
            {
                _channelAutoScann = value;
                OnPropertyChanged();

                // Call your model function here
                _channel.ToggleUpdateScannDate();
            }
        }
    }
    public DateTimeOffset? SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate != value)
            {
                _selectedDate = value;
                OnPropertyChanged();

                if (value is not null)
                {
                    _channel.SetScannAfter(value.Value.Date);
                }
            }
        }
    }


}
