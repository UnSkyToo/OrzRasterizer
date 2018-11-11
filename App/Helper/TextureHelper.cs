using XRasterizer;
using XRasterizer.Base;

namespace App.Helper
{
    public static class TextureHelper
    {
        public static Texture2D Load(string FilePath)
        {
            var FullPath = FileHelper.GetFullPath(FilePath);
            var OriginImage = new System.Drawing.Bitmap(FullPath);
            var Buffer = new byte[OriginImage.Width * OriginImage.Height * 4];
            var BitsData = OriginImage.LockBits(
                new System.Drawing.Rectangle(0, 0, OriginImage.Width, OriginImage.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(BitsData.Scan0, Buffer, 0, Buffer.Length);
            OriginImage.UnlockBits(BitsData);

            var Tex = new Texture2D(OriginImage.Width, OriginImage.Height, Buffer);
            Tex.SetFilterMode(TextureFilterMode.Point);
            Tex.SetWarpMode(TextureWarpMode.Repeat);
            Tex.SetBorderColor(Colorf.Black);

            return Tex;
        }
    }
}