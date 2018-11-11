using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace XRasterizer.Base
{
    public class Texture2D
    {
        public readonly int Width;
        public readonly int Height;
        private readonly Colorf[][] Pixels_;
        private readonly Vector<double>[] PixelsVec_;
        private readonly unsafe double* PixelsVecPointer_;
        private readonly float TexWidth_;
        private readonly float TexHeight_;
        private TextureWarpMode WarpMode_;
        private Colorf BorderColor_;
        private TextureFilterMode FilterMode_;

        public Texture2D(int Width, int Height, byte[] Data)
        {
            this.Width = Width;
            this.Height = Height;
            this.TexWidth_ = Width - 1;
            this.TexHeight_ = Height - 1;
            
            Pixels_ = new Colorf[Height][];
            PixelsVec_ = new Vector<double>[Width * Height];
            for (var Y = 0; Y < Height; ++Y)
            {
                Pixels_[Y] = new Colorf[Width];
                for (var X = 0; X < Width; ++X)
                {
                    var Index = (Y * Width + X) << 2;
                    Pixels_[Y][X] = new Colorf(Data[Index + 2] / 255.0f, Data[Index + 1] / 255.0f, Data[Index + 0] / 255.0f, Data[Index + 3] / 255.0f);
                    PixelsVec_[Index >> 2] = new Vector<double>(new double[]{Data[Index + 2] / 255.0d, Data[Index + 1] / 255.0d, Data[Index + 0] / 255.0d, Data[Index + 3] / 255.0d});
                }
            }

            unsafe
            {
                PixelsVecPointer_ = (double*)Unsafe.AsPointer(ref PixelsVec_[0]);
            }

            WarpMode_ = TextureWarpMode.Repeat;
            BorderColor_ = Colorf.Black;
            FilterMode_ = TextureFilterMode.Linear;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Colorf Sample(Vector2 TexCoords)
        {
            var U = (double)TexCoords.X;
            var V = (double)TexCoords.Y;

            if (WarpMode_ == TextureWarpMode.Repeat)
            {
                if (U > 1)
                {
                    U = U - (int) U;
                }
                else if (U < 0)
                {
                    U = U - (int) U + 1;
                }

                if (V > 1)
                {
                    V = V - (int) V;
                }
                else if (V < 0)
                {
                    V = V - (int) V + 1;
                }
            }
            else if (WarpMode_ == TextureWarpMode.Clamp)
            {
                U = Mathf.Clamp01(U);
                V = Mathf.Clamp01(V);
            }
            else if (WarpMode_ == TextureWarpMode.Border)
            {
                if (U < 0 || U > 1 || V < 0 || V > 1)
                {
                    return BorderColor_;
                }
            }

            U = U * TexWidth_;
            V = V * TexHeight_;
            if (FilterMode_ == TextureFilterMode.Point)
            {
                return Pixels_[(int)V][(int)U];
            }
            else
            {
                U = U - 0.5f;
                V = V - 0.5f;
                var X = (int)U;
                var Y = (int)V;
                var u_ratio = U - X;
                var v_ratio = V - Y;
                var u_opposite = 1 - u_ratio;
                var v_opposite = 1 - v_ratio;
                /*var result = (Pixels_[Y][X] * u_opposite + Pixels_[Y][X + 1] * u_ratio) * v_opposite +
                                (Pixels_[Y + 1][X] * u_opposite + Pixels_[Y + 1][X + 1] * u_ratio) * v_ratio;*/
                var result = Pixels_[Y][X] * (u_opposite * v_opposite) + Pixels_[Y][X + 1] * (u_ratio * v_opposite) +
                             Pixels_[Y + 1][X] * (u_opposite * v_ratio) + Pixels_[Y + 1][X + 1] * (u_ratio * v_ratio);
                
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void* SampleVec(Vector2 TexCoords)
        {
            var U = TexCoords.X;
            var V = TexCoords.Y;

            if (WarpMode_ == TextureWarpMode.Repeat)
            {
                if (U > 1)
                {
                    U = U - (int)U;
                }
                else if (U < 0)
                {
                    U = U - (int)U + 1;
                }

                if (V > 1)
                {
                    V = V - (int)V;
                }
                else if (V < 0)
                {
                    V = V - (int)V + 1;
                }

            }
            else if (WarpMode_ == TextureWarpMode.Clamp)
            {
                U = Mathf.Clamp01(U);
                V = Mathf.Clamp01(V);
            }
            else if (WarpMode_ == TextureWarpMode.Border)
            {
                if (U < 0 || U > 1 || V < 0 || V > 1)
                {
                    return null;
                }
            }

            U = U * TexWidth_;
            V = V * TexHeight_;
            if (FilterMode_ == TextureFilterMode.Point)
            {
                var Index = ((int)V * Width + (int) U) * Vector<double>.Count;
                return PixelsVecPointer_ + Index;
            }
            else
            {
                U = U - 0.5f;
                V = V - 0.5f;
                var X = (int)U;
                var Y = (int)V;
                var u_ratio = U - X;
                var v_ratio = V - Y;
                var u_opposite = 1 - u_ratio;
                var v_opposite = 1 - v_ratio;

                var result = PixelsVec_[Y * Width + X] * (u_opposite * v_opposite) + PixelsVec_[Y * Width + X + 1] * (u_ratio * v_opposite) +
                             PixelsVec_[(Y + 1) * Width + X] * (u_opposite * v_ratio) + PixelsVec_[(Y + 1) * Width + (X + 1)] * (u_ratio * v_ratio);
                
                return Unsafe.AsPointer(ref result);
            }
        }

        public void SetWarpMode(TextureWarpMode WarpMode)
        {
            WarpMode_ = WarpMode;
        }

        public void SetBorderColor(Colorf BorderColor)
        {
            BorderColor_ = BorderColor;
        }

        public void SetFilterMode(TextureFilterMode FilterMode)
        {
            FilterMode_ = FilterMode;
        }
    }
}