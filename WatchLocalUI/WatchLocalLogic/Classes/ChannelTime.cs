using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchLocalUI.WatchLocalLogic.Classes;

public class ChannelTime
{
    public YoutubeExplode.Channels.Channel ChannelPropeties;
    public DateTimeOffset scannAfter;
    public bool onlyAudio;
    public bool updateScannDate;
    public string picturePath;

    public ChannelTime(YoutubeExplode.Channels.Channel channelPropeties, DateTimeOffset scannAfter, bool onlyAudio, bool updateScannDate, string picturePath)
    {
        ChannelPropeties = channelPropeties;
        this.scannAfter = scannAfter;
        this.onlyAudio = onlyAudio;
        this.updateScannDate = updateScannDate;
        this.picturePath = picturePath;
    }

    public void ToggleOnlyAudio()
    {
        onlyAudio = !onlyAudio;
    }

    public void ToggleUpdateScannDate()
    {
        updateScannDate = !updateScannDate;

    }

    public bool GetIsOnlyAudio()
    {
        return onlyAudio;
    }

    public void SetScannAfter(DateTimeOffset date)
    {
        scannAfter = date;
    }

    public DateTimeOffset GetScannAfter()
    {
        return scannAfter;
    }

}
