using System.Runtime.CompilerServices;

namespace XRasterizer.Base
{
    public static class Mathf
    {
        public static readonly float Epsilon = 1.175494E-38f;
        public static readonly double Deg2Rad = System.Math.PI / 180.0d;
        public static readonly double Rad2Deg = 180.0d / System.Math.PI;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int Value)
        {
            return System.Math.Abs(Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float Value)
        {
            return System.Math.Abs(Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float Value)
        {
            return (float)System.Math.Sin(Value * Deg2Rad);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float Value)
        {
            return (float)System.Math.Cos(Value * Deg2Rad);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Pow(int X, int Y)
        {
            return (int)System.Math.Pow(X, Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(float X, float Y)
        {
            return (float)System.Math.Pow(X, Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Pow(double X, double Y)
        {
            return System.Math.Pow(X, Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref int Left, ref int Right)
        {
            Left = Left + Right;
            Right = Left - Right;
            Left = Left - Right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref float Left, ref float Right)
        {
            Left = Left + Right;
            Right = Left - Right;
            Left = Left - Right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int Left, int Right)
        {
            return Left >= Right ? Right : Left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float Left, float Right)
        {
            return (double)Left >= (double)Right ? Right : Left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Min(double Left, double Right)
        {
            return Left >= Right ? Right : Left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int Left, int Right)
        {
            return Left <= Right ? Right : Left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float Left, float Right)
        {
            return (double)Left <= (double)Right ? Right : Left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Max(double Left, double Right)
        {
            return Left <= Right ? Right : Left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float Left, float Right)
        {
            return (double)Abs(Right - Left) < (double)Max(1E-06f * Max(Abs(Left), Abs(Right)), Epsilon * 8.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int Value, int Min, int Max)
        {
            if (Value < Min)
            {
                return Min;
            }

            if (Value > Max)
            {
                return Max;
            }

            return Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float Value, float Min, float Max)
        {
            if (Value < Min)
            {
                return Min;
            }

            if (Value > Max)
            {
                return Max;
            }

            return Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double Value, double Min, double Max)
        {
            if (Value < Min)
            {
                return Min;
            }

            if (Value > Max)
            {
                return Max;
            }

            return Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp01(int Value)
        {
            if (Value < 0)
            {
                return 0;
            }

            if (Value > 1)
            {
                return 1;
            }

            return Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float Value)
        {
            if (Value < 0.0f)
            {
                return 0.0f;
            }

            if (Value > 1.0f)
            {
                return 1.0f;
            }

            return Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp01(double Value)
        {
            if (Value < 0.0d)
            {
                return 0.0d;
            }

            if (Value > 1.0d)
            {
                return 1.0d;
            }

            return Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Lerp(int Min, int Max, float T)
        {
            return (int)(Min + (Max - Min) * (double)T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float Min, float Max, float T)
        {
            return Min + (Max - Min) * T;
        }
    }
}