using System.Numerics;
using System.Runtime.CompilerServices;

namespace XRasterizer.Base
{
    public struct Light
    {
        public LightType Type;
        public bool Enabled;

        public Colorf Ambient;
        public Colorf Diffuse;
        public Colorf Specular;

        public Vector3 Position;
        public Vector3 Direction;

        public float KC;
        public float KL;
        public float KQ;

        public float Inner;
        public float Outer;
        public float PF;

        public Vector<double>[] Vec;

        public unsafe double* AmbientVecPtr;
        public unsafe double* DiffuseVecPtr;
        public unsafe double* SpecularVecPtr;


        public unsafe void UpdateSIMD()
        {
            if (Vec == null)
            {
                Vec = new Vector<double>[3];
                Vec[0] = new Vector<double>(new double[] {Ambient.R, Ambient.G, Ambient.B, Ambient.A});
                Vec[1] = new Vector<double>(new double[] {Diffuse.R, Diffuse.G, Diffuse.B, Diffuse.A});
                Vec[2] = new Vector<double>(new double[] {Specular.R, Specular.G, Specular.B, Specular.A});
            }

            AmbientVecPtr = (double*)Unsafe.AsPointer(ref Vec[0]);
            DiffuseVecPtr = AmbientVecPtr + Vector<double>.Count;
            SpecularVecPtr = DiffuseVecPtr + Vector<double>.Count;
        }
    }
}