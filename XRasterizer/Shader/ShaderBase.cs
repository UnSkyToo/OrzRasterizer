using System.Numerics;
using System.Runtime.CompilerServices;
using XRasterizer.Base;

namespace XRasterizer.Shader
{
    public abstract class ShaderBase : ShaderBuiltIn
    {
        public Matrix4x4 ModelMatrix;
        public Matrix4x4 ViewMatrix;
        public Matrix4x4 ProjectionMatrix;
        public Matrix4x4 ModelViewMatrix;
        public Matrix4x4 ModelViewProjectionMatrix;
        public Matrix4x4 NormalMatrix;
        public Texture2D TexDiffuse;
        public Texture2D TexSpecular;
        public float SpecularPower;
        public Vector3 CameraPos;
        public Light[] Lights;
        public float Time;
        
        internal abstract Vertex Vert(Vertex In);

        internal abstract Colorf Frag(Vertex In);

        internal Colorf Lighting(Vertex Vert, Vector3 ViewPos, Light[] Lights)
        {
            if (Lights == null)
            {
                return tex2D(TexDiffuse, Vert.TexCoords);
            }

            var Ambient = Colorf.Black;
            var Diffuse = Colorf.Black;
            var Specular = Colorf.Black;
            var FragPos = new Vector3(Vert.Position.X, Vert.Position.Y, Vert.Position.Z);
            var ViewDir = Vector3.Normalize(ViewPos - FragPos);
            
            foreach (var Light in Lights)
            {
                if (!Light.Enabled)
                {
                    continue;
                }

                if (Light.Type == LightType.Direction)
                {
                    var LightDir = Vector3.Normalize(-Light.Direction);
                    var Diff = Mathf.Max(Vector3.Dot(Vert.Normal, LightDir), 0.0f);
                    var ReflectDir = Vector3.Reflect(-LightDir, Vert.Normal);
                    var Spec = Mathf.Pow(Mathf.Max(Vector3.Dot(ViewDir, ReflectDir), 0.0f), SpecularPower);

                    Ambient += (Light.Ambient * tex2D(TexDiffuse, Vert.TexCoords));
                    Diffuse += (Light.Diffuse * Diff * tex2D(TexDiffuse, Vert.TexCoords));
                    Specular += (Light.Specular * Spec * tex2D(TexSpecular, Vert.TexCoords));
                }
                else if (Light.Type == LightType.Point)
                {
                    var LightDir = Vector3.Normalize(Light.Position - FragPos);
                    var Diff = Mathf.Max(Vector3.Dot(Vert.Normal, LightDir), 0.0f);
                    var ReflectDir = Vector3.Reflect(-LightDir, Vert.Normal);
                    var Spec = Mathf.Pow(Mathf.Max(Vector3.Dot(ViewDir, ReflectDir), 0.0f), SpecularPower);

                    var Dist = Vector3.Distance(FragPos, Light.Position);
                    var Atten = 1.0f / (Light.KC + Light.KL * Dist + Light.KQ * (Dist * Dist));

                    Ambient += Atten * (Light.Ambient * tex2D(TexDiffuse, Vert.TexCoords));
                    Diffuse += Atten * (Light.Diffuse * Diff * tex2D(TexDiffuse, Vert.TexCoords));
                    Specular += Atten * (Light.Specular * Spec * tex2D(TexSpecular, Vert.TexCoords));
                }
                else if (Light.Type == LightType.Spot)
                {
                    var LightDir = Vector3.Normalize(Light.Position - FragPos);
                    var Diff = Mathf.Max(Vector3.Dot(Vert.Normal, LightDir), 0.0f);
                    var ReflectDir = Vector3.Reflect(-LightDir, Vert.Normal);
                    var Spec = Mathf.Pow(Mathf.Max(Vector3.Dot(ViewDir, ReflectDir), 0.0f), SpecularPower);

                    var Dist = Vector3.Distance(FragPos, Light.Position);
                    var Atten = 1.0f / (Light.KC + Light.KL * Dist + Light.KQ * (Dist * Dist));
                    var Theta = Vector3.Dot(LightDir, Vector3.Normalize(-Light.Direction));
                    var Epsilon = Light.Inner - Light.Outer;
                    var Intensity = Mathf.Clamp((Theta - Light.Outer) / Epsilon, 0, 1);

                    Ambient += Atten * Intensity * (Light.Ambient * tex2D(TexDiffuse, Vert.TexCoords));
                    Diffuse += Atten * Intensity * (Light.Diffuse * Diff * tex2D(TexDiffuse, Vert.TexCoords));
                    Specular += Atten * Intensity * (Light.Specular * Spec * tex2D(TexSpecular, Vert.TexCoords));
                }
            }
            
            return Ambient + Diffuse + Specular;
        }

        internal unsafe Colorf LightingVec(Vertex Vert, Vector3 ViewPos, Light[] Lights)
        {
            if (Lights == null)
            {
                return tex2D(TexDiffuse, Vert.TexCoords);
            }
            
            var AmbientVec = Vector<double>.Zero;
            var DiffuseVec = Vector<double>.Zero;
            var SpecularVec = Vector<double>.Zero;
            var FragPos = new Vector3(Vert.Position.X, Vert.Position.Y, Vert.Position.Z);
            var ViewDir = Vector3.Normalize(ViewPos - FragPos);

            foreach (var Light in Lights)
            {
                if (!Light.Enabled)
                {
                    continue;
                }
                Light.UpdateSIMD();

                var Diff = 0.0d;
                var Spec = 0.0d;
                var Atten = 1.0d;

                if (Light.Type == LightType.Direction)
                {
                    var LightDir = Vector3.Normalize(-Light.Direction);
                    Diff = Mathf.Max(Vector3.Dot(Vert.Normal, LightDir), 0.0f);
                    var ReflectDir = Vector3.Reflect(-LightDir, Vert.Normal);
                    Spec = Mathf.Pow(Mathf.Max(Vector3.Dot(ViewDir, ReflectDir), 0.0f), SpecularPower);
                }
                else if (Light.Type == LightType.Point)
                {
                    var LightDir = Vector3.Normalize(Light.Position - FragPos);
                    Diff = Mathf.Max(Vector3.Dot(Vert.Normal, LightDir), 0.0f);
                    var ReflectDir = Vector3.Reflect(-LightDir, Vert.Normal);
                    Spec = Mathf.Pow(Mathf.Max(Vector3.Dot(ViewDir, ReflectDir), 0.0f), SpecularPower);

                    var Dist = Vector3.Distance(FragPos, Light.Position);
                    Atten = 1.0f / (Light.KC + Light.KL * Dist + Light.KQ * (Dist * Dist));
                }
                else if (Light.Type == LightType.Spot)
                {
                    var LightDir = Vector3.Normalize(Light.Position - FragPos);
                    Diff = Mathf.Max(Vector3.Dot(Vert.Normal, LightDir), 0.0f);
                    var ReflectDir = Vector3.Reflect(-LightDir, Vert.Normal);
                    Spec = Mathf.Pow(Mathf.Max(Vector3.Dot(ViewDir, ReflectDir), 0.0f), SpecularPower);

                    var Dist = Vector3.Distance(FragPos, Light.Position);
                    Atten = 1.0f / (Light.KC + Light.KL * Dist + Light.KQ * (Dist * Dist));
                    var Theta = Vector3.Dot(LightDir, Vector3.Normalize(-Light.Direction));
                    var Epsilon = Light.Inner - Light.Outer;
                    var Intensity = Mathf.Clamp((Theta - Light.Outer) / Epsilon, 0, 1);

                    Atten *= Intensity;
                }
                
                AmbientVec += Atten * (Unsafe.Read<Vector<double>>(Light.AmbientVecPtr) * Unsafe.Read<Vector<double>>(tex2DVec(TexDiffuse, Vert.TexCoords)));
                DiffuseVec += Atten * (Unsafe.Read<Vector<double>>(Light.DiffuseVecPtr) * Unsafe.Read<Vector<double>>(tex2DVec(TexDiffuse, Vert.TexCoords)) * Diff);
                SpecularVec += Atten * (Unsafe.Read<Vector<double>>(Light.SpecularVecPtr) * Unsafe.Read<Vector<double>>(tex2DVec(TexSpecular, Vert.TexCoords)) * Spec);
            }
            
            var ColorVec = AmbientVec + DiffuseVec + SpecularVec;
            return new Colorf((float)ColorVec[0], (float)ColorVec[1], (float)ColorVec[2], (float)ColorVec[3]);
        }
    }
}