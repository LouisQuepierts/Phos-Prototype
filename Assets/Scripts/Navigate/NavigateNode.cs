using System;
using UnityEngine;

namespace Phos.Navigate {
    [ExecuteInEditMode]
    public class NavigateNode : BaseNode {
        public static bool ShowGizmos = true;
        private static Vector3 SCALE = new(0.1f, 0.1f, 0.1f);

        [HideInInspector]
        public PathManager.PathList Paths {
            get; set;
        } = new();

        [NonSerialized]
        public bool accessable = false;

        private void OnEnable() {
            PathManager.TryGetInstance().Register(this);
        }

#if UNITY_EDITOR
        //public void Connect(NavigateNode other, Direction direction, bool neighbor = true) {
        //    PathManager controller = PathManager.TryGetInstance();
        //    if (controller == null) return;

        //    controller.Connect(this, other, direction, direction.Opposite(), neighbor);
        //}

        public bool TryConnect(NavigateNode other) {
            if (other == null || other == this) return false;

            PathManager controller = PathManager.TryGetInstance();
            if (controller == null) return false;

            float dot = Vector3.Dot(transform.up, other.transform.up);
            if (dot < 0) return false;

            foreach (Direction direction in AvailableDirections) {
                foreach (var opposite in other.AvailableDirections) {
                    Vector3 conA = this.GetConnectPoint(direction);
                    Vector3 conB = other.GetConnectPoint(opposite);

                    if (Vector3.Distance(conA, conB) < 0.1f) {
                        controller.Connect(this, other, direction, opposite);
                        return true;
                    }

                    conA = Camera.main.WorldToScreenPoint(conA);
                    conB = Camera.main.WorldToScreenPoint(conB);

                    if (Vector2.Distance(conA, conB) < 0.1f) {
                        controller.Connect(this, other, direction, opposite, false);
                        return true;
                    }
                }
            }

            return false;
        }

        public void Disconnect(NavigateNode other) {
            PathManager controller = PathManager.TryGetInstance();
            if (controller == null) return;

            controller.Disconnect(this, other);
        }

        public void ResetConnection() {
            if (PathManager.GetInstance() != null) {
                PathManager.GetInstance().DisconnectAll(this);
            }
        }

        private void OnDestroy() {
            if (Application.isPlaying) return;
            ResetConnection();
        }

        private void OnDrawGizmos() {
            if (!ShowGizmos) return;

            Vector3 center = GetNodePoint();
            Matrix4x4 matrix = Gizmos.matrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(center, 0.2f);

            if (accessable) {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(center, 0.1f);
            }

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.color = Color.blue;
            foreach (Direction direction in AvailableDirections) {
                Gizmos.DrawCube(GetLocalConnectPoint(direction, -0.1f), SCALE);
            }

            foreach (var path in Paths) {
                if (path == null || !path.active) continue;

                Gizmos.color = Color.green;
                Gizmos.DrawCube(GetLocalConnectPoint(path.GetDirection(this)), SCALE);
            }

            Gizmos.matrix = matrix;

            foreach (var path in Paths) {
                NavigateNode other = path.GetOther(this);
                Vector3 connect = GetConnectPoint(path.GetDirection(this));

                Gizmos.color = path.neighbor ? Color.blue : Color.magenta;

                Gizmos.DrawLine(center, connect);
                Gizmos.DrawLine(connect, other.GetConnectPoint(path.GetDirection(other)));
            }
        }

        public override BaseNode GetConnectedNode(Direction direction) {
            foreach (var item in Paths) {
                if (item.GetDirection(this) == direction) {
                    return item.GetOther(this);
                }
            }

            return null;
        }
#endif
    }
}