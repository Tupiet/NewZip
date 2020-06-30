using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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

            var dialogFailed = new ContentDialog();
            var dialogComplete = new MessageDialog("¡Perfecto! Su archivo ya fue comprimido.");

            dialogFailed.Title = "¿Desea sobreescribir?";
            dialogFailed.Content = "Parece que este archivo ya existe.";
            dialogFailed.PrimaryButtonText = "Sobreescribir";
            dialogFailed.CloseButtonText = "Cancelar";
            dialogFailed.DefaultButton = ContentDialogButton.Primary;

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

            try
            {
                if (await Task.Run(() => File.Exists(zipFolder.Path + "/" + wishedZipName + ".zip")))
                {
                    var result = await dialogFailed.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        await Task.Run(() => File.Delete(zipFolder.Path + "/" + wishedZipName + ".zip"));
                        progressItem.Visibility = Visibility.Visible;
                        await Task.Run(() => ZipFile.CreateFromDirectory(newFolder.Path, zipFolder.Path + "/" + wishedZipName + ".zip"));
                        progressItem.Visibility = Visibility.Collapsed;
                        await dialogComplete.ShowAsync();
                    }
                }
                else
                {
                    progressItem.Visibility = Visibility.Visible;
                    await Task.Run(() => ZipFile.CreateFromDirectory(newFolder.Path, zipFolder.Path + "/" + wishedZipName + ".zip"));
                    progressItem.Visibility = Visibility.Collapsed;
                    await dialogComplete.ShowAsync();
                }
            }
            catch
            {
                Debug.WriteLine("Oops...");
            }           
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string zipName = WishedFolder.Text;

            var dialogFailed = new ContentDialog();
            var dialogComplete = new MessageDialog("¡Perfecto! Su archivo ya fue descomprimido.");

            dialogFailed.Title = "¿Desea sobreescribir?";
            dialogFailed.Content = "Parece que esta carpeta ya existe.";
            dialogFailed.PrimaryButtonText = "Sobreescribir";
            dialogFailed.CloseButtonText = "Cancelar";
            dialogFailed.DefaultButton = ContentDialogButton.Primary;

            var zipPicker = new FileOpenPicker();
            zipPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            zipPicker.FileTypeFilter.Add(".zip");

            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.FileTypeFilter.Add("*");

            try
            {
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
                    var result = await dialogFailed.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        await Task.Run(() => Directory.Delete(folder.Path + "/" + zipName, true));
                        progressItem.Visibility = Visibility.Visible;
                        await Task.Run(() => ZipFile.ExtractToDirectory(file.Path, folder.Path + "/" + zipName));
                        progressItem.Visibility = Visibility.Collapsed;
                        await dialogComplete.ShowAsync();
                    }
                }
                else
                {
                    progressItem.Visibility = Visibility.Visible;
                    await Task.Run(() => ZipFile.ExtractToDirectory(file.Path, folder.Path + "/" + zipName));
                    progressItem.Visibility = Visibility.Collapsed;
                    await dialogComplete.ShowAsync();
                }
            } catch
            {
                Debug.WriteLine("Oops...");
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

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutCompress.ShowAt(CenterText);
        }
    }
}
