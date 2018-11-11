namespace XRasterizer.Base
{
    internal struct Box
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

        public int X;
        public int Y;
        public int Width;
        public int Height;
        
        public Box(int Left, int Top, int Right, int Bottom)
        {
            this.Left = Left;
            this.Right = Right;
            this.Top = Top;
            this.Bottom = Bottom;
            this.X = Left;
            this.Y = Top;
            this.Width = Right - Left;
            this.Height = Bottom - Top;
        }

        public bool Contanis(int X, int Y)
        {
            if (X < Left || X >= Right || Y < Top || Y >= Bottom)
            {
                return false;
            }

            return true;
        }
    }
}