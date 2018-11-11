using System.Numerics;

namespace XRasterizer.Base
{
    internal struct Fragment
    {
        public Vertex Vert;
        public int X;
        public int Y;
        
        public static Fragment Lerp(Fragment Min, Fragment Max, double T, int Y)
        {
            var Frag = new Fragment();
            Frag.Vert = Vertex.Lerp(Min.Vert, Max.Vert, (float)T);
            //Frag.X = Mathf.Lerp(Min.X, Max.X, T);
            //Frag.Y = Mathf.Lerp(Min.Y, Max.Y, T);
            Frag.X = (int)((double)Min.X + (double)(Max.X - Min.X) * T + 0.5d);
            Frag.Y = Y;
            return Frag;
        }
    }
}