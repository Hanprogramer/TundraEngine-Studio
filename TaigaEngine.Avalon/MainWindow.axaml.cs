using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TextMateSharp.Grammars;
using TundraEngine.Classes;
using TundraEngine.Studio.Compiler;
using TundraEngine.Studio.Controls;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio
{
    public partial class MainWindow : Window
    {
        public string TestString = "TEST";
        public string ProjectPath = "D:\\Programming\\C#\\TaigaEngine.Avalon\\TestGame1\\project.json";
        public static FontFamily CodeFamily = FontFamily.Parse("avares://TundraEngine.Studio/Assets/JetBrainsMono-Regular.ttf#JetBrains Mono");
        private Runner.Runner? Runner;


        public EditorTab? GameTab = null;

        public TundraProject CurrentProject;
        public MainWindow()
        {
            InitializeComponent();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //TODO: do this on application level maybe? Or for every window
                Win32Native.ImplementDarkTitleBar(this);
            CurrentProject = TundraProject.Parse(ProjectPath);

            var fb = this.FindControl<FileBrowser>("FileBrowser");
            fb.CurrentWorkingDirectory = CurrentProject.Path;
            fb.FileOpen += OnFileOpen;
            DataContext = new MainWindowViewModel();
        }

        /// <summary>
        /// Run the game in a new tab
        /// </summary>
        /// <param name="dllPath">Path to the dll file</param>
        public void RunGame(string dllPath)
        {
            var tab = new EditorTab("Game", dllPath, EditorType.Game);
            var tv = tab.Content as TundraView;
            Runner = new Runner.Runner(dllPath, window: tv);
            tv!.Game = Runner.Game;
            ((MainWindowViewModel)DataContext!).Tabs.Add(tab);
            FileTabs.SelectedItem = tab;
            GameTab = tab;

            PlayBtn.IsEnabled = false;
            PauseBtn.IsEnabled = true;
            StopBtn.IsEnabled = true;
        }

        // When stop button pressed
        public void OnStopGame(object? sender, RoutedEventArgs e)
        {
            StopGame();
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

        // Internal logger function
        public void Log(object o)
        {
            Console.WriteLine(o);
            if (TbConsole != null)
            {
                TbConsole.Text += o.ToString() + "\n";
                TbConsole.CaretIndex = TbConsole.Text.Length;
            }
        }

        // Event handler for file open
        private void OnFileOpen(FileBrowserItem item)
        {
            var tab = new EditorTab(item.FileName, item.Path);
            ((MainWindowViewModel)DataContext!).Tabs.Add(tab);
            FileTabs.SelectedItem = tab;
        }

        // Event handler for file close
        public void OnFileClose(object? s, RoutedEventArgs args)
        {
            EditorTab? el = (args.Source as Button)!.DataContext! as EditorTab;
            if (el != null)
            {
                if (el.EditorType == EditorType.Game)
                    OnStopGame(null, args);
                ((MainWindowViewModel)DataContext!).Tabs.Remove(el);
            }
        }

        // When the play button is clicked
        public async void OnPlayBtnClicked(object? s, RoutedEventArgs args)
        {
            PlayBtn.IsEnabled = false;
            PauseBtn.IsEnabled = false;
            StopBtn.IsEnabled = false;
            ClearConsole();
            var result = await GameCompiler.Compile(CurrentProject, Log);
            if (result != null)
            {
                foreach (var d in result.Items)
                {
                    Log(d.Title);
                }
                if (result.Success)
                    RunGame(result.DllPath);
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

        public void ClearConsole() {
            TbConsole.Clear();
        }
    }
}
