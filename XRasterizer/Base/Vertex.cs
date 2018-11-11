using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using XRasterizer.Core;

namespace XRasterizer.Base
{
    [StructLayout(LayoutKind.Explicit, Size = 36)]
    internal struct Vertex
    {
        [FieldOffset(0)]
        public Vector3 Normal;
        [FieldOffset(12)]
        public Vector2 TexCoords;
        [FieldOffset(20)]
        public Vector3 Position3;
        [FieldOffset(32)]
        public float W;
        [FieldOffset(20)]
        public Vector4 Position;
        [FieldOffset(0)]
        public Vector<float> Vec;
        
        public Vertex(Vector4 Position, Vector2 TexCoords, Vector3 Normal)
        {
            this.Vec = Vector<float>.Zero;
            this.Position3 = Vector3.Zero;
            this.Position = Position;
            this.W = Position.W;
            this.TexCoords = TexCoords;
            this.Normal = Normal;
        }

        public Vertex(Vector3 Position, Vector2 TexCoords, Vector3 Normal)
        {
            this.Vec = Vector<float>.Zero;
            this.Position = Vector4.Zero;
            this.Position3 = Position;
            this.W = 1;
            this.TexCoords = TexCoords;
            this.Normal = Normal;
        }

        public Vertex(Vector<float> Vec, float W)
        {
            this.Position = Vector4.Zero;
            this.Position3 = Vector3.Zero;
            this.TexCoords = Vector2.Zero;
            this.Normal = Vector3.Zero;
            this.W = W;
            this.Vec = Vec;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vertex Lerp(Vertex Min, Vertex Max, float T)
        {
            if (GraphicsContext.EnableSIMD)
            {
                var Vec = Min.Vec + (Max.Vec - Min.Vec) * T;
                return new Vertex(Vec, Mathf.Lerp(Min.W, Max.W, T));
            }

            var Vert = new Vertex
            {
                Position = Vector4.Lerp(Min.Position, Max.Position, T),
                Normal = Vector3.Lerp(Min.Normal, Max.Normal, T),
                TexCoords = Vector2.Lerp(Min.TexCoords, Max.TexCoords, T),
            };

            return Vert;
        }
    }
}