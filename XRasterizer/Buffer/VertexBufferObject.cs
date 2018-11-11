using System;
using System.Numerics;
using System.Collections.Generic;
using XRasterizer.Base;

namespace XRasterizer.Buffer
{
    public class VertexBufferObject
    {
        private readonly VertexBufferFormat Format_;
        private readonly float[] Data_;
        private readonly Vertex[] Vertices_;

        public VertexBufferObject(VertexBufferFormat Format, float[] Data)
        {
            Format_ = Format;
            Data_ = Data;
            Vertices_ = Parse();
        }
        
        internal Vertex[] GetVertices()
        {
            return Vertices_;
        }

        private Vertex[] Parse()
        {
            var Vertices = new List<Vertex>();

            if (Format_ == VertexBufferFormat.Position)
            {
                for (var Index = 0; Index < Data_.Length; Index += 3)
                {
                    Vertices.Add(new Vertex(
                        new Vector3(Data_[Index + 0], Data_[Index + 1], Data_[Index + 2]),
                        Vector2.Zero,
                        Vector3.Zero));
                }
            }
            else if (Format_ == VertexBufferFormat.PositionUv)
            {
                for (var Index = 0; Index < Data_.Length; Index += 5)
                {
                    Vertices.Add(new Vertex(
                        new Vector3(Data_[Index + 0], Data_[Index + 1], Data_[Index + 2]),
                        new Vector2(Data_[Index + 3], Data_[Index + 4]),
                        Vector3.Zero));
                }
            }
            else if (Format_ == VertexBufferFormat.PositionUvNormal)
            {
                for (var Index = 0; Index < Data_.Length; Index += 8)
                {
                    Vertices.Add(new Vertex(
                        new Vector3(Data_[Index + 0], Data_[Index + 1], Data_[Index + 2]),
                        new Vector2(Data_[Index + 3], Data_[Index + 4]),
                        new Vector3(Data_[Index + 5], Data_[Index + 6], Data_[Index + 7])));
                }
            }

            return Vertices.ToArray();
        }
    }
}