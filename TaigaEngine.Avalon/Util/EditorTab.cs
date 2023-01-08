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
using TextMateSharp.Grammars;
using TundraEngine.Studio.Controls;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace TundraEngine.Studio.Util
{
    public enum EditorType
    {
        RawText,
        Image,
        Sound,
        Game
    }

    public class EditorTab
    {
        public string Header { get; set; }
        public string? FilePath { get; set; }

        public EditorType EditorType { get; set; }

        public Control Content { get; set; }

        public EditorTab(string header, string filePath)
        {
            Header = header;
            FilePath = filePath;

            if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".bmp"))
                EditorType = EditorType.Image;
            else if (filePath.EndsWith(".mp3") || filePath.EndsWith(".wav") || filePath.EndsWith(".ogg"))
                EditorType = EditorType.Sound;
            else
                EditorType = EditorType.RawText;

            Initialize();
        }

        public EditorTab(string header, string? filePath = null, EditorType editorType = EditorType.RawText)
        {
            Header = header;
            EditorType = editorType;
            FilePath = filePath;
            Initialize();
        }

        /// <summary>
        /// Initialize a text editor with context, language and theme
        /// </summary>
        /// <param name="_textEditor"></param>
        public static void InitializeTextEditor(TextEditor _textEditor)
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

            _textEditor.AddHandler(InputElement.PointerWheelChangedEvent, (o, i) =>
            {
                if (i.KeyModifiers != KeyModifiers.Control) return;
                if (i.Delta.Y > 0) _textEditor.FontSize++;
                else _textEditor.FontSize = _textEditor.FontSize > 1 ? _textEditor.FontSize - 1 : 1;
                i.Handled = true;
            }, RoutingStrategies.Bubble, true);
            _textEditor.FontFamily = "JetBrains Mono";
        }

        /// <summary>
        /// Initialize the content of the tab
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Initialize()
        {
            if (EditorType == EditorType.Image)
            {
                var im = new Image();
                im.Source = new Bitmap(FilePath);
                Content = im;
            }
            else if (EditorType == EditorType.Sound)
            {
                throw new NotImplementedException();
            }
            else if (EditorType == EditorType.RawText)
            {
                var te = new TextEditor();
                InitializeTextEditor(te);
                if(FilePath != null)
                    te.Text = File.ReadAllText(FilePath);
                Content = te;
            }
            else if (EditorType == EditorType.Game)
            {
                var tv = new TundraView();
                Content = tv;
            }
            else
            {
                Content = new Label() { Content="This editor type isn't supported yet" };
            }
        }
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
