using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TundraEngine.Studio.Controls
{
    public class FileBrowserItem
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public bool IsDirectory;
        public List<FileBrowserItem> Items { get; set; }
        public string Icon { get; set; } = "/Assets/folder_white.svg";
        /// <summary>
        /// If the icon shown should be loaded from the file instead
        /// </summary>
        public bool UseCustomIcon { get; set; } = false;

        public FileBrowserItem(string name, string path, bool isDirectory, List<FileBrowserItem>? Items = null, string? Icon = null)
        {
            //Dock.Avalonia.Controls.DockControl dock;
            this.FileName = name;
            this.Path = path;
            IsDirectory = isDirectory;
            if (Items != null)
                this.Items = Items;
            else
                this.Items = new();

            if (Icon != null) this.Icon = Icon;
        }
    }
    public partial class FileBrowser : UserControl
    {
        private string _cwd = "";
        public string CurrentWorkingDirectory { get => _cwd; set { _cwd = value; RefreshItems(); } }
        public delegate void FileOpenHandler(FileBrowserItem item);
        public event FileOpenHandler? FileOpen;
        public bool InEditorUse { get; set; } = true;
        public FileBrowser()
        {
            InitializeComponent();
            DataContext = this;
            RefreshItems();
            MainTree.DoubleTapped += MainTree_DoubleTapped;
            MainTree.SelectionChanged += MainTree_SelectionChanged;
        }

        private void MainTree_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //Console.WriteLine(e.ToString());
        }

        private void MainTree_DoubleTapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var item = ((IVisual)e.Source).GetSelfAndVisualAncestors()
                .OfType<TreeViewItem>()
                .FirstOrDefault();

            if (item != null)
            {
                // Do whatever you need with item here.
                e.Handled = true;

                FileBrowserItem fbi = (FileBrowserItem)item.DataContext!;
                if (fbi.IsDirectory == false)
                {
                    // Open a new tab
                    FileOpen?.Invoke(fbi);
                }
            }
        }

        public void HandleTreeViewItemClick(object? sender, RoutedEventArgs e)
        {

        }

        public void RefreshItems()
        {
            MainTree.Items = GetFileBrowserItems(_cwd);
        }
        string[] skipFolders = new string[] { "bin", "obj", ".git", ".vscode" };
        public List<FileBrowserItem> GetFileBrowserItems(string path)
        {
            var items = new List<FileBrowserItem>();

            if (path != "")
            {
                // Directories
                foreach (var dir in Directory.GetDirectories(path))
                {
                    var relativePath = dir.Remove(0, path.Length);
                    if (InEditorUse && skipFolders.Contains(relativePath))
                    {
                        // Hide some folders
                        continue;
                    }
                    var item = new FileBrowserItem(Path.GetFileName(dir), dir, true);
                    item.Items = GetFileBrowserItems(item.Path);
                    items.Add(item);
                }

                // Files
                foreach (var file in Directory.GetFiles(path))
                {
                    var item = new FileBrowserItem(Path.GetFileName(file), file, false, Icon: "/Assets/file_white.svg");
                    if (file.EndsWith(".png"))
                    {
                        // TODO: handle other image formats
                        item.Icon = file;
                        item.UseCustomIcon = true;
                    }
                    items.Add(item);
                }
            }
            return items;
        }
    }
}
