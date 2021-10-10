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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Foursouls_Cards_Initializer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private IntPtr m_hwnd;
        
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
            ProgressBarIndicator.IsIndeterminate = true;

            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, m_hwnd);
            DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                FolderPath.Text = "Picked folder: " + folder.Path;


                await DoWork(dispatcherQueue, folder.Path);
            }
            else
            {
                ProgressBarIndicator.IsIndeterminate = false;
            }
        }

        private async Task DoWork(DispatcherQueue dispatcherQueue, string path)
        {
            await Task.Run(() => InitialProcess(dispatcherQueue, path));
        }

        private async void InitialProcess(DispatcherQueue dispatcherQueue, string path)
        {
            var files = Directory.GetFiles(path);
            bool found = false;

            foreach (var item in files)
            {
                if (item == "process")
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    ProgressBarIndicator.ShowError = true;
                    ProgressText.Text = "Not the correct folder. Couldn't find the file \"process\"";
                });

                return;
            }

            int maxToProcess = 0;

            var folder = Directory.EnumerateDirectories(path);

            foreach (var item in folder)
            {
                
            }
        }
    }
}
