using UnityEngine;

namespace Phos.Navigate {
    public class NavigateOperation {
        public BaseNode Node { get; }
        public Vector3 Offset { get; }
        public Vector3 Target { get; }
        public Vector3 Up { get; }
        public bool IsTeleport { get; }
        public float Speed { get; }

        public NavigateOperation(BaseNode node, bool isTeleport = false, float speed = .1f) {
            Node = node;
            Target = node.GetNodePoint();
            Up = node.transform.up;
            IsTeleport = isTeleport;
            Speed = speed;
        }

        public NavigateOperation(Transform transform) {
            Target = transform.position;
            Up = transform.up;
            IsTeleport = false;
            Speed = 1;
        }

        public static void Interpolate(
            NavigateOperation src,
            NavigateOperation dst,
            Transform transform,
            float progress
        ) {
            if (dst.IsTeleport) {
                transform.position = dst.Target;
                return;
            }

            float delta = Mathf.Clamp01(progress / 1f);
            transform.position = Vector3.Lerp(src.Target, dst.Target, delta);

            if (!src.Up.Equals(dst.Up)) {
                transform.up = Vector3.Lerp(src.Up, dst.Up, delta);
            }
        }
    }
}