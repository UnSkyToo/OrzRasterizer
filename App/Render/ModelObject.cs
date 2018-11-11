using System;
using System.Numerics;
using System.Windows.Forms;
using App.Base;
using App.Helper;
using XRasterizer;
using XRasterizer.Base;
using XRasterizer.Shader;

namespace App.Render
{
    internal class ModelObject : RenderObject
    {
        private ShaderBase Shader_;
        private Camera Camera_;
        private Model Model_;
        private float Value_;

        public ModelObject(GraphicsDevice Device)
            : base(Device)
        {
            var DirLight = new Light();
            DirLight.Type = LightType.Direction;
            DirLight.Direction = new Vector3(0, 0, -1f);
            DirLight.Ambient = new Colorf(0.05f, 0.05f, 0.05f);
            DirLight.Diffuse = new Colorf(0.4f, 0.4f, 0.4f);
            DirLight.Specular = new Colorf(0.5f, 0.5f, 0.5f);
            DirLight.Enabled = true;
            DirLight.UpdateSIMD();

            var PointLight = new Light();
            PointLight.Type = LightType.Point;
            PointLight.Position = new Vector3(0, 0, 5);
            PointLight.Ambient = new Colorf(0.05f, 0.05f, 0.05f);
            PointLight.Diffuse = new Colorf(0.8f, 0.8f, 0.8f);
            PointLight.Specular = new Colorf(1.0f, 1.0f, 1.0f);
            PointLight.KC = 1.0f;
            PointLight.KL = 0.09f;
            PointLight.KQ = 0.032f;
            PointLight.Enabled = false;
            PointLight.UpdateSIMD();

            var SpotLight = new Light();
            SpotLight.Type = LightType.Spot;
            SpotLight.Position = new Vector3(0, 0, 2);
            SpotLight.Direction = new Vector3(0, 0, -1f);
            SpotLight.Ambient = new Colorf(0.0f, 0.0f, 0.0f);
            SpotLight.Diffuse = new Colorf(1.0f, 0.0f, 0.0f);
            SpotLight.Specular = new Colorf(1.0f, 0.0f, 0.0f);
            SpotLight.KC = 1.0f;
            SpotLight.KL = 0.09f;
            SpotLight.KQ = 0.032f;
            SpotLight.Inner = Mathf.Cos(3.0f);
            SpotLight.Outer = Mathf.Cos(8.0f);
            SpotLight.Enabled = false;
            SpotLight.UpdateSIMD();

            Shader_ = new ShaderLight();
            Shader_.Lights = new Light[3];
            Shader_.Lights[0] = DirLight;
            Shader_.Lights[1] = PointLight;
            Shader_.Lights[2] = SpotLight;
            Shader_.SpecularPower = 32;

            //Camera_ = new Camera(new Vector3(0, 0, 3), new Vector3(0, 0, -1), new Vector3(0, 1, 0), 60.0f, 800.0f / 600.0f, 0.3f, 100.0f);
            Camera_ = new Camera(800, 600);
            //Camera_.Position = new Vector3(0, 0, 3);

            Model_ = new Model(FileHelper.GetFullPath("nanosuit/nanosuit.obj"));
            //Model_ = new Model(FileHelper.GetFullPath("SponzaAtrium/sponza.mod"));
            //Model_ = new Model(FileHelper.GetFullPath("model/Pagoda.mod"));
            //Model_ = new Model(FileHelper.GetFullPath("violin/violin.mod"));
            Model_.SetScale(new Vector3(0.2f));
            Model_.SetPosition(new Vector3(0, -1.5f, 0));
            Value_ = 0;

            if (false)
            {
                Camera_.Position = new Vector3(0, 10, -45);
            }
        }

        public override void Update(float DeltaTime)
        {
            base.Update(DeltaTime);
            Camera_.Update(DeltaTime);

            Value_ += DeltaTime;
            Shader_.Time = Shader_.Time + DeltaTime;

            if (Input.IsKeyPressed(Keys.L))
            {
                Shader_.Lights[1].Enabled = !Shader_.Lights[1].Enabled;
            }
            
            //Model_.SetRotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, Value_));
        }

        public override void Render()
        {
            Device_.ClearColor(new Colorf(0.0f, 0.1f, 0.1f, 1.0f));
            Device_.Clear(ClearType.Color | ClearType.Depth);
            
            Shader_.ModelMatrix = Model_.Transform;
            Shader_.ViewMatrix = Camera_.ViewMatrix;
            Shader_.ProjectionMatrix = Camera_.ProjectionMatrix;
            Shader_.ModelViewMatrix = Shader_.ModelMatrix * Shader_.ViewMatrix;
            Shader_.ModelViewProjectionMatrix = Shader_.ModelViewMatrix * Shader_.ProjectionMatrix;
            Matrix4x4.Invert(Shader_.ModelMatrix, out var NormalMatrix);
            NormalMatrix = Matrix4x4.Transpose(NormalMatrix);
            Shader_.NormalMatrix = NormalMatrix;
            Shader_.CameraPos = Camera_.Position;

            Model_.Draw(Device_, Shader_);
            //Model_.Draw(Device_, Shader_, 1);
        }
    }
}
