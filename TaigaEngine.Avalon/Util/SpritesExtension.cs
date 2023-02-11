using Avalonia.Media.Imaging;
using HarfBuzzSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TundraEngine.Classes.Data;

namespace TundraEngine.Studio.Util
{
    internal static class SpritesExtension
    {
        public static Bitmap GetAvaloniaBitmap(this SpriteResource res)
        {
            var filepath = Path.Join(TundraStudio.CurrentProject.Path, res.content);

            // Load using ImageSharp
            //var image = Image.Load(filepath);
            //Bitmap bitmap;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    image.Save(ms, PngFormat.Instance);
            //    bitmap = new Avalonia.Media.Imaging.Bitmap(ms);
            //}
            //return bitmap;

            // Load using avalonia
            var bitmap = new Bitmap(filepath);
            return bitmap;

            throw new Exception($"Can't create bitmap for sprite {res.path ?? res.uuid}");
        }
    }
}
