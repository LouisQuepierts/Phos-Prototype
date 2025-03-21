using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Navigate {
    public class NavigatePath : IEnumerable<NavigateOperation> {
        public static readonly NavigatePath Empty = new(new(), null, null);

        private readonly List<NavigateOperation> _moves;
        private BaseNode _dst;
        
        private BaseNode _last;
        private BaseNode _current;

        private int _index;

        public NavigatePath(List<NodePath> path, NavigateNode src, NavigateNode dst) {
            _index = 0;

            _moves = new();
            _last = src;
            _current = src;

            // check door
            var deep = 4;
            while (deep != 0 &&
                   dst &&
                   dst.type == NodeType.DOOR) {
                var direction = path[^1].GetDirection(dst);
                var node = dst.GetConnectedNode(direction.Opposite());
                if (node is not NavigateNode navi) {
                    break;
                }
                
                var next = dst.Paths[navi];
                dst = navi;
                path.Add(next);
                deep++;
            }
            _dst = dst;

            if (!src || !dst) return;

            var pool = PathManager.GetInstance().Pool;
            
            NavigateNode current = src;
            foreach (var item in path) {
                NavigateNode other = item.GetOther(current);
                if (item.router) {
                    if (item.router.Bound == current) {
                        _moves.Add(pool.Get().Init(item.router, item, true));
                        _moves.Add(pool.Get().Init(other, item));
                    } else {
                        _moves.Add(pool.Get().Init(item.router, item));
                        _moves.Add(pool.Get().Init(other, item, true));
                    }
                } 
                else if (!item.neighbor) {
                    _moves.Add(pool.Get().Init(current, item, false, item.GetDirection(current)));
                    _moves.Add(pool.Get().Init(other, item, true, item.GetDirection(other)));
                    _moves.Add(pool.Get().Init(other, item));
                }
                else {
                    _moves.Add(pool.Get().Init(current, item, false, item.GetDirection(current)));
                    _moves.Add(pool.Get().Init(other, item));
                }


                /*if (!item.neighbor) {
                        Vector3 adjust = current.GetNodePoint();
                        if (adjust.y != to.y) {
                            adjust.y = Mathf.Max(adjust.y, to.y);
                        }

                        if (_moves.Count > 0) {
                            _moves[^1].Target.Set(adjust.x, adjust.y, adjust.z);
                        } else {
                            _moves.Add(new MoveOpertaion(adjust, true, 1f));
                        }
                    }

                    _moves.Add(new MoveOpertaion(go, false, 0.2f));

                    if ((go - to).sqrMagnitude > 1e-6) {
                        _moves.Add(new MoveOpertaion(to, !item.neighbor, 0.2f));
                    }

                    _moves.Add(new MoveOpertaion(other.GetNodePoint(), false, 0.2f));*/
                current = other;
            }
        }

        public IEnumerator<NavigateOperation> GetEnumerator() {
            return _moves.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _moves.GetEnumerator();
        }

        public void Setup(BaseNode last) {
            _last = last;
        }

        public bool HasNext() {
            return _index < _moves.Count;
        }

        public NavigateOperation Next(Transform transform) {
            if (_index >= _moves.Count - 1) return Last();

            if ((transform.position - _moves[_index].Target).sqrMagnitude < 0.01f) {
                _moves[_index].Node.OnArrive(transform);
                _index++;
                //Debug.Log("Next");

                NavigateOperation current = _moves[_index];

                _last = _current;
                _current = current.Node;
            }

            return _moves[_index];
        }

        public void Move(PlayerController controller) {
            Transform transform = controller.transform;
            NavigateOperation operation = Next(transform);

            Vector3 target = operation.Target;
            if (operation.IsTeleport) {
                transform.position = target;
                return;
            }

            Vector3 delta = target - transform.position;

            if (Vector3.Dot(transform.forward, delta) < 1) {
                Vector3 forward = Vector3.ProjectOnPlane(delta, transform.up);
                transform.forward = forward;
            }

            controller.current?.PerformPassing(controller, operation, _last);
        }

        public bool Arrive(Transform transform) {
            _dst = Last().Node;
            for (int i = _index; i < _moves.Count; i++) {
                if (_moves[i].Path.active) continue;
                
                _dst = _moves[Mathf.Max(0, i - 1)].Node;
                break;
            }

            return (transform.position - _dst.GetNodePosition()).sqrMagnitude < 0.01f;
        }

        public NavigateOperation Last() {
            return _moves[^1];
        }
        
        public BaseNode Destination() {
            return _dst;
        }

        public BaseNode LastNode() {
            return _last;
        }

        public BaseNode CurrentNode() {
            return _current;
        }

        public void Free() {
            var pool = PathManager.GetInstance().Pool;
            foreach (var move in this._moves) {
                pool.Release(move);
            }
        }

        public int Count => _moves.Count;

        public NavigateOperation this[int index] => _moves[index];
    }
}