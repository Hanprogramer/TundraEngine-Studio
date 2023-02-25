using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaEdit;
using ReactiveUI;
using System;
using System.IO;
using TundraEngine.Classes.Data;
using TundraEngine.Studio.Controls;
using TundraEngine.Studio.Controls.Editor;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace TundraEngine.Studio.Util
{
    public enum EditorType
    {
        RawText,
        Image,
        Sound,
        Game,
        Object,
        Scene
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
        public Control content;

        public Control Content { get => content; set => this.RaiseAndSetIfChanged(ref content, value);
        }

        private bool isSaved = true;
        public bool IsSaved { get => isSaved; set => this.RaiseAndSetIfChanged(ref isSaved, value); }

        public EditorTab(string header, string filePath)
        {
            Header = header;
            FilePath = filePath;

            if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".bmp"))
                EditorType = EditorType.Image;
            else if (filePath.EndsWith(".mp3") || filePath.EndsWith(".wav") || filePath.EndsWith(".ogg"))
                EditorType = EditorType.Sound;
            else if (filePath.EndsWith(".tobj"))
                EditorType = EditorType.Object;
            else if (filePath.EndsWith(".tscn"))
                EditorType = EditorType.Scene;
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
        /// Save the editor file
        /// </summary>
        public async void Save()
        {
            IsSaved = true;
            switch (EditorType)
            {
                case EditorType.RawText:
                    var te = (TextEditor)Content;
                    var text = te.Document.Text;
                    await File.WriteAllTextAsync(FilePath, text);
                    break;
                
                default:
                    throw new NotImplementedException("This editor type isn't implemented yet");
            }
        }


        /// <summary>
        /// Initialize the content of the tab
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public async void Initialize()
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
            else if (EditorType == EditorType.Object && FilePath != null)
            {
                var obj = await GameObjectResource.Load(FilePath);
                Content = new ObjectEditor(obj);
            }
            else if (EditorType == EditorType.Scene)
            {
                var scene = await SceneResource.Load(FilePath);
                Content = new SceneEditor(scene);
            }
            else
                Content = new Label() { Content = "This editor type isn't supported yet" };
            
        }
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
