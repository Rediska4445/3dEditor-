using OpenTK;
using System;
using System.Drawing;
using System.IO;

namespace WindowsFormsApp3
{
    public class ModelPicker
    {
        private readonly MeshData _mesh;
        private readonly Func<Matrix4> _getModelMatrix;

        public ModelPicker(MeshData mesh, Func<Matrix4> getModelMatrix)
        {
            _mesh = mesh;
            _getModelMatrix = getModelMatrix;
        }

        public int FindClosestVertex(Vector3[] screenPositions, Point mousePos)
        {
            const float threshold = 10f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < _mesh.Vertices.Count && i < screenPositions.Length; i++)
            {
                float dx = screenPositions[i].X - mousePos.X;
                float dy = screenPositions[i].Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        public int FindClosestFace(Vector3[] screenCenters, Point mousePos)
        {
            const float threshold = 25f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < _mesh.Faces.Count && i < screenCenters.Length; i++)
            {
                float dx = screenCenters[i].X - mousePos.X;
                float dy = screenCenters[i].Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        public int FindClosestVertex(Point mousePos, Matrix4 view, Matrix4 projection,
                                     int controlWidth, int controlHeight)
        {
            const float threshold = 10f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {
                Vector3 screenPos = ProjectToScreen(_mesh.Vertices[i], view, projection,
                                                    controlWidth, controlHeight);
                float dx = screenPos.X - mousePos.X;
                float dy = screenPos.Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        public int FindClosestFace(Point mousePos, Matrix4 view, Matrix4 projection,
                                   int controlWidth, int controlHeight)
        {
            const float threshold = 50f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < _mesh.Faces.Count; i++)
            {
                var face = _mesh.Faces[i];
                Vector3 center = (_mesh.Vertices[face.v1] +
                                  _mesh.Vertices[face.v2] +
                                  _mesh.Vertices[face.v3]) / 3f;
                Vector3 screenPos = ProjectToScreen(center, view, projection,
                                                    controlWidth, controlHeight);
                float dx = screenPos.X - mousePos.X;
                float dy = screenPos.Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        public int DeleteFaceAtMousePosition(Point mousePos, Matrix4 view, Matrix4 projection,
                                             int width, int height)
        {
            int closestFace = FindClosestFace(mousePos, view, projection, width, height);
            if (closestFace == -1) return -1;

            _mesh.Faces.RemoveAt(closestFace);
            _mesh.RemoveOrphanedVertices();
            _mesh.UpdateIndicesAndEdges();
            return closestFace;
        }

        public void AddNewVertexAtPosition(Vector3 projectedPos, int baseFaceIndex,
                                           float radius = 0.1f)
        {
            if (baseFaceIndex < 0 || baseFaceIndex >= _mesh.Faces.Count) return;

            var baseFace = _mesh.Faces[baseFaceIndex];
            Vector3 edge1 = _mesh.Vertices[baseFace.v2] - _mesh.Vertices[baseFace.v1];
            Vector3 edge2 = _mesh.Vertices[baseFace.v3] - _mesh.Vertices[baseFace.v1];
            Vector3 tangent1 = Vector3.Normalize(edge1);
            Vector3 tangent2 = Vector3.Normalize(
                Vector3.Cross(edge1, Vector3.Cross(edge1, edge2)));

            int v1Index = _mesh.Vertices.Count;
            int v2Index = v1Index + 1;
            int v3Index = v1Index + 2;

            float angleStep = OpenTK.MathHelper.TwoPi / 3f;
            for (int i = 0; i < 3; i++)
            {
                float angle = i * angleStep;
                Vector3 offset = tangent1 * (float)Math.Cos(angle) * radius +
                                 tangent2 * (float)Math.Sin(angle) * radius;
                Vector3 newVertex = projectedPos + offset;
                _mesh.Vertices.Add(newVertex);
                _mesh.Normals.Add(Vector3.UnitY);
            }

            _mesh.Faces.Add(new Face { v1 = v1Index, v2 = v2Index, v3 = v3Index });
            _mesh.UpdateIndicesAndEdges();
        }

        public void AddNewFaceAtMousePosition(Point mousePos, Matrix4 view, Matrix4 projection,
                                              int controlWidth, int controlHeight,
                                              StreamWriter logWriter)
        {
            int closestFaceIndex = FindClosestFace(mousePos, view, projection,
                                                   controlWidth, controlHeight);
            if (closestFaceIndex == -1)
            {
                logWriter?.WriteLine("Нет ближайшей грани");
                return;
            }

            var baseFace = _mesh.Faces[closestFaceIndex];
            Vector3 baseCenter = _mesh.GetFaceCenter(closestFaceIndex);

            Vector3 clickWorldPos = ScreenToWorld(mousePos.X, mousePos.Y,
                                                  view, projection,
                                                  controlWidth, controlHeight);
            Vector3 projectedPos = ProjectPointToFacePlane(clickWorldPos, baseFace);

            float radius = 0.1f;
            int v1Index = _mesh.Vertices.Count;
            int v2Index = v1Index + 1;
            int v3Index = v1Index + 2;

            Vector3 edge1 = _mesh.Vertices[baseFace.v2] - _mesh.Vertices[baseFace.v1];
            Vector3 edge2 = _mesh.Vertices[baseFace.v3] - _mesh.Vertices[baseFace.v1];
            Vector3 tangent1 = Vector3.Normalize(edge1);
            Vector3 tangent2 = Vector3.Normalize(
                Vector3.Cross(edge1, Vector3.Cross(edge1, edge2)));

            float angleStep = OpenTK.MathHelper.TwoPi / 3f;
            for (int i = 0; i < 3; i++)
            {
                float angle = i * angleStep;
                Vector3 offset = tangent1 * (float)Math.Cos(angle) * radius +
                                 tangent2 * (float)Math.Sin(angle) * radius;
                Vector3 newVertex = projectedPos + offset;
                _mesh.Vertices.Add(newVertex);
                _mesh.Normals.Add(Vector3.UnitY);
            }

            _mesh.Faces.Add(new Face { v1 = v1Index, v2 = v2Index, v3 = v3Index });
            _mesh.UpdateIndicesAndEdges();

            logWriter?.WriteLine($"Новая грань #{_mesh.Faces.Count - 1} (вершины {v1Index}-{v2Index}-{v3Index})");
        }

        private Vector3 ProjectPointToFacePlane(Vector3 point, Face face)
        {
            Vector3 v1 = _mesh.Vertices[face.v1];
            Vector3 v2 = _mesh.Vertices[face.v2];
            Vector3 v3 = _mesh.Vertices[face.v3];

            Vector3 edge1 = v2 - v1;
            Vector3 edge2 = v3 - v1;
            Vector3 normal = Vector3.Cross(edge1, edge2);
            normal = Vector3.Normalize(normal);

            float t = Vector3.Dot(point - v1, normal);
            return point - (t * normal);
        }

        private Vector3 ScreenToWorld(int mouseX, int mouseY,
                                      Matrix4 view, Matrix4 projection,
                                      int width, int height)
        {
            float x = (2.0f * mouseX) / width - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / height;

            Vector4 rayStartNdc = new Vector4(x, y, -1.0f, 1.0f);
            Vector4 rayEndNdc = new Vector4(x, y, 1.0f, 1.0f);

            Matrix4 modelMatrix = _getModelMatrix();
            Matrix4 invModelViewProj = Matrix4.Invert(modelMatrix * view * projection);

            Vector4 rayStartWorld = Vector4.Transform(rayStartNdc, invModelViewProj);
            Vector4 rayEndWorld = Vector4.Transform(rayEndNdc, invModelViewProj);

            rayStartWorld /= rayStartWorld.W;
            rayEndWorld /= rayEndWorld.W;

            Vector3 dir = Vector3.Normalize(rayEndWorld.Xyz - rayStartWorld.Xyz);
            float dirY = dir.Y;

            if (Math.Abs(dirY) < 1e-6f)
                return rayStartWorld.Xyz + dir * 5f;

            float t = -rayStartWorld.Y / dirY;
            return rayStartWorld.Xyz + dir * t;
        }

        private Vector3 ProjectToScreen(Vector3 worldPos, Matrix4 view, Matrix4 projection,
                                        int width, int height)
        {
            Matrix4 modelMatrix = _getModelMatrix();
            Vector4 clipSpacePos = Vector4.Transform(new Vector4(worldPos, 1.0f),
                                                     modelMatrix * view * projection);

            Vector3 ndc = new Vector3(
                clipSpacePos.X / clipSpacePos.W,
                clipSpacePos.Y / clipSpacePos.W,
                clipSpacePos.Z / clipSpacePos.W
            );

            float screenX = (ndc.X * 0.5f + 0.5f) * width;
            float screenY = (1.0f - ndc.Y * 0.5f - 0.5f) * height;
            return new Vector3(screenX, screenY, ndc.Z);
        }
    }
}
