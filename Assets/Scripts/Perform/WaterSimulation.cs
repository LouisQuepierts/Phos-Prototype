using System;
using UnityEngine;

namespace Phos.Perform {
    public class WaterSimulation : MonoBehaviour {
        private static readonly int Wave1 = Shader.PropertyToID("_Wave1");
        private static readonly int Wave2 = Shader.PropertyToID("_Wave2");
        private static readonly int Wave3 = Shader.PropertyToID("_Wave3");
        private static readonly int Wave4 = Shader.PropertyToID("_Wave4");

        public float length;
        public float width;
        public float depth;
        public Material waterMaterial;
        
        private Wave[] _waves = new Wave[4];
        
        private void Start() {
            if (!waterMaterial) {
                enabled = false;
                return;
            }

            length = Mathf.Max(1, length);
            width = Mathf.Max(1, width);
        }

        private void FixedUpdate() {
            var position = transform.position;
            float minX = position.x - length / 2f;
            float maxX = position.x + length / 2f;
            float minZ = position.z - width / 2f;
            float maxZ = position.z + width / 2f;
            
            Vector3 ll = new Vector3(minX, position.y, minZ);
            Vector3 lr = new Vector3(maxX, position.y, minZ);
            Vector3 ur = new Vector3(maxX, position.y, maxZ);
            Vector3 ul = new Vector3(minX, position.y, maxZ);
            
            ll.y = CalculateHeight(ll, Time.time);
            lr.y = CalculateHeight(lr, Time.time);
            ur.y = CalculateHeight(ur, Time.time);
            ul.y = CalculateHeight(ul, Time.time);
            
            float centerY = (ll.y + lr.y + ur.y + ul.y) / 4f;
            Vector3 normal = Vector3.Cross(lr - ll, ul - ll);
            
            transform.position = new Vector3(position.x, centerY - depth, position.z);
            transform.rotation = Quaternion.LookRotation(transform.forward, normal);
        }

        private void OnDrawGizmosSelected() {
            if (!Application.isEditor) return;
            
            var position = transform.position;
            position.y -= depth;
            float minX = position.x - length / 2f;
            float maxX = position.x + length / 2f;
            float minZ = position.z - width / 2f;
            float maxZ = position.z + width / 2f;
            
            Vector3 ll = new Vector3(minX, position.y, minZ);
            Vector3 lr = new Vector3(maxX, position.y, minZ);
            Vector3 ur = new Vector3(maxX, position.y, maxZ);
            Vector3 ul = new Vector3(minX, position.y, maxZ);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ll, lr);
            Gizmos.DrawLine(lr, ur);
            Gizmos.DrawLine(ur, ul);
            Gizmos.DrawLine(ul, ll);
        }

        private float CalculateHeight(Vector3 position, float time) {
            float height = 0;
            _waves[0] = Wave.FromVector4(waterMaterial.GetVector(Wave1));
            _waves[1] = Wave.FromVector4(waterMaterial.GetVector(Wave2));
            _waves[2] = Wave.FromVector4(waterMaterial.GetVector(Wave3));
            _waves[3] = Wave.FromVector4(waterMaterial.GetVector(Wave4));
            foreach (var wave in _waves) {
                height += wave.CalculateHeight(position, time);
            }
            return height / 4f;
        }
    }

    public class Wave {
        private readonly float _amplitude;
        private readonly float _frequency;
        private readonly float _speed;
        private readonly Vector2 _direction;

        private Wave(float amplitude, float frequency, float speed, Vector2 direction) {
            _amplitude = amplitude;
            _frequency = frequency;
            _speed = speed;
            _direction = direction;
        }
        
        public float CalculateHeight(Vector3 position, float time) {
            var dotXZ = Vector2.Dot(_direction, new Vector2(position.x, position.z));
            return _amplitude * Mathf.Sin((dotXZ + time * _speed) * _frequency);
        }

        public static Wave FromVector4(Vector4 vector4) {
            float amplitude = vector4.x;
            float frequency = 2f / Mathf.Max(vector4.y, 1e-6f);
            float speed = vector4.z;
            float radians = Mathf.Deg2Rad * vector4.w;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            return new Wave(amplitude, frequency, speed, direction);
        }
    }
}