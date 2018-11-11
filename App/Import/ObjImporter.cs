using System.IO;
using System.Text;
using System.Collections.Generic;
using App.Helper;

namespace App.Import
{
    public class ObjImport : IImport
    {
        public ObjImport()
        {
        }

        public ImpScene Parse(string FilePath)
        {
            var Lines = File.ReadAllLines(FilePath, Encoding.UTF8);
            var Index = 0;

            var Scene = new ImpScene();
            Scene.RootNode = new ImpSceneNode();
            Scene.RootNode.Meshes = new List<int>();
            Scene.RootNode.Children = new List<ImpSceneNode>();

            Scene.Meshes = new List<ImpMesh>();
            Scene.Materials = new List<ImpMaterial>();
            
            var Mesh = new ImpMesh();
            
            var FaceToVertices = new Dictionary<string, int>();
            var Positions = new List<ImpVec3>();
            var TexCoords = new List<ImpVec2>();
            var Normals = new List<ImpVec3>();
            
            while (Index < Lines.Length)
            {
                var Line = Lines[Index++];

                if (Line.Length == 0 || Line[0] == '#')
                {
                    continue;
                }

                var Cmds = FormatLine(Line);
                switch (Cmds[0])
                {
                    case "mtllib":
                        var Materials = ParseMtlLib(FileHelper.Combine(FileHelper.GetDirectory(FilePath), Cmds[1]));
                        Scene.Materials.AddRange(Materials);
                        break;
                    case "usemtl":
                        for (var MIndex = 0; MIndex < Scene.Materials.Count; ++MIndex)
                        {
                            if (Scene.Materials[MIndex].Name == Cmds[1])
                            {
                                Mesh.MaterialIndex = MIndex;
                                break;
                            }
                        }
                        break;
                    case "o": // define object
                        Mesh = new ImpMesh();
                        Mesh.Name = Cmds[1];
                        Mesh.MaterialIndex = -1;
                        Mesh.Vertices = new List<ImpVertex>();
                        Mesh.Faces = new List<ImpFace>();
                        Scene.Meshes.Add(Mesh);
                        
                        Scene.RootNode.Meshes.Add(Scene.Meshes.Count - 1);
                        break;
                    case "g": // define group
                        Mesh = new ImpMesh();
                        Mesh.Name = Cmds[1];
                        Mesh.MaterialIndex = -1;
                        Mesh.Vertices = new List<ImpVertex>();
                        Mesh.Faces = new List<ImpFace>();
                        Scene.Meshes.Add(Mesh);

                        Scene.RootNode.Meshes.Add(Scene.Meshes.Count - 1);
                        break;
                    case "v": // define vertex
                        Positions.Add(new ImpVec3(float.Parse(Cmds[1]), float.Parse(Cmds[2]), float.Parse(Cmds[3])));
                        break;
                    case "vt": // define uv
                        TexCoords.Add(new ImpVec2(float.Parse(Cmds[1]), 1-float.Parse(Cmds[2])));
                        break;
                    case "vn": // define normal
                        Normals.Add(new ImpVec3(float.Parse(Cmds[1]), float.Parse(Cmds[2]), float.Parse(Cmds[3])));
                        break;
                    case "s": // smoothing group
                        break;
                    case "f": // face
                        var Face = new ImpFace();
                        Face.Indices = new List<int>();

                        for (var FIndex = 1; FIndex < Cmds.Length; ++FIndex)
                        {
                            if (!FaceToVertices.ContainsKey(Cmds[FIndex]))
                            {
                                var Vertex = new ImpVertex();
                                var Segments = Cmds[FIndex].Split('/');
                                Vertex.Position = Positions[int.Parse(Segments[0]) - 1];
                                Vertex.TexCoord = TexCoords[int.Parse(Segments[1]) - 1];
                                Vertex.Normal = Normals[int.Parse(Segments[2]) - 1];
                                Mesh.Vertices.Add(Vertex);

                                FaceToVertices.Add(Cmds[FIndex], Mesh.Vertices.Count - 1);
                            }
                            
                            Face.Indices.Add(FaceToVertices[Cmds[FIndex]]);
                        }

                        if (Cmds.Length == 5)
                        {
                            Face.Indices.Insert(3, Face.Indices[0]);
                            Face.Indices.Insert(4, Face.Indices[2]);
                        }
                        
                        Mesh.Faces.Add(Face);
                        break;
                    default:
                        break;
                }
            }
            
            return Scene;
        }

        private string[] FormatLine(string Line)
        {
            var Lines = Line.Split(' ');
            var Result = new List<string>();

            foreach (var L in Lines)
            {
                if (!string.IsNullOrWhiteSpace(L))
                {
                    Result.Add(L);
                }
            }

            return Result.ToArray();
        }

        private List<ImpMaterial> ParseMtlLib(string FilePath)
        {
            var Lines = File.ReadAllLines(FilePath, Encoding.UTF8);
            var Index = 0;

            var Materials = new List<ImpMaterial>();
            var Material = new ImpMaterial();

            while (Index < Lines.Length)
            {
                var Line = Lines[Index++];

                if (Line.Length == 0 || Line[0] == '#')
                {
                    continue;
                }

                var Cmds = Line.Split(' ');
                switch (Cmds[0].ToLower().TrimStart())
                {
                    case "newmtl":
                        Material = new ImpMaterial();
                        Material.Name = Cmds[1];
                        Material.Ambients = new List<string>();
                        Material.Diffuses = new List<string>();
                        Material.Speculars = new List<string>();
                        Material.Normals = new List<string>();
                        Materials.Add(Material);
                        break;
                    case "ns":
                        break;
                    case "ka":
                        break;
                    case "kd":
                        break;
                    case "ks":
                        break;
                    case "ke":
                        break;
                    case "ki":
                        break;
                    case "d":
                        break;
                    case "illum":
                        break;
                    case "tr":
                        break;
                    case "tf":
                        break;
                    case "map_ka":
                        Material.Ambients.Add(Cmds[1]);
                        break;
                    case "map_kd":
                        Material.Diffuses.Add(Cmds[1]);
                        break;
                    case "map_bump":
                        Material.Normals.Add(Cmds[1]);
                        break;
                    case "map_ks":
                        Material.Speculars.Add(Cmds[1]);
                        break;
                    default:
                        break;
                }
            }

            return Materials;
        }
    }
}