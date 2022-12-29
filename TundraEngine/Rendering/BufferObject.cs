using Silk.NET.OpenGL;

namespace TundraEngine.Rendering
{
    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private uint _handle;
        private BufferTargetARB _bufferType;
        private GL _gl;

        public unsafe BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType, uint length)
        {
            _gl = gl;
            _bufferType = bufferType;

            _handle = _gl.GenBuffer();
            Bind();
            fixed (void* d = data)
            {
                _gl.BufferData(bufferType, (nuint)(length * sizeof(TDataType)), d, BufferUsageARB.DynamicDraw);
            }
        }

        public unsafe void UpdateData(Span<TDataType> data, uint length)
        {
            Bind();
            fixed (void* d = data)
            {
                _gl?.BufferData(_bufferType, (nuint)(length * sizeof(TDataType)), d, BufferUsageARB.DynamicDraw);
            }
        }

        public void Bind()
        {
            _gl.BindBuffer(_bufferType, _handle);
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_handle);
        }
    }
}
