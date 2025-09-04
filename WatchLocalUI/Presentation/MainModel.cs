using WatchLocalUI.WatchLocalLogic.Managers;
using Windows.Storage.Pickers;
namespace WatchLocalUI.Presentation;

public partial record MainModel
{
    private INavigator _navigator;
    public string? Title { get; }
    public IState<string> SelectedFolder => State<string>.Value(this, () => string.Empty);
    public IState<string> ContinueButtonVisibility => State<string>.Value(this, () => "Collapsed");

    public MainModel(IStringLocalizer localizer,IOptions<AppConfig>? appInfo,INavigator navigator){
        StorageManager.SetUpStorageFilePath();
        StorageManager.LoadSavedInfo();
        StorageManager.SaveChanges();
        _navigator = navigator;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
        //Skip to second page if a folder is already selected
        if (StorageManager.GetMediaDirectoryPath() != "")
        {
            Continue().GetAwaiter().GetResult();
        }
    }

    public async Task PickFolder()
    {
        var folderPicker = new FolderPicker();
        folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        folderPicker.FileTypeFilter.Add("*");

        StorageFolder? pickedFolder = await folderPicker.PickSingleFolderAsync();
        if (pickedFolder != null)
        {
            await SelectedFolder.Set(pickedFolder.Path, CancellationToken.None);
            StorageManager.SetMediaDirectoryPath(pickedFolder.Path);
            StorageManager.SaveChanges();
            await ContinueButtonVisibility.Set("Visible", CancellationToken.None);
        }
        else
        {
            // No folder was picked or the dialog was cancelled.
            await SelectedFolder.Set(string.Empty, CancellationToken.None);
        }
    }

    public async Task Continue()
    {
        var path = await SelectedFolder;
        await _navigator.NavigateViewModelAsync<SecondModel>(this);
    }

}
