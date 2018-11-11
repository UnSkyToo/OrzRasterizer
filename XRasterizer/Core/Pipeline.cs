using System.Numerics;
using XRasterizer.Base;
using XRasterizer.Helper;

namespace XRasterizer.Core
{
    internal class Pipeline
    {
        private readonly GraphicsDevice Device_;
        private readonly GraphicsContext Context_;
        private readonly Rasterizer[] Rasterizer_;
        
        public Pipeline(GraphicsDevice Device)
        {
            Device_ = Device;
            Context_ = Device.GetContext();
            Rasterizer_ = new Rasterizer[Context_.CoreNum];
            for (var Index = 0; Index < Context_.CoreNum; ++Index)
            {
                Rasterizer_[Index] = new Rasterizer(Index, Context_);
            }
        }

        public Rasterizer GetRasterizer(int Index)
        {
            return Rasterizer_[Index];
        }

        public void Handle(int StartVertex, int VertexCount)
        {
            Context_.Vertices.Clear();
            Context_.Vertices.AddRange(Context_.VertexBuffer.GetVertices());

            ShaderVertex();
            PrimitiveAssembly(StartVertex, VertexCount);
            TransformToCVV();
            Rasterize();
            ShaderFragment();
        }

        public void Handle(int StartVertex, int StartIndex, int IndexCount)
        {
            Context_.Vertices.Clear();
            Context_.Vertices.AddRange(Context_.VertexBuffer.GetVertices());
            Context_.Indices = Context_.IndexBuffer.GetIndices();

            ShaderVertex();
            PrimitiveAssembly(StartVertex, StartIndex, IndexCount);
            TransformToCVV();
            Rasterize();
            ShaderFragment();
        }
        
        public void ShaderVertex()
        {
            if (Context_.Shader == null)
            {
                return;
            }
            
            Device_.GetProfiler().Begin(ProfilerElementType.VertexShaderTime);

            for (var Index = 0; Index < Context_.CoreNum; ++Index)
            {
                Device_.GetTaskDispatcher().PushTask(Index, new VertexShaderTask(Context_, Index, Context_.CoreNum));
            }
            Device_.GetTaskDispatcher().Wait();

            Device_.GetProfiler().End(ProfilerElementType.VertexShaderTime);
        }

        public void PrimitiveAssembly(int StartVertex, int VertexCount)
        {
            Device_.GetProfiler().Begin(ProfilerElementType.PrimitiveAssemblyTime);

            for (var Index = 0; Index < Context_.CoreNum; ++Index)
            {
                Device_.GetTaskDispatcher().PushTask(Index, new PrimitiveAssemblyTask(Context_, Index, StartVertex + Index * 3, StartVertex + VertexCount, Context_.CoreNum * 3));
            }
            Device_.GetTaskDispatcher().Wait();
            Context_.Vertices.FlushToMain();

            Device_.GetProfiler().End(ProfilerElementType.PrimitiveAssemblyTime);
            Device_.GetProfiler().Increase(ProfilerElementType.VertexCount, Context_.Vertices.Count);
        }

        public void PrimitiveAssembly(int StartVertex, int StartIndex, int IndexCount)
        {
            Device_.GetProfiler().Begin(ProfilerElementType.PrimitiveAssemblyTime);

            for (var Index = 0; Index < Context_.CoreNum; ++Index)
            {
                Device_.GetTaskDispatcher().PushTask(Index, new IndexPrimitiveAssemblyTask(Context_, Index, StartIndex + Index * 3, StartIndex + IndexCount, Context_.CoreNum * 3));
            }
            Device_.GetTaskDispatcher().Wait();
            Context_.Vertices.FlushToMain();

            Device_.GetProfiler().End(ProfilerElementType.PrimitiveAssemblyTime);
            Device_.GetProfiler().Increase(ProfilerElementType.VertexCount, Context_.Vertices.Count);
        }
        
        public void TransformToCVV()
        {
            Device_.GetProfiler().Begin(ProfilerElementType.TransformToCVVTime);

            for (var Index = 0; Index < Context_.CoreNum; ++Index)
            {
                Device_.GetTaskDispatcher().PushTask(Index, new TransformToCVVTask(Context_, Index, Context_.CoreNum));
            }
            Device_.GetTaskDispatcher().Wait();

            Device_.GetProfiler().End(ProfilerElementType.TransformToCVVTime);
        }

        public void Rasterize()
        {
            Device_.GetProfiler().Begin(ProfilerElementType.RasterizeTime);
            
            Context_.Fragments.Clear();
            for (var Index = 0; Index < Context_.CoreNum; ++Index)
            {
                Device_.GetTaskDispatcher().PushTask(Index, new RasterizerTask(Context_, Rasterizer_[Index], Index * 3, Context_.CoreNum * 3));
            }
            Device_.GetTaskDispatcher().Wait();
            Context_.Fragments.FlushToMain();

            Device_.GetProfiler().End(ProfilerElementType.RasterizeTime);
        }

        public void ShaderFragment()
        {
            Device_.GetProfiler().Begin(ProfilerElementType.FragmentShaderTime);

            for (var Index = 0; Index < Context_.CoreNum; ++Index)
            {
                Device_.GetTaskDispatcher().PushTask(Index, new FragmentShaderTask(Context_, this, Index, Context_.CoreNum));
            }
            Device_.GetTaskDispatcher().Wait();

            Device_.GetProfiler().End(ProfilerElementType.FragmentShaderTime);
        }
    }
}