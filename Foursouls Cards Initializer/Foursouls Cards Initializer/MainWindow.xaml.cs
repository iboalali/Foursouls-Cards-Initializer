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
        private string message = "%s out of %s done (%s%%)";

        private Dictionary<string, int> IdOffsets = new()
        {
            { "Characters", 10000 },
            { "Eternals", 20000 },
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
            ProgressBarIndicator.IsIndeterminate = true;

            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, m_hwnd);
            DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

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
                if (item.EndsWith("process"))
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

            int maxToProcess = GetNumberOfFiles(path) - 1;

            dispatcherQueue.TryEnqueue(() =>
            {
                ProgressBarIndicator.Maximum = maxToProcess;
                ProgressText.Text = string.Format(message, 0, maxToProcess, 0);
            });

            List<CardModel> cards = GetAllCards(path);
            Dictionary<string, string> fileContent = new Dictionary<string, string>();

            for (int i = 0; i < cards.Count; i++)
            {
                CardModel item = cards[i];
                dispatcherQueue.TryEnqueue(() =>
                {
                    ProgressText.Text = string.Format(message, (i + 1), cards.Count, (((i + 1) / cards.Count) * 100));
                });

                if (fileContent.ContainsKey(item.Type))
                {
                    fileContent[item.Type] += item.ToLine();
                }
                else
                {
                    fileContent.Add(item.Type, item.ToLine());
                }
            }

            Directory.CreateDirectory(Path.Combine(path, "result"));
       
            foreach (var item in fileContent)
            {
                var stream = File.Create(Path.Combine(path, "result", item.Key + ".txt"));
                stream.Dispose();

                File.AppendAllText(Path.Combine(path, "result", item.Key+ ".txt"), item.Value);
            }

            // todo the background process doesn't stop
            // the iu does not update at all
        }

        private int GetNumberOfFiles(string folderName)
        {
            var folder = Directory.EnumerateDirectories(folderName);
            int count = 0;

            foreach (var item in folder)
            {
               count += GetNumberOfFiles(item);
            }

            return count + Directory.GetFiles(folderName).Length;
        }

        private List<CardModel> GetAllCards(string folderName)
        {
            var folder = Directory.EnumerateDirectories(folderName);
            var list = new List<CardModel>();

            foreach (var item in folder)
            {
                list.AddRange(GoToType(item));
            }

            return list;
        }

        private List<CardModel> GoToType(string expansion)
        {
            var list = new List<CardModel>();
            var folder = Directory.EnumerateDirectories(expansion);

            foreach (var item in folder)
            {
                list.AddRange(GoToFile(expansion, item));
            }

            return list;
        }

        private List<CardModel> GoToFile(string expansion, string type)
        {
            var list = new List<CardModel>();
            var files = Directory.GetFiles(type);

            List<string> names = new List<string>(files)
                .Maped((x) => { return Path.GetFileName(x); })
                .Sorted((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));

            var id = IdOffsets[Path.GetFileName(type)];

            for (int i = 0; i < names.Count; i++)
            {
                string item = names[i];
                list.Add(new CardModel(
                    item,
                    Path.GetFileName(expansion),
                    Path.GetFileName(type),
                    id + i + 1
               ));
            }

            return list;
        }
    }
}
