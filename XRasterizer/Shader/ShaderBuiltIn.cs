using System.Numerics;
using System.Runtime.CompilerServices;
using XRasterizer.Base;

namespace XRasterizer.Shader
{
    public class ShaderBuiltIn
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Colorf tex2D(Texture2D Sample2D, Vector2 TexCoords)
        {
            return Sample2D?.Sample(TexCoords) ?? Colorf.Black;
        }

        private static Vector<double> tex2DVecNull = Vector<double>.Zero;
        private static readonly unsafe void* tex2DVecNullPtr = Unsafe.AsPointer(ref tex2DVecNull);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void* tex2DVec(Texture2D Sample2D, Vector2 TexCoords)
        {
            if (Sample2D == null)
            {
                return tex2DVecNullPtr;
            }
            return Sample2D.SampleVec(TexCoords);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Vector3 normalize(Vector3 Normal)
        {
            return Vector3.Normalize(Normal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float max(float A, float B)
        {
            return Mathf.Max(A, B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float dot(Vector3 A, Vector3 B)
        {
            return Vector3.Dot(A, B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float sin(float Value)
        {
            return Mathf.Sin(Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float abs(float Value)
        {
            return Mathf.Abs(Value);
        }
    }
}