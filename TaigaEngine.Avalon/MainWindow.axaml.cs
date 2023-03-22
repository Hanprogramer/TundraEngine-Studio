using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.IO;
using System.Runtime.InteropServices;
using TundraEngine.Studio.Compiler;
using TundraEngine.Studio.Controls;
using TundraEngine.Studio.Dialogs;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio
{
    public partial class MainWindow : Window
    {
        public string TestString = "TEST";
        public string ProjectPath = "D:\\Programming\\C#\\TaigaEngine.Avalon\\TestGame1\\project.json";
        public static FontFamily CodeFamily = FontFamily.Parse("avares://TundraEngine.Studio/Assets/JetBrainsMono-Regular.ttf#JetBrains Mono");
        private Runtime.Runner? Runner;


        public EditorTab? GameTab = null;
        public MainWindow()
        {
            InitializeComponent();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //TODO: do this on application level maybe? Or for every window
                Win32Native.ImplementDarkTitleBar(this);
            TundraStudio.CurrentProject = TundraProject.Parse(ProjectPath, this);
            TundraStudio.CurrentProject.InitializeResourceManager(null);
            var fb = this.FindControl<FileBrowser>("FileBrowser");
            fb.CurrentWorkingDirectory = TundraStudio.CurrentProject.Path;
            fb.FileOpen += OnFileOpen;
            DataContext = new MainWindowViewModel();
            InitializeProject();
            
            HotKeyManager.SetHotKey(this, KeyGesture.Parse("Ctrl+S"));

        }
        private async void InitializeProject()
        {
            var extraComps = await GameCompiler.AnalyzeProject(
                Path.Join(TundraStudio.CurrentProject.Path,TundraStudio.CurrentProject.CSProject),
                Path.Join(TundraStudio.CurrentProject.Path,"bin")
            );
            foreach (var comp in extraComps)
            {
                TundraStudio.ComponentRegistry.Register(comp);
            }
        }

        /// <summary>
        /// Handler for TabItem middle pressed to close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabItem_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var tab = (e.Source as Control)?.DataContext as EditorTab;
            if (tab != null && e.InitialPressMouseButton == MouseButton.Middle)
            {
                CloseTab(tab);
            }
        }


        // When stop button pressed
        public void OnStopGame(object? sender, RoutedEventArgs e)
        {
            StopGame();
        }

        // Event handler for file open
        private void OnFileOpen(FileBrowserItem item)
        {
            var tab = new EditorTab(item.FileName, item.Path);
            ((MainWindowViewModel)DataContext!).Tabs.Add(tab);
            FileTabs.SelectedItem = tab;
        }

        // Event handler for file close
        public async void OnFileClose(object? s, RoutedEventArgs args)
        {
            if ((args.Source as Button)!.DataContext! is not EditorTab el)
                return;
            if (!el.IsSaved)
            {
                var result = await ConfirmationDialog.Show(
                    "Warning", 
                    "File isn't saved yet. Save now?", 
                    this, 
                    "Save", "Discard", 
                    showCancel: true, showNegative: true);
                switch (result)
                {
                    case true:
                        // Save and close
                        el.Save();
                        CloseTab(el);
                        break;
                    case false:
                        // Close without saving
                        CloseTab(el);
                        break;
                }
                // if null Cancel the closing
            }
            else 
                CloseTab(el);
        }


        // When the play button is clicked
        public async void OnPlayBtnClicked(object? s, RoutedEventArgs args)
        {
            if (TundraStudio.CurrentProject == null) return;
            PlayBtn.IsEnabled = false;
            PauseBtn.IsEnabled = false;
            StopBtn.IsEnabled = false;
            ClearConsole();

            var compileOutputPath = System.IO.Path.Join(TundraStudio.CurrentProject.Path, "bin");
            var resourcesPath = await ResourceCompiler.Compile(TundraStudio.CurrentProject.Path, compileOutputPath);
            var texturesPath = await TextureCompiler.Compile(TundraStudio.CurrentProject.Path, compileOutputPath);
            var settingsPath = await GameCompiler.CompileProjectSettings(TundraStudio.CurrentProject.Path, compileOutputPath);
            var result = await GameCompiler.Compile(TundraStudio.CurrentProject, Log);

            if (result != null)
            {
                foreach (var d in result.Items)
                {
                    Log(d.Title);
                }
                if (result.Success)
                    RunGame(result.DllPath, resourcesPath, texturesPath, settingsPath);
                else
                {
                    PlayBtn.IsEnabled = true;
                    PauseBtn.IsEnabled = false;
                    StopBtn.IsEnabled = false;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (GameTab != null)
            {
                StopGame();
            }
        }

        // Internal logger function
        public void Log(object o)
        {
            Console.WriteLine(o);
            //if (TbConsole != null)
            //{
            //    TbConsole.Text += o.ToString() + "\n";
            //    TbConsole.CaretIndex = TbConsole.Text.Length;
            //}
        }

        /// <summary>
        /// Clears the console text
        /// </summary>
        public void ClearConsole()
        {
            //TbConsole.Clear();
        }

        /// <summary>
        /// Close a tab
        /// </summary>
        /// <param name="tab"></param>
        public void CloseTab(EditorTab tab)
        {
            if (tab.EditorType == EditorType.Game)
                StopGame();
            ((MainWindowViewModel)DataContext!).Tabs.Remove(tab);
        }

        /// <summary>
        /// Run the game in a new tab
        /// </summary>
        /// <param name="dllPath">Path to the dll file</param>
        public void RunGame(string dllPath, string resourcesPath, string texturesPath, string settingsPath)
        {
            var tab = new EditorTab("Game", dllPath, EditorType.Game);
            var tv = tab.Content as TundraView;
            Runner = new Runtime.Runner(dllPath, resourcesPath, texturesPath, settingsPath, window: tv);
            tv!.Game = Runner.Game;
            ((MainWindowViewModel)DataContext!).Tabs.Add(tab);
            FileTabs.SelectedItem = tab;
            GameTab = tab;

            PlayBtn.IsEnabled = false;
            PauseBtn.IsEnabled = true;
            StopBtn.IsEnabled = true;
        }


        public void StopGame()
        {
            if (GameTab == null) return;
            var tv = (GameTab.Content as TundraView)!;

            tv.Stop();

            Runner?.Destroy();
            ((MainWindowViewModel)DataContext!).Tabs.Remove(GameTab);
            GameTab = null;
            PlayBtn.IsEnabled = true;
            PauseBtn.IsEnabled = false;
            StopBtn.IsEnabled = false;
            GC.Collect();
        }
        private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            if (FileTabs.SelectedItem is EditorTab tab)
            {
                tab.Save();
            }
        }
    }
}
