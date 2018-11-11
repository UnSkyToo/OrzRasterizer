using System.Windows.Forms;
using App.Interface;
using XRasterizer;

namespace App.Render
{
    internal class RenderObject : IUpdateable, IRenderable
    {
        protected readonly GraphicsDevice Device_;
        
        public RenderObject(GraphicsDevice Device)
        {
            Device_ = Device;
        }
        
        public virtual void Update(float DeltaTime)
        {
            if (Input.IsKeyPressed(Keys.F))
            {
                if (Device_.GetFillMode() == FillMode.Solid)
                {
                    Device_.SetFillMode(FillMode.WireFrame);
                }
                else
                {
                    Device_.SetFillMode(FillMode.Solid);
                }
            }

            if (Input.IsKeyPressed(Keys.B))
            {
                if (Device_.GetCullFaceMode() == CullFaceMode.Back)
                {
                    Device_.SetCullFaceMode(CullFaceMode.Front);
                }
                else
                {
                    Device_.SetCullFaceMode(CullFaceMode.Back);
                }
            }

            if (Input.IsKeyPressed(Keys.C))
            {
                if (Device_.GetFrontFaceMode() == FrontFaceMode.CCW)
                {
                    Device_.SetFrontFaceMode(FrontFaceMode.CW);
                }
                else
                {
                    Device_.SetFrontFaceMode(FrontFaceMode.CCW);
                }
            }

            if (Input.IsKeyPressed(Keys.T))
            {
                Device_.GetTaskDispatcher().Stop();
                Device_.GetTaskDispatcher().Enable = !Device_.GetTaskDispatcher().Enable;
                Device_.GetTaskDispatcher().Start();
            }
            
            if (Input.IsKeyPressed(Keys.P))
            {
                Device_.GetProfiler().Enable = !Device_.GetProfiler().Enable;
            }

            if (Input.IsKeyPressed(Keys.M))
            {
                Device_.EnableSIMD(!Device_.IsEnableSIMD());
            }
        }

        public virtual void Render()
        {
        }
    }
}