using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using Microsoft.CodeAnalysis.Differencing;
using System.Collections.Generic;
using TextMateSharp.Grammars;

namespace TundraEngine.Studio.Controls
{
    public partial class CodeEditor : Grid
    {
        public TextEditor Editor { get; set; }
        public CodeEditor()
        {
            Editor = new TextEditor();
            Children.Add(Editor);
            InitializeTextEditor(Editor);
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
    }
}
