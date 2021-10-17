using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Win32;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Foursouls_Cards_Initializer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly IntPtr m_hwnd;
        private readonly string message = "%s out of %s done (%s%%)";
        HashSet<string> folders = new HashSet<string>();

        private Dictionary<string, int> IdOffsets = new()
        {
            { "Characters", 10000 },
            { "Starting Items", 20000 },
            { "Monsters", 30000 },
            { "Treasure", 40000 },
            { "Loot", 50000 },
            { "Bonus Souls", 60000 },
            { "Rooms", 70000 }
        };

        public MainWindow()
        {
            InitializeComponent();
            m_hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) => Environment.Exit(Environment.ExitCode);

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog();
        }

        private async void OpenFolderDialog()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.ViewMode = PickerViewMode.List;

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, m_hwnd);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                addToUi(folder.Name);
            }
        }

        private async void Folders_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var storageItems = await e.DataView.GetStorageItemsAsync();

                foreach (var item in storageItems)
                {
                    if (item.Attributes.HasFlag(Windows.Storage.FileAttributes.Directory))
                    {
                        addToUi(item.Path);
                    }
                }
            }

            

            //dragAndDropImage.Visibility = Visibility.Collapsed;
            //e.Handled = true;
        }

        private async void Folders_DragOver(object sender, DragEventArgs e)
        {

            bool areDirectories = true;

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var storageItems = await e.DataView.GetStorageItemsAsync();

                foreach (var item in storageItems)
                {
                    if (!item.Attributes.HasFlag(Windows.Storage.FileAttributes.Directory))
                    {
                        areDirectories = false;
                    }
                }
            }

            if (areDirectories)
            {
                //dragAndDropImage.Visibility = Visibility.Visible;
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        private void Folders_DragLeave(object sender, DragEventArgs e)
        {
            //dragAndDropImage.Visibility = Visibility.Collapsed;
        }

        private void addToUi(string folderPath)
        {
            folders.Add(folderPath);
            Folders.Text = "";

            foreach (var item in folders)
            {
                Folders.Text += Path.GetFileName(item) + Environment.NewLine;
            }
        }
    }
}
