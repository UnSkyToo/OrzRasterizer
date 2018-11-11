using System.Runtime.CompilerServices;

namespace XRasterizer.Base
{
    public struct Colorf
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Colorf(float R, float G, float B, float A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public Colorf(float R, float G, float B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = 1.0f;
        }

        public override int GetHashCode()
        {
            return R.GetHashCode() ^ G.GetHashCode() << 2 ^ B.GetHashCode() >> 2 ^ A.GetHashCode() >> 1;
        }

        public override bool Equals(object Other)
        {
            if (!(Other is Colorf))
            {
                return false;
            }

            return Equals((Colorf)Other);
        }

        public bool Equals(Colorf Other)
        {
            return R.Equals(Other.R) && G.Equals(Other.G) && B.Equals(Other.B) && A.Equals(Other.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf operator +(Colorf Left, Colorf Right)
        {
            return new Colorf(Left.R + Right.R, Left.G + Right.G, Left.B + Right.B, Left.A + Right.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf operator -(Colorf Left, Colorf Right)
        {
            return new Colorf(Left.R - Right.R, Left.G - Right.G, Left.B - Right.B, Left.A - Right.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf operator *(Colorf Left, Colorf Right)
        {
            return new Colorf(Left.R * Right.R, Left.G * Right.G, Left.B * Right.B, Left.A * Right.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf operator *(Colorf Left, float Right)
        {
            return new Colorf(Left.R * Right, Left.G * Right, Left.B * Right, Left.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf operator *(float Left, Colorf Right)
        {
            return new Colorf(Right.R * Left, Right.G * Left, Right.B * Left, Right.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf operator *(Colorf Left, double Right)
        {
            return new Colorf((float)(Left.R * Right), (float)(Left.G * Right), (float)(Left.B * Right), Left.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf operator *(double Left, Colorf Right)
        {
            return new Colorf((float)(Right.R * Left), (float)(Right.G * Left), (float)(Right.B * Left), Right.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf operator /(Colorf Left, float Right)
        {
            return new Colorf(Left.R / Right, Left.G / Right, Left.B / Right, Left.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Colorf Left, Colorf Right)
        {
            var R = (double)(Left.R - Right.R);
            var G = (double)(Left.G - Right.G);
            var B = (double)(Left.B - Right.B);
            var A = (double)(Left.A - Right.A);
            return R * R + G * G + B * B + A * A < 9.99999943962493E-11;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Colorf Left, Colorf Right)
        {
            return !(Left == Right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf Lerp(Colorf Min, Colorf Max, float T)
        {
            T = Mathf.Clamp01(T);
            return new Colorf(Min.R + (Max.R - Min.R) * T, Min.G + (Max.G - Min.G) * T, Min.B + (Max.B - Min.B) * T, Min.A + (Max.A - Min.A) * T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Colorf LerpUnclamped(Colorf Min, Colorf Max, float T)
        {
            return new Colorf(Min.R + (Max.R - Min.R) * T, Min.G + (Max.G - Min.G) * T, Min.B + (Max.B - Min.B) * T, Min.A + (Max.A - Min.A) * T);
        }

        public static readonly Colorf Black = new Colorf(0, 0, 0);
        public static readonly Colorf White = new Colorf(1, 1, 1);
        public static readonly Colorf Gray = new Colorf(0.5f, 0.5f, 0.5f);
        public static readonly Colorf Red = new Colorf(1.0f, 0, 0);
        public static readonly Colorf Green = new Colorf(0, 1.0f, 0);
        public static readonly Colorf Blue = new Colorf(0, 0, 1.0f);
    }
}