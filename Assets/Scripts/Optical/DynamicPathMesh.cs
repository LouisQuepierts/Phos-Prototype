using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Phos.Optical {
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DynamicPathMesh : MonoBehaviour {
        public float width = 0.1f;
        public Material material;

        private Mesh _mesh;
        private readonly List<Vector3> _vertices = new();
        private readonly List<int> _triangles = new();
        private readonly List<Vector2> _uvs = new();
        private readonly List<Color> _colors = new();

        private void Awake() {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            GetComponent<MeshRenderer>().material = material;
        }

        public void UpdatePath(List<LightPath> paths) {
            GenerateMesh(paths);
            UpdateMesh();
        }

        public void UpdatePath(List<Vector3> points) {
            GenerateMeshData(points);
            UpdateMesh();
        }

        private void GenerateMesh(List<LightPath> paths) {
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();
            _colors.Clear();
            
            foreach (LightPath path in paths) {
                Vector3 start = path.Start;
                Vector3 end = path.End;

                Vector3 forward = path.Light.Direction;
                
                Vector3 right = Vector3.Cross(forward, Vector3.up).normalized;
                Vector3 up = Vector3.Cross(forward, right).normalized;
                
                if (right.magnitude < 0.1f) {
                    right = Vector3.Cross(forward, Vector3.forward).normalized;
                    up = Vector3.Cross(forward, right).normalized;
                }

                Tube(start, end, right, up, new Color(1.0f, 0.98f, 0.75f, 0.5f), width);
                Tube(start, end, right, up, new Color(1.0f, 0.98f, 0.75f, 0.5f), width * 0.5f);
                
                LightData last = path.Light.Last;

                if (path.Light.Continuously) {
                    Vector3 mDirection = Vector3.Lerp(forward, last.Direction, 0.5f);
                    Plane mPlane = new Plane(mDirection, start);

                    int sIndex = path.Index * 16;
                    AdjustVertices(sIndex, forward, mPlane);
                    AdjustVertices(sIndex + 8, forward, mPlane);
                    
                    if (last.Path == null) continue;
                    
                    int eIndex = last.Path.Index * 16 + 4;
                    AdjustVertices(eIndex, last.Direction, mPlane);
                    AdjustVertices(eIndex + 8, last.Direction, mPlane);
                    ConnectFaces(sIndex, eIndex);
                    ConnectFaces(sIndex + 8, eIndex + 8);
                }
                else if (last != null) {
                    Plane startPlane = new Plane(path.StartNormal, start);
                    int sIndex = path.Index * 16;
                    int eIndex = last.Path.Index * 16 + 4;
                    
                    AdjustVertices(sIndex, forward, startPlane);
                    AdjustVertices(sIndex + 8, forward, startPlane);
                    AdjustVertices(eIndex, last.Direction, startPlane);
                    AdjustVertices(eIndex + 8, last.Direction, startPlane);
                }
            }
        }

        private void ConnectFaces(int start, int end) {
            int sShift = start;
            int eShift = end;
            for (int i = 0; i < 4; i++, sShift++, eShift++) {
                Vector3 center = Vector3.Lerp(_vertices[sShift], _vertices[eShift], 0.5f);
                _vertices[sShift] = _vertices[eShift] = center;
            }
        }

        private void AdjustVertices(int index, Vector3 direction, Plane plane) {
            int shifted = index;
            for (int i = 0; i < 4; i++, shifted++) {
                _vertices[shifted] = Attach(_vertices[shifted], direction, plane);
            }
        }

        private void AdjustEnd(int index, Vector3 direction, Plane plane) {
            int shifted = index + 4;
            for (int i = 0; i < 4; i++, shifted ++) {
                _vertices[shifted] = Attach(_vertices[shifted], direction, plane);
            }
        }

        private void Tube(Vector3 start, Vector3 end, Vector3 right, Vector3 up, Color color, float f) {
            right *= f;
            up *= f;
            var _000 = start + right + up;
            var _001 = start - right + up;
            var _010 = start + right - up;
            var _011 = start - right - up;
            var _100 = end + right + up;;
            var _101 = end - right + up;;
            var _110 = end + right - up;;
            var _111 = end - right - up;;
            
            int idx = _vertices.Count;
            
            _vertices.Add(_000);
            _vertices.Add(_001);
            _vertices.Add(_010);
            _vertices.Add(_011);
            _vertices.Add(_100);
            _vertices.Add(_101);
            _vertices.Add(_110);
            _vertices.Add(_111);
            
            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(1, 1));

            for (int i = 0; i < 8; i++) {
                _colors.Add(color);
            }
            
            _triangles.Add(idx);       // _000
            _triangles.Add(idx + 1);   // _001
            _triangles.Add(idx + 4);   // _100
            _triangles.Add(idx + 4);   // _100
            _triangles.Add(idx + 1);   // _001
            _triangles.Add(idx + 5);   // _101

            _triangles.Add(idx + 1);   // _001
            _triangles.Add(idx + 3);   // _011
            _triangles.Add(idx + 5);   // _101
            _triangles.Add(idx + 5);   // _101
            _triangles.Add(idx + 3);   // _011
            _triangles.Add(idx + 7);   // _111

            _triangles.Add(idx + 3);   // _011
            _triangles.Add(idx + 2);   // _010
            _triangles.Add(idx + 7);   // _111
            _triangles.Add(idx + 7);   // _111
            _triangles.Add(idx + 2);   // _010
            _triangles.Add(idx + 6);   // _110

            _triangles.Add(idx + 2);   // _010
            _triangles.Add(idx);       // _000
            _triangles.Add(idx + 6);   // _110
            _triangles.Add(idx + 6);   // _110
            _triangles.Add(idx);       // _000
            _triangles.Add(idx + 4);   // _100
        }

        private Vector3 PointOnPlane(Vector3 point, Vector3 direction, Plane plane) {
            return point;
        }

        private Vector3 Attach(Vector3 point, Vector3 direction, Plane plane) {
            Ray ray = new Ray(point, direction);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }
        
        private void GenerateMesh_(List<LightPath> paths) {
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();
            
            foreach (LightPath path in paths) {
                Vector3 start = path.Start;
                Vector3 end = path.End;

                Vector3 forward = path.Light.Direction;
                
                Vector3 right = Vector3.Cross(forward, Vector3.up).normalized;
                Vector3 up = Vector3.Cross(forward, right).normalized;
                
                if (right.magnitude < 0.1f) {
                    right = Vector3.Cross(forward, Vector3.forward).normalized;
                    up = Vector3.Cross(forward, right).normalized;
                }

                right *= width;
                up *= width;

                Vector3[] directions = {
                    up,
                    right,
                    -up,
                    -right
                };

                for (int face = 0; face < 4; face++) {
                    Vector3 offset = directions[face];
                    Vector3 u = directions[(face + 1) % 4];
                
                    _vertices.Add(start + offset + u);
                    _vertices.Add(start - offset + u);
                    _vertices.Add(end + offset + u);
                    _vertices.Add(end - offset + u);
                    
                    int idx = face * 4;
                    
                    _uvs.Add(new Vector2(0, 0));
                    _uvs.Add(new Vector2(0, 0));
                    _uvs.Add(new Vector2(1, 1));
                    _uvs.Add(new Vector2(1, 1));

                    _triangles.Add(idx);
                    _triangles.Add(idx + 1);
                    _triangles.Add(idx + 2);
                    _triangles.Add(idx + 2);
                    _triangles.Add(idx + 1);
                    _triangles.Add(idx + 3);
                }
            }
        }

        private void GenerateMeshData(List<Vector3> path) {
            _vertices.Clear();
            _triangles.Clear();

            for (int i = 0; i < path.Count - 1; i++) {
                Vector3 start = path[i];
                Vector3 end = path[i + 1];
                Vector3 forward = (end - start).normalized;

                Vector3 up = Vector3.Cross(forward, Vector3.up).normalized;
                Vector3 right = Vector3.Cross(forward, up).normalized;

                if (up.magnitude < 0.1f) {
                    up = Vector3.Cross(forward, Vector3.forward).normalized;
                    right = Vector3.Cross(forward, up).normalized;
                }

                Vector3[] directions = {
                    up,
                    right,
                    -up,
                    -right
                };

                for (int face = 2; face < 3; face++) {
                    Vector3 offset = directions[face] * width;
                    Vector3 u = directions[(face + 1) % 4] * width;
                
                    int idx = _vertices.Count;
                
                    _vertices.Add(start + offset + u);
                    _vertices.Add(start - offset + u);
                    _vertices.Add(end + offset + u);
                    _vertices.Add(end - offset + u);

                    _triangles.Add(idx);
                    _triangles.Add(idx + 1);
                    _triangles.Add(idx + 2);
                    _triangles.Add(idx + 2);
                    _triangles.Add(idx + 1);
                    _triangles.Add(idx + 3);
                }
            }
        }

        private void UpdateMesh() {
            _mesh.Clear();
            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
            _mesh.uv = _uvs.ToArray();
            _mesh.colors = _colors.ToArray();
            _mesh.RecalculateBounds();
        }
    }
}