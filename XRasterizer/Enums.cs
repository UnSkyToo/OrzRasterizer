using System;

namespace XRasterizer
{
    [Flags]
    public enum ClearType : byte
    {
        Color = 1 << 0,
        Depth = 1 << 1,
        //Stencil = 1 << 2,
    }
    
    public enum FillMode : byte
    {
        Solid = 0,
        WireFrame = 1,
    }

    public enum CullFaceMode : byte
    {
        Back = 0,
        Front = 1,
        FrontAndBack = 2,
    }

    public enum FrontFaceMode : byte
    {
        None = 0,
        CCW = 1,
        CW = 2,
    }
    
    public enum PrimitiveType : byte
    {
        Triangle = 1,
    }

    public enum TextureSlot : byte
    {
        Texture1 = 0,
        Texture2 = 1,
        Texture3 = 2,
        Texture4 = 3,
        Texture5 = 4,
        Texture6 = 5,
        Texture7 = 6,
        Texture8 = 7,
    }

    public enum TextureWarpMode : byte
    {
        Repeat = 0,
        Clamp = 1,
        Border = 2,
    }

    public enum TextureFilterMode : byte
    {
        Linear = 0,
        Point = 1,
    }

    public enum LightType : byte
    {
        Direction = 0,
        Point = 1,
        Spot = 2,
    }

    public enum VertexBufferFormat : byte
    {
        Position = 0,
        PositionUv = 1,
        PositionUvNormal = 2,
    }
}