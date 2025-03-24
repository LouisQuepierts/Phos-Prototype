using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Phos.Optical {
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class LightBeamMesh : MonoBehaviour {
        public Material material;

        private Mesh _mesh;
        private readonly List<Vector3> _vertices = new();
        private readonly List<int> _triangles = new();
        private readonly List<Vector2> _uvs = new();
        private readonly List<Color> _colors = new();

        private void Awake() {
            Init();
        }

        public void Init() {
            _mesh ??= new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            GetComponent<MeshRenderer>().material = material;
        }

        public void Reset() {
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();
            _colors.Clear();
            _mesh.Clear();
        }

        public void UpdatePath(List<LightPath> paths) {
            GenerateMesh(paths);
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

                float beginIntensity = Mathf.Sqrt(Mathf.Clamp01(path.Intensity / 100f));
                float endIntensity = Mathf.Sqrt(Mathf.Clamp01((path.Intensity - path.Distance) / 100f));
                
                // Debug.Log($"Intensity {beginIntensity} - {endIntensity}");

                Tube(path, right, up, new Color(1.0f, 0.98f, 0.75f, 0.75f), 1.0f, beginIntensity, endIntensity);
                Tube(path, right, up, new Color(1.0f, 0.98f, 0.75f, 0.75f), 0.5f, beginIntensity, endIntensity);
                
                LightData last = path.Light.Last;

                if (path.Light.IsContinuous) {
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

        private void Tube(
            LightPath path,
            Vector3 right, Vector3 up, 
            Color color, 
            float amplifier,
            float beginIntensity,
            float endIntensity
            ) {
            var start = path.Start;
            var end = path.End;

            var startRight = path.StartWidth * amplifier * right;
            var startUp = path.StartWidth * amplifier * up;
            var endRight = path.EndWidth * amplifier * right;
            var endUp = path.EndWidth * amplifier * up;
            
            var _000 = start + startRight + startUp;
            var _001 = start - startRight + startUp;
            var _010 = start + startRight - startUp;
            var _011 = start - startRight - startUp;
            var _100 = end + endRight + endUp;
            var _101 = end - endRight + endUp;
            var _110 = end + endRight - endUp;
            var _111 = end - endRight - endUp;
            
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

            Color beginColor = new Color(color.r, color.g, color.b, color.a * beginIntensity);
            Color endColor = new Color(color.r, color.g, color.b, color.a * endIntensity);
            for (int i = 0; i < 4; i++) {
                _colors.Add(beginColor);
            }
            for (int i = 0; i < 4; i++) {
                _colors.Add(endColor);
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