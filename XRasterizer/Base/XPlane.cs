using System.Numerics;

namespace XRasterizer.Base
{
    /*internal struct XPlane
    {
        private readonly Vector4 Normal_;

        public XPlane(Vector3 Normal, float Distance)
        {
            Normal_ = Vector4.Normalize(new Vector4(Normal, 0));
            Normal_.W = Distance;
        }

        public int Contanis(Vector4 Point)
        {
            var Value = Vector4.Dot(Normal_, Point);

            if (Value < 0)
            {
                return -1;
            }
            if (Value > 0)
            {
                return 1;
            }

            return 0;
        }

        public bool Intersect(Vertex P1, Vertex P2, out Vertex HitPoint)
        {
            if (Mathf.Approximately(Vector4.Dot(P2.Position - P1.Position, Normal_), 0))
            {
                HitPoint = P1;
                return false;
            }

            var D1 = Mathf.Abs(Vector4.Dot(P1.Position, Normal_));
            var D2 = Mathf.Abs(Vector4.Dot(P2.Position, Normal_));

            var Time = D1 / (D1 + D2);

            HitPoint = Vertex.Lerp(P1, P2, Time);
            return true;
        }
    }*/
}