using System;
using System.Numerics;
using System.Windows.Forms;
using App.Base;
using App.Helper;
using XRasterizer;
using XRasterizer.Base;
using XRasterizer.Buffer;
using XRasterizer.Shader;

namespace App.Render
{
    internal class TestObject : RenderObject
    {
        private ShaderBase Shader_;
        private Camera Camera_;
        private readonly Random Rand_;
        
        private float Value_ = 0.0f;

        private VertexBufferObject VertexBuffer_;
        private IndexBufferObject IndexBuffer_;

        public TestObject(GraphicsDevice Device)
            : base(Device)
        {
            Rand_ = new Random((int)DateTime.Now.Ticks);

            var Vertices = new[]
            {
                -1f,  1f, 0f, 0f, 0f, 0f, 0f, 1f, // 0
                 1f,  1f, 0f, 1f, 0f, 0f, 0f, 1f, // 1
                 1f, -1f, 0f, 1f, 1f, 0f, 0f, 1f, // 2
                -1f, -1f, 0f, 0f, 1f, 0f, 0f, 1f  // 3
            };

            var Indices = new[]
            {
                0, 3, 1, // first triangle
                1, 3, 2  // second triangle
            };

            VertexBuffer_ = new VertexBufferObject(VertexBufferFormat.PositionUvNormal, Vertices);
            IndexBuffer_ = new IndexBufferObject(Indices);

            var DirLight = new Light();
            DirLight.Type = LightType.Direction;
            DirLight.Direction = new Vector3(0.2f, 0.2f, -1f);
            DirLight.Ambient = new Colorf(0.05f, 0.05f, 0.05f);
            DirLight.Diffuse = new Colorf(0.4f, 0.4f, 0.4f);
            DirLight.Specular = new Colorf(0.5f, 0.5f, 0.5f);
            DirLight.Enabled = true;
            DirLight.UpdateSIMD();

            Shader_ = new ShaderLight();
            Shader_.Lights = new Light[1];
            Shader_.Lights[0] = DirLight;
            
            Shader_.TexDiffuse = TextureHelper.Load("container2.png");
            Shader_.TexSpecular = TextureHelper.Load("container2_specular.png");
            Shader_.SpecularPower = 32;

            //Camera_ = new Camera(new Vector3(0, 0, 3), new Vector3(0, 0, -1), new Vector3(0, 1, 0), 60.0f, 800.0f / 600.0f, 0.3f, 100.0f);
            Camera_ = new Camera(800, 600);
        }

        public override void Update(float DeltaTime)
        {
            base.Update(DeltaTime);
            Camera_.Update(DeltaTime);

            Value_ = Value_ + (float)Math.PI * DeltaTime;
            
            if (Input.IsKeyPressed(Keys.L))
            {
                Shader_.Lights[0].Enabled = !Shader_.Lights[0].Enabled;
            }
        }

        public override void Render()
        {
            Device_.ClearColor(new Colorf(0.0f, 0.1f, 0.1f, 1.0f));
            Device_.Clear(ClearType.Color | ClearType.Depth);

            //Device_.DrawDebug();return;

            var ObjMatrix = Matrix4x4.CreateRotationY(Value_);
            //ObjMatrix = Matrix4x4.Identity;

            Shader_.ModelMatrix = Matrix4x4.Identity;
            Shader_.ViewMatrix = Camera_.ViewMatrix;
            Shader_.ProjectionMatrix = Camera_.ProjectionMatrix;
            Shader_.ModelViewMatrix = Shader_.ModelMatrix * Shader_.ViewMatrix;
            Shader_.ModelViewProjectionMatrix = Shader_.ModelViewMatrix * Shader_.ProjectionMatrix;
            Matrix4x4.Invert(Shader_.ModelMatrix, out var NormalMatrix);
            NormalMatrix = Matrix4x4.Transpose(NormalMatrix);
            Shader_.NormalMatrix = NormalMatrix;
            Shader_.CameraPos = Camera_.Position;

            Device_.BindVertexBuffer(VertexBuffer_);
            Device_.BindIndexBuffer(IndexBuffer_);
            Device_.UseShader(Shader_);
            //Device_.DrawArrays(PrimitiveType.Triangle, 0, 6);
            Device_.DrawElements(PrimitiveType.Triangle, 0, 0, 6);
        }
    }
}