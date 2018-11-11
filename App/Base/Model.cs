using System.Collections.Generic;
using System.Numerics;
using App.Helper;
using App.Import;
using XRasterizer;
using XRasterizer.Shader;

namespace App.Base
{
    public class Model
    {
        private readonly Dictionary<string, Texture> LoadTextures_;
        private readonly List<Mesh> Meshes_;
        private string Directory_;
        private Vector3 Position_;
        private Quaternion Rotate_;
        private Vector3 Scale_;

        public int MeshCount => Meshes_.Count;

        public Matrix4x4 Transform => Matrix4x4.CreateScale(Scale_) * Matrix4x4.CreateFromQuaternion(Rotate_) * Matrix4x4.CreateTranslation(Position_);
        
        public Model(string FilePath)
        {
            LoadTextures_ = new Dictionary<string, Texture>();
            Meshes_ = new List<Mesh>();
            Position_  = Vector3.Zero;
            Rotate_ = Quaternion.Identity;
            Scale_ = Vector3.One;
            LoadModel(FilePath);
        }

        public void SetPosition(Vector3 Position)
        {
            Position_ = Position;
        }

        public void SetRotate(Quaternion Rotate)
        {
            Rotate_ = Rotate;
        }

        public void SetScale(Vector3 Scale)
        {
            Scale_ = Scale;
        }

        public void Draw(GraphicsDevice Device, ShaderBase Shader)
        {
            foreach (var Mesh in Meshes_)
            {
                Mesh.Draw(Device, Shader);
            }
        }

        public void Draw(GraphicsDevice Device, ShaderBase Shader, int Index)
        {
            Meshes_[Index].Draw(Device, Shader);
        }

        private void LoadModel(string FilePath)
        {
            var Import = new ObjImport();
            var Scene = Import.Parse(FilePath);

            if (Scene == null)
            {
                return;
            }

            Directory_ = FileHelper.GetDirectory(FilePath);
            ProcessNode(Scene.RootNode, Scene);
        }

        private void ProcessNode(ImpSceneNode Node, ImpScene Scene)
        {
            foreach (var MeshIndex in Node.Meshes)
            {
                var Mesh = ProcessMesh(Scene.Meshes[MeshIndex], Scene);
                if (Mesh != null)
                {
                    Meshes_.Add(Mesh);
                }
            }

            foreach (var Child in Node.Children)
            {
                ProcessNode(Child, Scene);
            }
        }

        private Mesh ProcessMesh(ImpMesh Mesh, ImpScene Scene)
        {
            var Vertices = new List<float>();
            var Indices = new List<int>();
            var Textures = new List<Texture>();

            foreach(var Vert in Mesh.Vertices)
            {
                Vertices.Add(Vert.Position.X);
                Vertices.Add(Vert.Position.Y);
                Vertices.Add(Vert.Position.Z);
                Vertices.Add(Vert.TexCoord.X);
                Vertices.Add(Vert.TexCoord.Y);
                Vertices.Add(Vert.Normal.X);
                Vertices.Add(Vert.Normal.Y);
                Vertices.Add(Vert.Normal.Z);
            }

            foreach (var Face in Mesh.Faces)
            {
                Indices.AddRange(Face.Indices);
            }
            
            if (Mesh.MaterialIndex != -1)
            {
                var Material = Scene.Materials[Mesh.MaterialIndex];

                var Diffuses = LoadMaterialTextures(Material.Diffuses, "texture_diffuse");
                Textures.AddRange(Diffuses);
                var Speculars = LoadMaterialTextures(Material.Speculars, "texture_specular");
                Textures.AddRange(Speculars);
                var Normals = LoadMaterialTextures(Material.Normals, "texture_normal");
                Textures.AddRange(Normals);
            }

            if (Textures.Count == 0)
            {
                return null;
            }
            var Me = new Mesh(Vertices.ToArray(), Indices.ToArray(), Textures.ToArray());
            Me.Name = Mesh.Name;
            return Me;
        }

        private List<Texture> LoadMaterialTextures(List<string> TextureNames, string TypeStr)
        {
            var Textures = new List<Texture>();

            if (TextureNames == null)
            {
                return Textures;
            }

            foreach(var TexName in TextureNames)
            {
                if (LoadTextures_.ContainsKey(TexName))
                {
                    Textures.Add(LoadTextures_[TexName]);
                    continue;
                }

                var Tex = new Texture();
                Tex.Tex = TextureHelper.Load(FileHelper.Combine(Directory_, TexName));
                Tex.Type = TypeStr;
                Tex.Name = TexName;
                Textures.Add(Tex);
                LoadTextures_.Add(Tex.Name, Tex);
            }

            return Textures;
        }
    }
}