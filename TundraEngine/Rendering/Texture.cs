using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TundraEngine.Rendering
{
    public class Texture : IDisposable
    {
        private uint _handle;
        private GL _gl;
        private RendererFilter filter;
        public string Path { get; private set; } = "";

        public Texture(string path) {
            Path = path;
        }
        public unsafe void Load(Renderer renderer)
        {
            _gl = renderer.Gl;
            filter = renderer.RendererFilter;

            _handle = _gl.GenTexture();
            Bind();
            //Loading an image using imagesharp.
            using (var img = SixLabors.ImageSharp.Image.Load<Rgba32>(Path))
            {
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

            SetParameters();

        }

        public unsafe void Load(Renderer renderer, Span<byte> data, uint width, uint height)
        {
            //Saving the gl instance.
            _gl = renderer.Gl;
            filter = renderer.RendererFilter;

            //Generating the opengl handle;
            _handle = _gl.GenTexture();
            Bind();

            //We want the ability to create a texture using data generated from code aswell.
            fixed (void* d = &data[0])
            {
                //Setting the data of a texture.
                _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
                SetParameters();
            }
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
            //When we bind a texture we can choose which textureslot we can bind it to.
            _gl.ActiveTexture(textureSlot);
            _gl.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public void Unbind() {
            _gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            //In order to dispose we need to delete the opengl handle for the texure.
            _gl.DeleteTexture(_handle);
        }
    }
}
