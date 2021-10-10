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
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, m_hwnd);


            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                FolderPath.Text = "Picked folder: " + folder.Path;
            }
            else
            {
                FolderPath.Text = "Operation cancelled.";
            }
        }

    }
}
