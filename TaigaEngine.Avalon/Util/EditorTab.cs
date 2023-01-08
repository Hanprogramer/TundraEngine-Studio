using Avalonia.Controls;
using Avalonia.Media.Imaging;
using AvaloniaEdit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (FilePath == null) throw new NotImplementedException("Empty editor can't be created yet");
                var te = new TextEditor();
                MainWindow.InitializeEditor(te);
                te.Text = File.ReadAllText(FilePath);
                Content = te;
            }
            else if (EditorType == EditorType.Game)
            {
                var tv = new TundraView();
                Content = tv;
            }
        }
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
