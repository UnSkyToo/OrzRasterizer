using XRasterizer.Base;
using XRasterizer.Buffer;
using XRasterizer.Shader;

namespace XRasterizer.Core
{
    internal unsafe class GraphicsContext
    {
        public const int MaxTextureCount = 8;

        public int CoreNum;
        public static bool EnableSIMD = true;

        public GraphicsDevice Device;
        public Box Viewport;
        public int FrameCount;
        public byte* ColorBuffer;
        public float[] ZBuffer;

        public ListCore<Vertex> Vertices;
        public ListCore<Fragment> Fragments;
        public int[] Indices;

        public Colorf ClearColor;
        public FillMode FillModeState;
        public CullFaceMode CullFaceModeState;
        public FrontFaceMode FrontFaceModeState;
        public ShaderBase Shader;
        public VertexBufferObject VertexBuffer;
        public IndexBufferObject IndexBuffer;

        public GraphicsContext(GraphicsDevice Device, Box Viewport)
        {
            this.CoreNum = 6;

            this.Device = Device;
            this.Viewport = Viewport;
            this.FrameCount = 0;

            this.ColorBuffer = null;
            this.ZBuffer = new float[Viewport.Width * Viewport.Height];

            this.Vertices = new ListCore<Vertex>(this.CoreNum);
            this.Fragments = new ListCore<Fragment>(this.CoreNum);
            this.Indices = null;

            this.ClearColor = Colorf.Black;
            this.FillModeState = FillMode.Solid;
            this.CullFaceModeState = CullFaceMode.Back;
            this.FrontFaceModeState = FrontFaceMode.CCW;
            this.Shader = null;
            this.VertexBuffer = null;
            this.IndexBuffer = null;
        }
    }
}