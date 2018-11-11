namespace XRasterizer.Buffer
{
    public class IndexBufferObject
    {
        private readonly int[] Indices_;

        public IndexBufferObject(int[] Indices)
        {
            Indices_ = Indices;
        }

        public int[] GetIndices()
        {
            return Indices_;
        }
    }
}