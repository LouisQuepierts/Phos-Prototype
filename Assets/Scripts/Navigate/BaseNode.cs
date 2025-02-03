
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Phos.Navigate {
    public abstract class BaseNode : MonoBehaviour {
        public const float BLOCK_SCALE = 0.5f;

        public Type type = Type.GROUND;
        public float offset = 0.0f;

        public Vector3 GetNodePoint() {
            return type switch {
                Type.GROUND => transform.position + transform.up * (offset + 0.5f),
                Type.STAIR => transform.position + transform.up * offset,
                Type.LADDER => transform.position + transform.forward * (offset + 0.5f),
                _ => throw new NotImplementedException(),
            };
        }

        public Vector3 GetLocalOffset() {
            return type switch {
                Type.GROUND => Vector3.up * (offset + 0.5f),
                Type.STAIR => Vector3.up * offset,
                Type.LADDER => Vector3.forward * (offset + 0.5f),
                _ => throw new NotImplementedException(),
            };
        }

        public Vector3 GetRelativeConnectPoint(Direction direction, float offset = 0.0f) {
            float mul = (BLOCK_SCALE + offset);
            return type switch {
                Type.GROUND => direction switch {
                    Direction.Forward => transform.forward * mul,
                    Direction.Backward => transform.forward * -mul,
                    Direction.Right => transform.right * mul,
                    Direction.Left => transform.right * -mul,
                    _ => throw new NotImplementedException()
                },
                Type.STAIR => direction switch {
                    Direction.Forward => (transform.forward + transform.up) * mul,
                    Direction.Backward => (transform.forward + transform.up) * -mul,
                    Direction.Right => transform.right * mul,
                    Direction.Left => transform.right * -mul,
                    _ => throw new NotImplementedException()
                },
                Type.LADDER => direction switch {
                    Direction.Forward => transform.up * mul,
                    Direction.Backward => transform.up * -mul,
                    Direction.Right => transform.right * mul,
                    Direction.Left => transform.right * -mul,
                    _ => throw new NotImplementedException()
                },
                _ => throw new NotImplementedException(),
            };
        }

        public Vector3 GetLocalConnectPoint(Direction direction, float offset = 0.0f) {
            float mul = (BLOCK_SCALE + offset);
            return type switch {
                Type.GROUND => direction switch {
                    Direction.Forward => Vector3.forward * mul,
                    Direction.Backward => Vector3.forward * -mul,
                    Direction.Right => Vector3.right * mul,
                    Direction.Left => Vector3.right * -mul,
                    _ => throw new NotImplementedException()
                },
                Type.STAIR => direction switch {
                    Direction.Forward => (Vector3.forward + Vector3.up) * mul,
                    Direction.Backward => (Vector3.forward + Vector3.up) * -mul,
                    Direction.Right => Vector3.right * mul,
                    Direction.Left => Vector3.right * -mul,
                    _ => throw new NotImplementedException()
                },
                Type.LADDER => direction switch {
                    Direction.Forward => Vector3.up * mul,
                    Direction.Backward => Vector3.up * -mul,
                    Direction.Right => Vector3.right * mul,
                    Direction.Left => Vector3.right * -mul,
                    _ => throw new NotImplementedException()
                },
                _ => throw new NotImplementedException(),
            } + GetLocalOffset();
        }

        public Vector3 GetConnectPoint(Direction direction, float offset = 0.0f) {
            return GetNodePoint() + GetRelativeConnectPoint(direction, offset);
        }

        [System.Serializable]
        public enum Type {
            GROUND,
            STAIR,
            LADDER
        }
    }
}
