using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaEdit;
using ReactiveUI;
using System;
using System.IO;
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

    public class EditorTab : ReactiveObject
    {
        string header = "";
        public string Header
        {
            get => header; 
            set
            {
                this.RaiseAndSetIfChanged(ref header, value);
            }
        }
        public string? FilePath { get; set; }

        public EditorType EditorType { get; set; }

        public Control Content { get; set; }
        public bool IsSaved { get; set; } = true;

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
        /// Initialize the content of the tab
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Initialize()
        {
            if (EditorType == EditorType.Image)
            {
                var im = new Image();
                var bitmap = new Bitmap(FilePath);
                RenderOptions.SetBitmapInterpolationMode(im, Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.Default);
                im.Source = bitmap;

                var container = new Panel();
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var checkerboard = new Bitmap(assets.Open(new Uri("avares://TundraEngine.Studio/Assets/checkerboard_dark.png")));

                container.Background = new ImageBrush(checkerboard) { TileMode = TileMode.Tile };
                container.Children.Add(im);

                Content = container;
            }
            else if (EditorType == EditorType.Sound)
            {
                throw new NotImplementedException();
            }
            else if (EditorType == EditorType.RawText)
            {
                var te = new TextEditor();
                CodeEditor.InitializeTextEditor(te);
                if (FilePath != null)
                    te.Text = File.ReadAllText(FilePath);
                Content = te;
                te.TextChanged += (o, e) =>
                {
                    IsSaved = false;
                };
            }
            else if (EditorType == EditorType.Game)
            {
                var tv = new TundraView();
                Content = tv;
            }
            else
            {
                Content = new Label() { Content = "This editor type isn't supported yet" };
            }
        }
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
