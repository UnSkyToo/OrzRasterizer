using System.Numerics;
using XRasterizer.Base;

namespace XRasterizer.Shader
{
    public class ShaderColor : ShaderBase
    {
        internal override Vertex Vert(Vertex In)
        {
            In.Position = Vector4.Transform(In.Position, ModelViewProjectionMatrix);
            In.Normal = Vector3.Transform(In.Normal, NormalMatrix);

            return In;
        }

        internal override Colorf Frag(Vertex In)
        {
            return Colorf.Gray;
        }
    }
}