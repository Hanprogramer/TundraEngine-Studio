using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static MainWindow? Instance;

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
            fb.FileOpen += FileBrowser_FileOpen;
            DataContext = new MainWindowViewModel();
            Instance = this;

        }

        public void _Log(object o)
        {
            Console.WriteLine(o);
            if (TbConsole != null)
            {
                TbConsole.Text += o.ToString() + "\n";
                TbConsole.CaretIndex = TbConsole.Text.Length;
            }
        }

        public static void Log(object o)
        {
            Instance?._Log(o);
        }

        private void FileBrowser_FileOpen(FileBrowserItem item)
        {
            var tab = new EditorTab(item.FileName, item.Path);
            ((MainWindowViewModel)DataContext!).Tabs.Add(tab);
            FileTabs.SelectedItem = tab;
        }

        public void FileClose(object? s, RoutedEventArgs args)
        {
            EditorTab? el = (args.Source as Button)!.DataContext! as EditorTab;
            if (el != null)
                ((MainWindowViewModel)DataContext!).Tabs.Remove(el);
        }

        public async void OnPlayBtnClicked(object? s, RoutedEventArgs args)
        {
            var result = await GameCompiler.Compile(CurrentProject);
            if (result != null)
            {
                foreach (var d in result)
                {
                    Log(d.Description);
                }
            }
        }

        public static void InitializeEditor(TextEditor _textEditor)
        {
            _textEditor.HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Visible;
            _textEditor.Background = Brushes.Transparent;
            _textEditor.ShowLineNumbers = true;
            _textEditor.ContextMenu = new ContextMenu
            {
                Items = new List<MenuItem>
                {
                    new MenuItem { Header = "Copy", InputGesture = new KeyGesture(Avalonia.Input.Key.C, KeyModifiers.Control) },
                    new MenuItem { Header = "Paste", InputGesture = new KeyGesture(Avalonia.Input.Key.V, KeyModifiers.Control) },
                    new MenuItem { Header = "Cut", InputGesture = new KeyGesture(Avalonia.Input.Key.X, KeyModifiers.Control) }
                }
            };


            //_textEditor.FontFamily = FontFamily.Parse("resm:DefaultNamespace.Fonts.JetbrainsMono-Regular.ttf#JetBrains Mono Regular");

            //_textEditor.TextArea.Background = this.Background;
            //_textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            //_textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            _textEditor.Options.ShowBoxForControlCharacters = true;
            //_textEditor.Options.ColumnRulerPositions = new List<int>() { 80, 100 };
            _textEditor.TextArea.IndentationStrategy = new AvaloniaEdit.Indentation.CSharp.CSharpIndentationStrategy(_textEditor.Options);
            //_textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            _textEditor.TextArea.RightClickMovesCaret = true;

            //Here we initialize RegistryOptions with the theme we want to use.
            var _registryOptions = new TextMateSharp.Grammars.RegistryOptions(ThemeName.DarkPlus);

            //Initial setup of TextMate.
            var _textMateInstallation = _textEditor.InstallTextMate(_registryOptions);

            //Here we are getting the language by the extension and right after that we are initializing grammar with this language.
            //And that's all, you are ready to use AvaloniaEdit with syntax highlighting!
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));

            _textEditor.AddHandler(PointerWheelChangedEvent, (o, i) =>
            {
                if (i.KeyModifiers != KeyModifiers.Control) return;
                if (i.Delta.Y > 0) _textEditor.FontSize++;
                else _textEditor.FontSize = _textEditor.FontSize > 1 ? _textEditor.FontSize - 1 : 1;
                i.Handled = true;
            }, RoutingStrategies.Bubble, true);
            _textEditor.FontFamily = "JetBrains Mono";
        }
    }

    public enum EditorType
    {
        RawText,
        Image,
        Sound
    }

    public class EditorTab
    {
        public string Header { get; set; }
        public string FilePath { get; set; }

        public EditorType EditorType { get; set; }

        public Control Content { get; set; }

        public EditorTab(string header, string filePath)
        {
            Header = header;
            FilePath = filePath;

            if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".bmp"))
            {
                EditorType = EditorType.Image;
                var im = new Image();
                im.Source = new Bitmap(filePath);
                Content = im;

            }
            else if (filePath.EndsWith(".mp3") || filePath.EndsWith(".wav") || filePath.EndsWith(".ogg"))
            {
                EditorType = EditorType.Sound;
            }
            else
            {
                EditorType = EditorType.RawText;
                var te = new TextEditor();
                MainWindow.InitializeEditor(te);
                te.Text = File.ReadAllText(filePath);
                Content = te;
            }


        }
    }
}
