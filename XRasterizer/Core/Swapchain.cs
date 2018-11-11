using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace XRasterizer.Core
{
    public class Swapchain
    {
        public Bitmap Current => Surface_[CurrentIndex_];
        public Bitmap Background => Surface_[1 - CurrentIndex_];
        
        private readonly Graphics Device_;
        private readonly Bitmap[] Surface_;
        private readonly Rectangle Bounds_;
        private int CurrentIndex_;
        private BitmapData BitmapData_;

        public Swapchain(IntPtr Handle, int Width, int Height)
        {
            Device_ = Graphics.FromHwnd(Handle);
            Device_.SmoothingMode = SmoothingMode.AntiAlias;

            Bounds_ = new Rectangle(0, 0, Width, Height);
            Surface_ = new Bitmap[2];
            Surface_[0] = new Bitmap(Width, Height);
            Surface_[1] = new Bitmap(Width, Height);
            CurrentIndex_ = 0;
        }

        public void SwapBuffers()
        {
            CurrentIndex_ = 1 - CurrentIndex_;
            Device_.DrawImage(Current, Point.Empty);
        }

        public unsafe void Begin(GraphicsDevice Device)
        {
            BitmapData_ = Background.LockBits(Bounds_, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Device.GetContext().ColorBuffer = (byte*)BitmapData_.Scan0;
        }

        public void End()
        {
            Background.UnlockBits(BitmapData_);
        }
    }
}