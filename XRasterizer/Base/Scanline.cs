namespace XRasterizer.Base
{
    internal struct Scanline
    {
        public Vertex Left;
        public Vertex Step;
        public int X;
        public int Y;
        public int Width;
    }
}