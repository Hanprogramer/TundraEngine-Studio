using Silk.NET.Core;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace TundraEngine.Rendering
{
    public class Image
    {
        public Image() { }
        public unsafe static RawImage Load(string filename)
        {
            using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(filename);
            var bytes = new byte[image.Width * image.Height * sizeof(Rgba32)];
            image.ProcessPixelRows
            (
                a =>
                {
                    for (var y = 0; y < a.Height; y++)
                    {
                        MemoryMarshal.Cast<Rgba32, byte>(a.GetRowSpan(y)).CopyTo(bytes.AsSpan().Slice((y * a.Width * sizeof(Rgba32))));
                    }
                }
            );
            return new RawImage(image.Width, image.Height, bytes);
        }
    }
}
