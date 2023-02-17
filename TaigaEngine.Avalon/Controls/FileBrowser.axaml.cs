using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaEdit.Utils;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TundraEngine.Studio.Dialogs;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio.Controls
{
    public class FileBrowserItem : ReactiveObject
    {
        string fileName = "";
        public string FileName
        {
            get => fileName;
            set => this.RaiseAndSetIfChanged(ref fileName, value);
        }
        public string Path { get; set; }
        public FileBrowserItem? Parent = null;
        public bool IsDirectory;
        public bool IsExpanded { get; set; } = false;

        private ObservableCollection<FileBrowserItem> items;
        public ObservableCollection<FileBrowserItem> Items
        {
            get => items;
            set => this.RaiseAndSetIfChanged(ref items, value);
        }
        public string Icon { get; set; } = "/Assets/folder_white.svg";
        /// <summary>
        /// If the icon shown should be loaded from the file instead
        /// </summary>
        public bool UseCustomIcon { get; set; } = false;

        public FileBrowserItem(string name, string path, bool isDirectory, ObservableCollection<FileBrowserItem>? Items = null, string? Icon = null)
        {
            this.FileName = name;
            this.Path = path;
            IsDirectory = isDirectory;
            if (Items != null)
                this.Items = Items;
            else
                this.Items = new();

            if (Icon != null) this.Icon = Icon;
            else
            {
                if (UseCustomIcon == false)
                {
                    // Auto determine file icon
                    if (!IsDirectory)
                    {
                        this.Icon = "/Assets/file_white.svg";
                        if (Path.EndsWith(".png"))
                        {
                            // TODO: Supports other format
                            this.Icon = Path;
                            this.UseCustomIcon = true;
                        }
                    }
                }
            }
        }

        public FileBrowserItem? FindItem(string path, FileBrowserItem? startingPath = null)
        {
            FileBrowserItem obj = startingPath ?? this;
            if (obj.Path == path)
            {
                return obj;
            }

            if (obj.IsDirectory)
            {
                foreach (var item in obj.Items)
                {
                    if (path.StartsWith(item.Path))
                    {
                        var result = FindItem(path, item);
                        if (result != null) return result;
                    }
                }
            }
            return null;
        }

        public void RemoveFromParent()
        {
            if (Parent != null)
                Parent.Items.Remove(this);
            else
                throw new Exception("Item has no parent");
        }
    }
    public partial class FileBrowser : UserControl
    {
        private string _cwd = "";
        public string CurrentWorkingDirectory
        {
            get => _cwd;
            set
            {
                _cwd = value;
                if (value != "") InitializeWatcher(value);
                RefreshItems();
            }
        }
        public delegate void FileOpenHandler(FileBrowserItem item);
        public event FileOpenHandler? FileOpen;
        public bool InEditorUse { get; set; } = true;
        public FileBrowserItem? SelectedFile { get; set; }
        public bool HasFileSelected { get; set; } = false;
        FileSystemWatcher watcher;

        public ObservableCollection<FileBrowserItem> Items { get; private set; }

        public FileBrowser()
        {
            InitializeComponent();
            Items = new ObservableCollection<FileBrowserItem>();
            RefreshItems();
            MainTree.DoubleTapped += MainTree_DoubleTapped;
            MainTree.SelectionChanged += MainTree_SelectionChanged;
            DataContext = this;
        }

        public void InitializeWatcher(string path)
        {
            watcher = new(_cwd);
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnFileWatcherChanged;
            watcher.Created += OnFileWatcherCreated;
            watcher.Deleted += OnFileWatcherDeleted;
            watcher.Renamed += OnFileWatcherRenamed;
            watcher.Error += OnFileWatcherError;

            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private void OnFileWatcherError(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnFileWatcherRenamed(object sender, RenamedEventArgs e)
        {
            var oldPath = e.OldFullPath;
            var oldName = e.OldName;
            var newPath = e.FullPath;
            var newName = (e.Name ?? "").Split(Path.DirectorySeparatorChar).Last();

            var item = FindItem(oldPath);
            if (item != null)
            {
                item.FileName = newName;
                item.Path = newPath;
                //TODO: might need the path to be reactive as well?
            }
            //Console.WriteLine($"[{Path.DirectorySeparatorChar}] Rename {oldName} ({oldPath}) to {newName} ({newPath}), found: {item != null}");

        }

        private void OnFileWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            var item = FindItem(e.FullPath);
            if (item != null)
            {
                item.RemoveFromParent();
            }
        }

        private void OnFileWatcherCreated(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            var name = (e.Name ?? "").Split(Path.DirectorySeparatorChar).Last();
            var isDirectory = File.GetAttributes(path).HasFlag(FileAttributes.Directory);

            var item = new FileBrowserItem(name, path, isDirectory);
            var parentPath = Path.GetDirectoryName(path);

            var parent = FindItem(parentPath);
            if (parent != null)
            {
                parent.Items.Add(item);
                item.Parent = parent;
            }
            else
            {
                throw new DirectoryNotFoundException($"Parent item not found {parentPath}");
            }
        }

        private void OnFileWatcherChanged(object sender, FileSystemEventArgs e)
        {
            //throw new NotImplementedException();
        }


        private async void OnRenameFile(object? sender, RoutedEventArgs e)
        {
            if (SelectedFile != null)
            {
                var window = this.FindAncestorOfType<Window>();
                var newName = await RenameDialog.Show(
                    $"Rename file '{SelectedFile.FileName}'",
                    SelectedFile.FileName,
                    SelectedFile,
                    window);

                if (newName != "" && newName != null)
                {
                    var oldFolder = Path.GetDirectoryName(SelectedFile.Path);
                    var newPath = Path.Join(oldFolder, newName);
                    try
                    {
                        if (SelectedFile.IsDirectory)
                            Directory.Move(SelectedFile.Path, newPath);
                        else
                            File.Move(SelectedFile.Path, newPath, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error", $"Error renaming file: {ex}", window);
                    }
                }
            }
        }
        private async void OnDeleteFile(object? sender, RoutedEventArgs e)
        {
            if (SelectedFile != null)
            {
                var window = this.FindAncestorOfType<Window>();
                if (await ConfirmationDialog.Show(
                    $"Delete '{SelectedFile.FileName}'?",
                    $"Are you sure you want to delete '{SelectedFile.FileName}'?\nThis action can't be undone!",
                    window, isDangerous: true, positiveAction: "Delete") == true)
                {
                    try
                    {
                        if (SelectedFile.IsDirectory)
                            Directory.Delete(SelectedFile.Path, true);
                        else
                            File.Delete(SelectedFile.Path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error", $"Error deleting file: {ex}", window);
                    }
                    finally
                    {
                        SelectedFile = null;
                    }
                }
            }
        }

        public void OnRevealExplorer(object? sender, RoutedEventArgs e)
        {
            if (SelectedFile == null) return;
            var IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (IsWindows)
            {
                var args = @$"/select,""{SelectedFile.Path}""";
                Process.Start("explorer.exe", args);
            }
            else
            {
                throw new NotImplementedException();
                // TODO: implement for other platforms
            }
        }

        private FileBrowserItem? FindItem(string path)
        {
            foreach (var item in MainTree.Items)
            {
                var ti = (item as FileBrowserItem)!;
                if (ti.Path == path) return ti;
                if (ti.IsDirectory)
                {
                    var result = ti.FindItem(path, ti);
                    if (result != null) return result;
                }
            }
            return null;
        }

        private void MainTree_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0];
            if (item != null)
            {
                SelectedFile = (FileBrowserItem?)item;
                HasFileSelected = SelectedFile != null;
            }
            else
                HasFileSelected = false;
        }

        private void Item_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (e.InputModifiers.HasFlag(InputModifiers.LeftMouseButton))
            {
                var item = ((IVisual)e.Source).GetSelfAndVisualAncestors()
                    .OfType<TreeViewItem>()
                    .FirstOrDefault();
                DoDrag(e, item.DataContext as FileBrowserItem);
            }

        }

        private async void DoDrag(PointerEventArgs e, FileBrowserItem item)
        {
            //Console.WriteLine(item.Path);
            DataObject dragData = new DataObject();
            dragData.Set(DataFormats.Text, item.Path);
            
            var result = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Copy);
            //switch (result)
            //{
            //    case DragDropEffects.Copy:
            //        Console.WriteLine("The text was copied");
            //        break;
            //    case DragDropEffects.Link:
            //        Console.WriteLine("The text was linked");
            //        break;
            //    case DragDropEffects.None:
            //        Console.WriteLine("The drag operation was canceled");
            //        break;
            //}
        }


        private void MainTree_DoubleTapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var item = ((IVisual)e.Source).GetSelfAndVisualAncestors()
                .OfType<TreeViewItem>()
                .FirstOrDefault();

            if (item == null)
                return;
            // Do whatever you need with item here.
            e.Handled = true;

            FileBrowserItem fbi = (FileBrowserItem)item.DataContext!;
            if (fbi.IsDirectory == false)
            {
                // Open a new tab
                FileOpen?.Invoke(fbi);
            }
        }

        public void RefreshItems()
        {
            if (TundraStudio.CurrentProject != null)
            {
                Items.Clear();
                var root = new FileBrowserItem(TundraStudio.CurrentProject.Title, TundraStudio.CurrentProject.Path, true);
                root.IsExpanded = true;
                root.Items.AddRange(GetFileBrowserItems(_cwd, root));
                Items.Add(root);
            }
        }
        string[] skipFolders = { "bin", "obj", ".git", ".vscode" };
        private string[] skipExtensions = { ".png", ".exe", ".dll" };
        public ObservableCollection<FileBrowserItem> GetFileBrowserItems(string path, FileBrowserItem? parent = null)
        {
            var items = new ObservableCollection<FileBrowserItem>();

            if (path == "")
                return items;
            // Directories
            foreach (var dir in Directory.GetDirectories(path))
            {
                var relativePath = dir.Remove(0, path.Length+1);
                if (InEditorUse && skipFolders.Contains(relativePath))
                {
                    // Hide some folders
                    continue;
                }
                var item = new FileBrowserItem(Path.GetFileName(dir), dir, true);
                item.Items = GetFileBrowserItems(item.Path, item);
                item.Parent = parent;
                items.Add(item);
            }

            // Files
            foreach (var file in Directory.GetFiles(path))
            {
                foreach (var ext in skipExtensions)
                {
                    if (file.EndsWith(ext))
                        goto End;
                }
                var item = new FileBrowserItem(Path.GetFileName(file), file, false)
                {
                    Parent = parent
                };
                items.Add(item);
                    
                End: ;
            }
            return items;
        }

        public void OnCreateFileClicked(object? sender, RoutedEventArgs e)
        {
            var dlg = new CreateFileDialog();
            dlg.ShowDialog(this.FindAncestorOfType<Window>());
        }
    }
}
