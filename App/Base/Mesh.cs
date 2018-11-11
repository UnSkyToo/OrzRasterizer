using XRasterizer;
using XRasterizer.Base;
using XRasterizer.Buffer;
using XRasterizer.Shader;

namespace App.Base
{
    public class Mesh
    {
        public string Name;
        public float[] Vertices;
        public int[] Indices;
        public Texture[] Textures;
        private VertexBufferObject VertexBuffer_;
        private IndexBufferObject IndexBuffer_;
        private Texture2D TexDiffuse_;
        private Texture2D TexSpecular_;
        
        public Mesh(float[] Vertices, int[] Indices, Texture[] Textures)
        {
            this.Vertices = Vertices;
            this.Indices = Indices;
            this.Textures = Textures;

            SetupMesh();
        }

        public void Draw(GraphicsDevice Device, ShaderBase Shader)
        {
            Shader.TexDiffuse = TexDiffuse_;
            Shader.TexSpecular = TexSpecular_;
            Device.UseShader(Shader);
            Device.BindVertexBuffer(VertexBuffer_);
            Device.BindIndexBuffer(IndexBuffer_);

            Device.DrawElements(PrimitiveType.Triangle, 0, 0, Indices.Length);
        }

        private void SetupMesh()
        {
            VertexBuffer_ = new VertexBufferObject(VertexBufferFormat.PositionUvNormal, Vertices);
            IndexBuffer_ = new IndexBufferObject(Indices);

            for (var Index = 0; Index < Textures.Length; ++Index)
            {
                if (Textures[Index].Type == "texture_diffuse")
                {
                    TexDiffuse_ = Textures[Index].Tex;
                }
                else if (Textures[Index].Type == "texture_specular")
                {
                    TexSpecular_ = Textures[Index].Tex;
                }
            }
        }
    }
}