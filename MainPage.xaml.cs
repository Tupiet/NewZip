using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace NewZip
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string wishedZipName = WishedName.Text;

            var dialogFailed = new MessageDialog("¡Vaya! Parece que ya existe un archivo llamado " + wishedZipName + ". Por favor, cambie el nombre.");
            var dialogComplete = new MessageDialog("¡Perfecto! Su archivo ya fue comprimido.");

            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            var folderFinal = new FolderPicker();
            folderFinal.SuggestedStartLocation = PickerLocationId.Desktop;
            folderFinal.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            StorageFolder folderEnd = await folderFinal.PickSingleFolderAsync();

            StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
            StorageApplicationPermissions.FutureAccessList.AddOrReplace("ZipFinal", folderEnd);

            StorageFolder newFolder;
            StorageFolder zipFolder;

            newFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
            zipFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("ZipFinal");

            if (await Task.Run(() => File.Exists(zipFolder.Path + "/" + wishedZipName + ".zip")))
            {
                await dialogFailed.ShowAsync();
            } else
            {
                await Task.Run(() => ZipFile.CreateFromDirectory(newFolder.Path, zipFolder.Path + "/" + wishedZipName + ".zip"));
                await dialogComplete.ShowAsync();
            }
            
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string zipName = WishedFolder.Text;

            var dialogFailed = new MessageDialog("¡Vaya! Parece que ya existe una carpeta llamada " + zipName + ". Por favor, cambie el nombre.");
            var dialogComplete = new MessageDialog("¡Perfecto! Su archivo ya fue descomprimido.");

            var zipPicker = new FileOpenPicker();
            zipPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            zipPicker.FileTypeFilter.Add(".zip");

            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.FileTypeFilter.Add("*");

            StorageFile fileStorage = await zipPicker.PickSingleFileAsync();
            StorageFolder folderStorage = await folderPicker.PickSingleFolderAsync();

            StorageApplicationPermissions.FutureAccessList.AddOrReplace("ZipFile", fileStorage);
            StorageApplicationPermissions.FutureAccessList.AddOrReplace("Folder", folderStorage);

            StorageFile file;
            StorageFolder folder;

            file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync("ZipFile");
            folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("Folder");

            if (await Task.Run(() => Directory.Exists(folder.Path + "/" + zipName)))
            {
                await dialogFailed.ShowAsync();
            } else
            {
                await Task.Run(() => ZipFile.ExtractToDirectory(file.Path, folder.Path + "/" + zipName));
                await dialogComplete.ShowAsync();
            }
        }

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            if (this.RequestedTheme == ElementTheme.Dark)
            {
                this.RequestedTheme = ElementTheme.Light;
                ThemeImage.Glyph = "\xE706";

            }
            else
            {
                this.RequestedTheme = ElementTheme.Dark;
                ThemeImage.Glyph = "\xE708";
            }
        }
    }
}
