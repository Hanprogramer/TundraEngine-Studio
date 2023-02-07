using Silk.NET.OpenGL;
using SixLabors.ImageSharp.PixelFormats;
using PixelFormat = Silk.NET.OpenGL.PixelFormat;
using PixelType = Silk.NET.OpenGL.PixelType;

namespace TundraEngine.Rendering
{
    public struct TextureData
    {
        public string Content;
        public int Width;
        public int Height;
    }
    public class Texture : IDisposable
    {
        private uint _handle;
        private Renderer _renderer;
        private GL _gl { get => _renderer.Gl; }
        private RendererFilter filter;

        public bool IsLoaded = false;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string? Path { get; private set; }
        public byte[]? Bytes { get; private set; }

        public Texture(string path)
        {
            Path = path;
        }
        public Texture(byte[] bytes, int width, int height)
        {
            Bytes = bytes;
            Width = width;
            Height = height;
        }
        public unsafe void Load(Renderer renderer)
        {
            if (Path == null)
            {
                if (Bytes != null)
                {
                    Load(renderer, Bytes);
                    return;
                }
                else
                {
                    throw new Exception("Error: texture doesn't have filepath or bytes data! Can't load");
                }
            }
            _renderer = renderer;
            //_gl = renderer.Gl;
            filter = renderer.RendererFilter;

            _handle = _gl.GenTexture();
            Bind();
            //Loading an image using imagesharp.
            using (var img = SixLabors.ImageSharp.Image.Load<Rgba32>(Path))
            {
                Width = img.Width;
                Height = img.Height;
                //Reserve enough memory from the gpu for the whole image
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

                img.ProcessPixelRows(accessor =>
                {
                    //ImageSharp 2 does not store images in contiguous memory by default, so we must send the image row by row
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        fixed (void* data = accessor.GetRowSpan(y))
                        {
                            //Loading the actual image.
                            _gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    }
                });
            }
            IsLoaded = true;

            SetParameters();

        }

        public unsafe void Load(Renderer renderer, Span<byte> data)
        {
            //Saving the gl instance.
            _renderer = renderer;
            //_gl = renderer.Gl;
            filter = renderer.RendererFilter;

            //Generating the opengl handle;
            _handle = _gl.GenTexture();
            Bind();
            using (var img = SixLabors.ImageSharp.Image.Load<Rgba32>(data))
            {
                Width = img.Width;
                Height = img.Height;
                //Reserve enough memory from the gpu for the whole image
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

                img.ProcessPixelRows(accessor =>
                {
                    //ImageSharp 2 does not store images in contiguous memory by default, so we must send the image row by row
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        fixed (void* data = accessor.GetRowSpan(y))
                        {
                            //Loading the actual image.
                            _gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    }
                });
            }
            IsLoaded = true;

            SetParameters();
            //We want the ability to create a texture using data generated from code aswell.
            //fixed (void* d = &data[0])
            //{
            //    //Setting the data of a texture.
            //    _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, (uint)Width, (uint)Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
            //    SetParameters();
            //}
            IsLoaded = true;
        }

        private void SetParameters()
        {
            //Setting some texture perameters so the texture behaves as expected.
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);

            if (filter == RendererFilter.Linear)
            {
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            }
            else
            {
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.NearestMipmapNearest);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
            }

            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
            //Generating mipmaps.
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            if (_gl == null) throw new Exception("Texture hasn't been loaded yet");
            //When we bind a texture we can choose which textureslot we can bind it to.
            _gl.ActiveTexture(textureSlot);
            _gl.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public void Unbind()
        {
            _gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            //In order to dispose we need to delete the opengl handle for the texure.
            _gl.DeleteTexture(_handle);
        }
    }
}
