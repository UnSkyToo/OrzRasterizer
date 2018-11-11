using System;
using System.Numerics;
using XRasterizer.Base;
using XRasterizer.Buffer;
using XRasterizer.Core;
using XRasterizer.Helper;
using XRasterizer.Shader;

namespace XRasterizer
{
    public class GraphicsDevice
    {
        private GraphicsContext Context_;
        private readonly Swapchain Swapchain_;
        private readonly TaskDispatcher TaskDispatcher_;
        private readonly Profiler ProfilerHelper_;
        private readonly Pipeline Pipeline_;

        public GraphicsDevice(IntPtr Handle, int Width, int Height)
        {
            Context_ = new GraphicsContext(this, new Box(0, 0, Width, Height));
            Swapchain_ = new Swapchain(Handle, Width, Height);

            TaskDispatcher_ = new TaskDispatcher(this, Context_.CoreNum);
            TaskDispatcher_.Enable = true;
            TaskDispatcher_.Start();

            ProfilerHelper_ = new Profiler();
            ProfilerHelper_.Enable = true;

            Pipeline_ = new Pipeline(this);
        }

        public void Destroy()
        {
            TaskDispatcher_.Stop();
        }
        
        internal GraphicsContext GetContext()
        {
            return Context_;
        }

        public Swapchain GetSwapchain()
        {
            return Swapchain_;
        }

        public TaskDispatcher GetTaskDispatcher()
        {
            return TaskDispatcher_;
        }

        public Profiler GetProfiler()
        {
            return ProfilerHelper_;
        }
        
        public void Viewport(int X, int Y, int Width, int Height)
        {
            Context_ = new GraphicsContext(this, new Box(X, Y, X + Width, Y + Height));
        }
        
        public void Clear(ClearType Type)
        {
            ProfilerHelper_.Begin(ProfilerElementType.ClearTime);

            if ((Type & ClearType.Color) == ClearType.Color)
            {
                TaskDispatcher_.PushTask(0 % Context_.CoreNum, new ClearColorTask(Context_, 0, (byte)(Context_.ClearColor.B * 255)));
                TaskDispatcher_.PushTask(1 % Context_.CoreNum, new ClearColorTask(Context_, 1, (byte)(Context_.ClearColor.G * 255)));
                TaskDispatcher_.PushTask(2 % Context_.CoreNum, new ClearColorTask(Context_, 2, (byte)(Context_.ClearColor.R * 255)));
                TaskDispatcher_.PushTask(3 % Context_.CoreNum, new ClearColorTask(Context_, 3, (byte)(Context_.ClearColor.A * 255)));
            }

            if ((Type & ClearType.Depth) == ClearType.Depth)
            {
                TaskDispatcher_.PushTask(4 % Context_.CoreNum, new ClearDepthTask(Context_));
            }

            /*if ((Type & ClearType.Stencil) == ClearType.Stencil)
            {
            }*/

            TaskDispatcher_.Wait();
            Context_.FrameCount++;

            ProfilerHelper_.End(ProfilerElementType.ClearTime);
        }

        public void ClearColor(Colorf ClearColor)
        {
            Context_.ClearColor = ClearColor;
        }
        
        public void DrawArrays(PrimitiveType Type, int StartVertex, int VertexCount)
        {
            Pipeline_.Handle(StartVertex, VertexCount);
        }

        public void DrawElements(PrimitiveType Type, int StartVertex, int StartIndex, int IndexCount)
        {
            Pipeline_.Handle(StartVertex, StartIndex, IndexCount);
        }

        public void DrawLine(Vector3 V1, Vector3 V2)
        {
            Pipeline_.GetRasterizer(0).BresenhamLine(new Vertex(V1, Vector2.Zero, Vector3.Zero), new Vertex(V2, Vector2.Zero, Vector3.Zero));
        }

        public void DrawDebug()
        {
        }

        public void BindVertexBuffer(VertexBufferObject Buffer)
        {
            Context_.VertexBuffer = Buffer;
        }

        public void BindIndexBuffer(IndexBufferObject Buffer)
        {
            Context_.IndexBuffer = Buffer;
        }
        
        public void UseShader(ShaderBase Shader)
        {
            Context_.Shader = Shader;
        }

        public void SetFillMode(FillMode Mode)
        {
            Context_.FillModeState = Mode;
        }

        public FillMode GetFillMode()
        {
            return Context_.FillModeState;
        }

        public void SetCullFaceMode(CullFaceMode Mode)
        {
            Context_.CullFaceModeState = Mode;
        }

        public CullFaceMode GetCullFaceMode()
        {
            return Context_.CullFaceModeState;
        }

        public void SetFrontFaceMode(FrontFaceMode Mode)
        {
            Context_.FrontFaceModeState = Mode;
        }

        public FrontFaceMode GetFrontFaceMode()
        {
            return Context_.FrontFaceModeState;
        }

        public bool IsEnableSIMD()
        {
            return GraphicsContext.EnableSIMD;
        }

        public void EnableSIMD(bool Enabled)
        {
            GraphicsContext.EnableSIMD = Enabled;
        }
    }
}