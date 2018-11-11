using System;

namespace XRasterizer
{
    public static class RasterizerEngine
    {
        public static GraphicsDevice CreateDevice(IntPtr Handle, int Width, int Height)
        {
            return new GraphicsDevice(Handle, Width, Height);
        }

        public static void DestroyDevice(GraphicsDevice Device)
        {
            Device.Destroy();
        }
    }
}