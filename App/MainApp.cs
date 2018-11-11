using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using App.Helper;
using App.Render;
using XRasterizer;
using XRasterizer.Helper;

namespace App
{
    internal class MainApp
    {
        public const int Width = 800;
        public const int Height = 600;

        private readonly Form WinForm_;
        private readonly PictureBox Surface_;
        private readonly Font TextFont_;
        private double DeltaTime_;

        private readonly GraphicsDevice Device_;
        private readonly RenderObject Obj_;
        
        public MainApp()
        {
            WinForm_ = new Form
            {
                ClientSize = new Size(Width, Height),
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                Text = "Orz App",
                KeyPreview = true,
            };

            Surface_ = new PictureBox
            {
                Location = Point.Empty,
                ClientSize = WinForm_.ClientSize,
            };
            WinForm_.Controls.Add(Surface_);

            WinForm_.KeyDown += OnKeyDown;
            WinForm_.KeyUp += OnKeyUp;
            Surface_.MouseDown += OnMouseDown;
            Surface_.MouseUp += OnMouseUp;
            Surface_.MouseMove += OnMouseMove;

            TextFont_ = new Font("Arial", 10);

            FileHelper.AddSearchPath(Application.StartupPath + "/../../Res/");
            FileHelper.AddSearchPath(Application.StartupPath + "/../../../Res/");

            Device_ = RasterizerEngine.CreateDevice(WinForm_.Handle, Width, Height);
            Obj_ = new TestObject(Device_);
        }

        private void OnKeyDown(object Sender, KeyEventArgs Args)
        {
            Input.OnKeyDown(Args.KeyCode);
        }

        private void OnKeyUp(object Sender, KeyEventArgs Args)
        {
            Input.OnKeyUp(Args.KeyCode);
        }

        private void OnMouseDown(object Sender, MouseEventArgs Args)
        {
            Input.OnMouseDown(Args.Button, Args.X, Args.Y);
        }

        private void OnMouseUp(object Sender, MouseEventArgs Args)
        {
            Input.OnMouseUp(Args.Button, Args.X, Args.Y);
        }

        private void OnMouseMove(object Sender, MouseEventArgs Args)
        {
            Input.OnMouseMove(Args.Button, Args.X, Args.Y);
        }

        public void Run()
        {
            WinForm_.Show();

            var PreviousFrameTicks = 0L;
            var Watch = new Stopwatch();
            Watch.Start();

            while (!WinForm_.IsDisposed)
            {
                var CurrentFrameTicks = Watch.ElapsedTicks;
                DeltaTime_ = (CurrentFrameTicks - PreviousFrameTicks) / (double)Stopwatch.Frequency;
                PreviousFrameTicks = CurrentFrameTicks;

                MainLoop();
                Application.DoEvents();
            }

            RasterizerEngine.DestroyDevice(Device_);
        }

        private void MainLoop()
        {
            Device_.GetProfiler().Begin(ProfilerElementType.LoopTime);

            Update();
            Render();
            Input.Update();

            Device_.GetProfiler().End(ProfilerElementType.LoopTime);
            Device_.GetProfiler().Update();
        }

        private void Update()
        {
            Obj_.Update((float)DeltaTime_);
        }
        
        private void Render()
        {
            Device_.GetProfiler().Begin(ProfilerElementType.RenderTime);

            Device_.GetSwapchain().Begin(Device_);
            Obj_.Render();
            Device_.GetSwapchain().End();

            Device_.GetProfiler().End(ProfilerElementType.RenderTime);

            using (var G = Graphics.FromImage(Device_.GetSwapchain().Background))
            {
                G.SmoothingMode = SmoothingMode.AntiAlias;
                G.TextRenderingHint = TextRenderingHint.AntiAlias;
                G.DrawString($"Fps : {GetFps((float)DeltaTime_)}", TextFont_, Brushes.White, 5, 5);

                DrawProfilerInfo(G);
            }

            //Surface_.Visible = false;
            //Device_.GetSwapchain().SwapBuffers();
            Surface_.Image = Device_.GetSwapchain().Background;
        }

        private void DrawProfilerInfo(Graphics G)
        {
            var X = 5;
            var Y = 25;
            var YStep = 20;

            var Profiler = Device_.GetProfiler();
            var Text = $"LoopTime : {Profiler.GetAverageTime(ProfilerElementType.LoopTime):00.00}ms  " +
                       $"RenderTime : {Profiler.GetAverageTime(ProfilerElementType.RenderTime):00.00}ms  " +
                       $"ClearTime : {Profiler.GetAverageTime(ProfilerElementType.ClearTime):00.00}ms  " +
                       $"RasterizeTime : {Profiler.GetAverageTime(ProfilerElementType.RasterizeTime):00.00}ms  ";
            G.DrawString(Text, TextFont_, Brushes.White, X, Y);
            Y += YStep;

            Text = $"VertexShaderTime : {Profiler.GetAverageTime(ProfilerElementType.VertexShaderTime):00.00}ms  " +
                   $"FragmentShaderTime : {Profiler.GetAverageTime(ProfilerElementType.FragmentShaderTime):00.00}ms  " +
                   $"AssemblyTime : {Profiler.GetAverageTime(ProfilerElementType.PrimitiveAssemblyTime):00.00}ms  " +
                   $"ToCVVTime : {Profiler.GetAverageTime(ProfilerElementType.TransformToCVVTime):00.00}ms  ";
            G.DrawString(Text, TextFont_, Brushes.White, X, Y);
            Y += YStep;

            Text = $"ThreadTask : {Profiler.GetCount(ProfilerElementType.Core1TaskCount):0000}" +
                   $" / {Profiler.GetCount(ProfilerElementType.Core2TaskCount):0000}" +
                   $" / {Profiler.GetCount(ProfilerElementType.Core3TaskCount):0000}" +
                   $" / {Profiler.GetCount(ProfilerElementType.Core4TaskCount):0000}" +
                   $" / {Profiler.GetCount(ProfilerElementType.Core5TaskCount):0000}" +
                   $" / {Profiler.GetCount(ProfilerElementType.Core6TaskCount):0000}" +
                   $" / {Profiler.GetCount(ProfilerElementType.Core7TaskCount):0000}" +
                   $" / {Profiler.GetCount(ProfilerElementType.Core8TaskCount):0000}  " +
                   $"TaskCount : {Profiler.GetCount(ProfilerElementType.TaskCount)}  " +
                   $"VertexCount : {Profiler.GetCount(ProfilerElementType.VertexCount)}  ";
            G.DrawString(Text, TextFont_, Brushes.White, X, Y);
            Y += YStep;

            Text = $"CullFaceMode(B) : {Device_.GetCullFaceMode()}  " +
                   $"FrontFaceMode(C) : {Device_.GetFrontFaceMode()}  " +
                   $"EnableThread(T) : {Device_.GetTaskDispatcher().Enable}  " +
                   $"EnableSIMD(M) : {Device_.IsEnableSIMD()}  ";
            G.DrawString(Text, TextFont_, Brushes.White, X, Y);
            Y += YStep;
        }
        
        private float FpsDuration_ = 1.0f / 60.0f;
        private float FpsAlpha = 1.0f / 10.0f;
        private int FpsFrameCount_ = 0;
        private int GetFps(float Ms)
        {
            FpsFrameCount_++;
            FpsDuration_ = FpsDuration_ * (1 - FpsAlpha) + Ms * FpsAlpha;
            return (int)(1.0f / FpsDuration_);
        }
    }
}