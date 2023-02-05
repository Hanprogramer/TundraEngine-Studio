using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Linq;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio.Dialogs
{
    public partial class CreateFileDialog : Window
    {
        public string FilePath { get; set; } = "";

        public CreateFileDialog()
        {
            InitializeComponent();
        }

        public void OnCreateFile(string path, string filename)
        {
            var root = TundraStudio.CurrentProject!.Path;
            var folder = Path.Join(root, path);
            var filePath = Path.Join(folder, filename);

            if (!Directory.Exists(folder))
            {
                MessageBox.Show("Can't create file", $"Can't find directory: {folder}", this);
                return;
            }
            
            if (File.Exists(filePath))
                MessageBox.Show("File already exists", $"File {filename} already exists", this);
            else
            {
                File.Create(filePath);
                Close(true);
            }
        }

        public void OnClickCreate(object? sender, RoutedEventArgs e)
        {
            var fileName = TbFileName.Text ?? "";
            var filePath = TbFileLoc.Text ?? "/";
            if (CheckValid(fileName))
            {
                if (CheckValid(filePath))
                {
                    OnCreateFile(filePath, fileName);
                }
            }
        }

        public bool CheckValid(string pathString)
        {
            if (pathString == "")
            {
                MessageBox.Show("Error", "All parameters can't be empty", this);
                return false;
            }
            var index = ValidateFileNameOrPath(pathString);
            if (index > -1)
            {
                MessageBox.Show("Error", $"Invalid character(s) found: {pathString.ElementAt(index)}", this);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Check if path contains any invalid chars
        /// </summary>
        /// <param name="pathString"></param>
        /// <returns>the index of invalid char, if not found -1</returns>
        public int ValidateFileNameOrPath(string pathString)
        {
            return pathString.IndexOfAny(Path.GetInvalidPathChars()); // -1 means valid
        }
    }
}
