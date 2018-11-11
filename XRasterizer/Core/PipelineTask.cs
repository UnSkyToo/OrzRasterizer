using System.Numerics;
using System.Runtime.CompilerServices;
using XRasterizer.Base;
using XRasterizer.Helper;

namespace XRasterizer.Core
{
    internal unsafe class ClearColorTask : TaskBase
    {
        private readonly GraphicsContext Context_;
        private readonly int Start_;
        private readonly int Count_;
        private readonly byte Value_;

        public ClearColorTask(GraphicsContext Context, int Start, byte Value)
        {
            Context_ = Context;
            Start_ = Start;
            Count_ = Context_.Viewport.Width * Context.Viewport.Height * 4;
            Value_ = Value;
        }

        public override void Execute()
        {
            for (var Index = Start_; Index < Count_; Index += 4)
            {
                Context_.ColorBuffer[Index] = Value_;
            }
        }
    }

    internal class ClearDepthTask : TaskBase
    {
        private readonly GraphicsContext Context_;
        private readonly int Count_;

        public ClearDepthTask(GraphicsContext Context)
        {
            Context_ = Context;
            Count_ = Context.ZBuffer.Length;
        }

        public override void Execute()
        {
            for (var Index = 0; Index < Count_; ++Index)
            {
                Context_.ZBuffer[Index] = 1.0f;
            }
        }
    }

    internal class VertexShaderTask : TaskBase
    {
        private readonly GraphicsContext Context_;
        private readonly int Start_;
        private readonly int Step_;
        private readonly int Count_;

        public VertexShaderTask(GraphicsContext Context, int Start, int Step)
        {
            Context_ = Context;
            Start_ = Start;
            Step_ = Step;
            Count_ = Context_.Vertices.Count;
        }

        public override void Execute()
        {
            for (var Index = Start_; Index < Count_; Index += Step_)
            {
                Context_.Vertices[Index] = Context_.Shader.Vert(Context_.Vertices[Index]);
            }
        }
    }

    internal class PrimitiveAssemblyTask : TaskBase
    {
        protected readonly GraphicsContext Context_;
        protected readonly int CoreIndex_;
        protected readonly int Start_;
        protected readonly int Step_;
        protected readonly int Count_;

        public PrimitiveAssemblyTask(GraphicsContext Context, int CoreIndex, int Start, int Count, int Step)
        {
            Context_ = Context;
            CoreIndex_ = CoreIndex;
            Start_ = Start;
            Step_ = Step;
            Count_ = Count;
        }

        public override void Execute()
        {
            for (var Index = Start_; Index < Count_; Index += Step_)
            {
                if (!BackCull(Context_.Vertices[Index + 0], Context_.Vertices[Index + 1], Context_.Vertices[Index + 2]))
                {
                    continue;
                }

                if (!FrustumClipping(Context_.Vertices[Index + 0]) ||
                    !FrustumClipping(Context_.Vertices[Index + 1]) ||
                    !FrustumClipping(Context_.Vertices[Index + 2]))
                {
                    continue;
                }

                Context_.Vertices.Add(CoreIndex_, Context_.Vertices[Index + 0]);
                Context_.Vertices.Add(CoreIndex_, Context_.Vertices[Index + 1]);
                Context_.Vertices.Add(CoreIndex_, Context_.Vertices[Index + 2]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool BackCull(Vertex V1, Vertex V2, Vertex V3)
        {
            if (Context_.FillModeState == FillMode.WireFrame || Context_.FrontFaceModeState == FrontFaceMode.None)
            {
                return true;
            }

            var Vec1 = V2.Position - V1.Position;
            var Vec2 = V3.Position - V2.Position;
            var Normal = Vector3.Normalize(Vector3.Cross(new Vector3(Vec1.X, Vec1.Y, Vec1.Z), new Vector3(Vec2.X, Vec2.Y, Vec2.Z)));

            var ViewDir = Vector3.Normalize(new Vector3(V1.Position.X, V1.Position.Y, V1.Position.Z)/* - new Vector3(0, 0, 0)*/); // 变换MVP后相机位置为原点
            var DotValue = Vector3.Dot(ViewDir, Normal);

            switch (Context_.CullFaceModeState)
            {
                case CullFaceMode.Back:
                    return (Context_.FrontFaceModeState == FrontFaceMode.CCW) ? DotValue >= 0 : DotValue < 0;
                case CullFaceMode.Front:
                    return (Context_.FrontFaceModeState == FrontFaceMode.CCW) ? DotValue < 0 : DotValue >= 0;
                case CullFaceMode.FrontAndBack:
                    return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool FrustumClipping(Vertex Vert)
        {
            if (Context_.FillModeState == FillMode.WireFrame)
            {
                return true;
            }

            if (Vert.Position.X >= -Vert.Position.W && Vert.Position.X <= Vert.Position.W &&
                Vert.Position.Y >= -Vert.Position.W && Vert.Position.Y <= Vert.Position.W &&
                Vert.Position.Z >= 0.0f && Vert.Position.Z <= Vert.Position.W)
            {
                return true;
            }

            return false;
        }
    }

    internal class IndexPrimitiveAssemblyTask : PrimitiveAssemblyTask
    {
        public IndexPrimitiveAssemblyTask(GraphicsContext Context, int CoreIndex, int Start, int Count, int Step)
            : base(Context, CoreIndex, Start, Count, Step)
        {
        }

        public override void Execute()
        {
            for (var Index = Start_; Index < Count_; Index += Step_)
            {
                if (!BackCull(Context_.Vertices[Context_.Indices[Index + 0]],
                    Context_.Vertices[Context_.Indices[Index + 1]], Context_.Vertices[Context_.Indices[Index + 2]]))
                {
                    continue;
                }

                if (!FrustumClipping(Context_.Vertices[Context_.Indices[Index + 0]]) ||
                    !FrustumClipping(Context_.Vertices[Context_.Indices[Index + 1]]) ||
                    !FrustumClipping(Context_.Vertices[Context_.Indices[Index + 2]]))
                {
                    continue;
                }

                Context_.Vertices.Add(CoreIndex_, Context_.Vertices[Context_.Indices[Index + 0]]);
                Context_.Vertices.Add(CoreIndex_, Context_.Vertices[Context_.Indices[Index + 1]]);
                Context_.Vertices.Add(CoreIndex_, Context_.Vertices[Context_.Indices[Index + 2]]);
            }
        }
    }

    internal class TransformToCVVTask : TaskBase
    {
        private readonly GraphicsContext Context_;
        private readonly int Start_;
        private readonly int Step_;
        private readonly int Count_;

        public TransformToCVVTask(GraphicsContext Context, int Start, int Step)
        {
            Context_ = Context;
            Start_ = Start;
            Step_ = Step;
            Count_ = Context_.Vertices.Count;
        }

        public override void Execute()
        {
            if (GraphicsContext.EnableSIMD)
            {
                for (var Index = Start_; Index < Count_; Index += Step_)
                {
                    var OneDivZ = 1.0f / Context_.Vertices[Index].W;
                    Context_.Vertices[Index] = new Vertex(Context_.Vertices[Index].Vec * OneDivZ, OneDivZ);
                }
            }
            else
            {
                for (var Index = Start_; Index < Count_; Index += Step_)
                {
                    var OneDivZ = 1.0f / Context_.Vertices[Index].Position.W;
                    var Vert = new Vertex(Context_.Vertices[Index].Position * OneDivZ,
                        Context_.Vertices[Index].TexCoords * OneDivZ,
                        Context_.Vertices[Index].Normal * OneDivZ);
                    Vert.Position.W = OneDivZ;

                    Context_.Vertices[Index] = Vert;
                }
            }
        }
    }

    internal class RasterizerTask : TaskBase
    {
        private readonly GraphicsContext Context_;
        private readonly Rasterizer Rasterizer_;
        private readonly int Start_;
        private readonly int Step_;
        private readonly int Count_;

        public RasterizerTask(GraphicsContext Context, Rasterizer Rasterizer, int Start, int Step)
        {
            Context_ = Context;
            Rasterizer_ = Rasterizer;
            Start_ = Start;
            Step_ = Step;
            Count_ = Context_.Vertices.Count;
        }

        public override void Execute()
        {
            for (var Index = Start_; Index < Count_; Index += Step_)
            {
                Rasterizer_.Handle(Context_.Vertices[Index + 0], Context_.Vertices[Index + 1], Context_.Vertices[Index + 2]);
            }
        }
    }
    
    internal class FragmentShaderTask : TaskBase
    {
        private readonly GraphicsContext Context_;
        private readonly Pipeline Pipeline_;
        private readonly int Index_;
        private readonly int Count_;
        private readonly int Step_;

        public FragmentShaderTask(GraphicsContext Context, Pipeline Pipeline, int Index, int Step)
        {
            Context_ = Context;
            Pipeline_ = Pipeline;
            Index_ = Index;
            Count_ = Context_.Fragments.Count;
            Step_ = Step;
        }

        public override void Execute()
        {
            for (var Index = Index_; Index < Count_; Index += Step_)
            {
                var Frag = Context_.Fragments[Index];
                var PixelIndex = (Frag.Y * Context_.Viewport.Width + Frag.X) << 2;
                if (Context_.FillModeState == FillMode.WireFrame)
                {
                    WritePixel(PixelIndex, Colorf.White);
                }
                else
                {
                    var OriginZ = 1.0f / Frag.Vert.Position.W;
                    Frag.Vert.Normal *= OriginZ;
                    Frag.Vert.TexCoords *= OriginZ;

                    var Color = Context_.Shader.Frag(Frag.Vert);
                    WritePixel(PixelIndex, Color);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WritePixel(int Index, Colorf Color)
        {
            Context_.ColorBuffer[Index + 0] = (byte)(Color.B * 255);
            Context_.ColorBuffer[Index + 1] = (byte)(Color.G * 255);
            Context_.ColorBuffer[Index + 2] = (byte)(Color.R * 255);
            Context_.ColorBuffer[Index + 3] = 255;
        }
    }
}