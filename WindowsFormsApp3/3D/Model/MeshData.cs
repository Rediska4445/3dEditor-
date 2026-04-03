using OpenTK;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WindowsFormsApp3
{
    public struct Face
    {
        public int v1, v2, v3;

        public Vector3 Color;

        public Face(int v1, int v2, int v3, Vector3 color = default)
        {
            this.v1 = v1; this.v2 = v2; this.v3 = v3; this.Color = color;
        }
    }

    public class MeshData
    {
        public List<Vector3> Vertices { get; private set; } = new List<Vector3>();
        public List<Vector3> Normals { get; private set; } = new List<Vector3>();
        public List<Face> Faces { get; private set; } = new List<Face>();
        public List<uint> Indices { get; private set; } = new List<uint>();
        public List<uint> Edges { get; private set; } = new List<uint>();

        public void Clear()
        {
            Vertices.Clear();
            Normals.Clear();
            Faces.Clear();
            Indices.Clear();
            Edges.Clear();
        }

        public Vector3 GetFaceNormal(int faceIndex)
        {
            if (faceIndex < 0 || faceIndex >= Faces.Count)
                return Vector3.UnitY;

            var face = Faces[faceIndex];
            if (face.v1 >= Vertices.Count || face.v2 >= Vertices.Count || face.v3 >= Vertices.Count)
                return Vector3.UnitY;

            Vector3 edge1 = Vertices[face.v2] - Vertices[face.v1];
            Vector3 edge2 = Vertices[face.v3] - Vertices[face.v1];
            Vector3 normal = Vector3.Cross(edge1, edge2);

            if (normal.Length > 0.0001f)
                return Vector3.Normalize(normal);

            return Vector3.UnitY;
        }

        public void LoadFromAssimpMesh(Assimp.Mesh mesh)
        {
            Clear();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var v = mesh.Vertices[i];
                Vertices.Add(new Vector3(v.X, v.Y, v.Z));
            }

            if (mesh.HasNormals)
            {
                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    var n = mesh.Normals[i];
                    Normals.Add(new Vector3(n.X, n.Y, n.Z));
                }
            }
            else
            {
                for (int i = 0; i < Vertices.Count; i++)
                    Normals.Add(Vector3.UnitY);
            }

            float maxCoord = Vertices.Max(v => v.Length);
            float scale = 1.0f / (maxCoord * 1.5f);
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] *= scale;
                if (Normals.Count > i) Normals[i] = Vector3.Normalize(Normals[i]);
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                var face = mesh.Faces[i];
                if (face.IndexCount >= 3)
                {
                    Faces.Add(new Face
                    {
                        v1 = face.Indices[0],
                        v2 = face.Indices[1],
                        v3 = face.Indices[2]
                    });
                }
            }

            UpdateIndicesAndEdges();
        }

        public void UpdateIndicesAndEdges()
        {
            Indices.Clear();
            Edges.Clear();

            foreach (var face in Faces)
            {
                if (face.v1 >= 0 && face.v2 >= 0 && face.v3 >= 0 &&
                    face.v1 < Vertices.Count && face.v2 < Vertices.Count && face.v3 < Vertices.Count)
                {
                    Indices.Add((uint)face.v1);
                    Indices.Add((uint)face.v2);
                    Indices.Add((uint)face.v3);

                    Edges.Add((uint)face.v1); Edges.Add((uint)face.v2);
                    Edges.Add((uint)face.v2); Edges.Add((uint)face.v3);
                    Edges.Add((uint)face.v3); Edges.Add((uint)face.v1);
                }
            }
        }

        public void RemoveOrphanedVertices()
        {
            var usedVertices = new HashSet<int>();
            foreach (var face in Faces)
            {
                if (face.v1 >= 0 && face.v1 < Vertices.Count) usedVertices.Add(face.v1);
                if (face.v2 >= 0 && face.v2 < Vertices.Count) usedVertices.Add(face.v2);
                if (face.v3 >= 0 && face.v3 < Vertices.Count) usedVertices.Add(face.v3);
            }

            var newVertices = new List<Vector3>();
            var newNormals = new List<Vector3>();
            var vertexMap = new Dictionary<int, int>();

            for (int i = 0; i < Vertices.Count; i++)
            {
                if (usedVertices.Contains(i))
                {
                    int newIndex = newVertices.Count;
                    vertexMap[i] = newIndex;
                    newVertices.Add(Vertices[i]);
                    newNormals.Add(Normals.Count > i ? Normals[i] : Vector3.UnitY);
                }
            }

            Vertices = newVertices;
            Normals = newNormals;

            for (int i = 0; i < Faces.Count; i++)
            {
                var face = Faces[i];
                face.v1 = vertexMap.ContainsKey(face.v1) ? vertexMap[face.v1] : 0;
                face.v2 = vertexMap.ContainsKey(face.v2) ? vertexMap[face.v2] : 0;
                face.v3 = vertexMap.ContainsKey(face.v3) ? vertexMap[face.v3] : 0;
                Faces[i] = face;
            }

            UpdateIndicesAndEdges();
        }

        public Vector3 GetFaceCenter(int faceIndex)
        {
            if (faceIndex < 0 || faceIndex >= Faces.Count) return Vector3.Zero;
            var f = Faces[faceIndex];
            return (Vertices[f.v1] + Vertices[f.v2] + Vertices[f.v3]) / 3f;
        }

        public float[] ToInterleavedVertexArray()
        {
            float[] vertexArray = new float[Vertices.Count * 6];
            for (int i = 0; i < Vertices.Count; i++)
            {
                vertexArray[i * 6 + 0] = Vertices[i].X;
                vertexArray[i * 6 + 1] = Vertices[i].Y;
                vertexArray[i * 6 + 2] = Vertices[i].Z;
                vertexArray[i * 6 + 3] = Normals.Count > i ? Normals[i].X : 0;
                vertexArray[i * 6 + 4] = Normals.Count > i ? Normals[i].Y : 1;
                vertexArray[i * 6 + 5] = Normals.Count > i ? Normals[i].Z : 0;
            }
            return vertexArray;
        }

        public float[] ToVerticesOnlyArray()
        {
            float[] arr = new float[Vertices.Count * 3];
            for (int i = 0; i < Vertices.Count; i++)
            {
                arr[i * 3 + 0] = Vertices[i].X;
                arr[i * 3 + 1] = Vertices[i].Y;
                arr[i * 3 + 2] = Vertices[i].Z;
            }
            return arr;
        }

        public float[] ToEdgesArray()
        {
            float[] arr = new float[Edges.Count * 3];
            for (int i = 0; i < Edges.Count; i++)
            {
                int idx = (int)Edges[i];
                if (idx < Vertices.Count)
                {
                    arr[i * 3 + 0] = Vertices[idx].X;
                    arr[i * 3 + 1] = Vertices[idx].Y;
                    arr[i * 3 + 2] = Vertices[idx].Z;
                }
            }
            return arr;
        }

        public void SaveToObj(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("# Exported from custom model");

                foreach (var v in Vertices)
                    writer.WriteLine(
                        "v " +
                        v.X.ToString(CultureInfo.InvariantCulture) + " " +
                        v.Y.ToString(CultureInfo.InvariantCulture) + " " +
                        v.Z.ToString(CultureInfo.InvariantCulture));

                if (Normals.Count == Vertices.Count)
                {
                    foreach (var n in Normals)
                        writer.WriteLine(
                            "vn " +
                            n.X.ToString(CultureInfo.InvariantCulture) + " " +
                            n.Y.ToString(CultureInfo.InvariantCulture) + " " +
                            n.Z.ToString(CultureInfo.InvariantCulture));
                }

                foreach (var f in Faces)
                    writer.WriteLine("f " + (f.v1 + 1) + " " + (f.v2 + 1) + " " + (f.v3 + 1));
            }
        }
    }
}
