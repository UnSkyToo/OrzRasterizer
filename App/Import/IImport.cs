using System.Collections.Generic;
using App.Base;

namespace App.Import
{
    public class ImpVec2
    {
        public float X;
        public float Y;

        public ImpVec2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public class ImpVec3
    {
        public float X;
        public float Y;
        public float Z;

        public ImpVec3(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }

    public class ImpVertex
    {
        public ImpVec3 Position;
        public ImpVec3 Normal;
        public ImpVec2 TexCoord;
    }

    public class ImpScene
    {
        public ImpSceneNode RootNode;
        public List<ImpMesh> Meshes;
        public List<ImpMaterial> Materials;
    }

    public class ImpMesh
    {
        public string Name;
        public List<ImpVertex> Vertices;
        public List<ImpFace> Faces;
        public int MaterialIndex;
    }

    public class ImpFace
    {
        public List<int> Indices;
    }
    
    public class ImpMaterial
    {
        public string Name;
        public List<string> Ambients;
        public List<string> Diffuses;
        public List<string> Speculars;
        public List<string> Normals;
    }

    public class ImpSceneNode
    {
        public List<ImpSceneNode> Children;
        public List<int> Meshes;
    }
    
    interface IImport
    {
        ImpScene Parse(string FilePath);
    }
}