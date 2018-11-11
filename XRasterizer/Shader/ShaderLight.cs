using System.Numerics;
using XRasterizer.Base;
using XRasterizer.Core;
using XRasterizer.Helper;

namespace XRasterizer.Shader
{
    public class ShaderLight : ShaderBase
    {
        internal override Vertex Vert(Vertex In)
        {
            In.Position = Vector4.Transform(In.Position, ModelViewProjectionMatrix);
            In.Normal = Vector3.Normalize(Vector3.Transform(In.Normal, NormalMatrix));

            return In;
        }

        internal override Colorf Frag(Vertex In)
        {
            //return TexDiffuse.Sample(In.TexCoords);
            if (GraphicsContext.EnableSIMD)
            {
                return LightingVec(In, CameraPos, Lights);
            }
            return Lighting(In, CameraPos, Lights);
        }
    }
}