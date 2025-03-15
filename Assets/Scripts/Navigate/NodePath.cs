using Phos.Callback;
using Phos.Predicate;
using Phos.Structure;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Phos.Navigate {
    [Serializable]
    public class NodePath {
        public NavigateNode nodeA;
        public NavigateNode nodeB;

        public Direction directionA;
        public Direction directionB;

        public bool active;
        public bool neighbor;

        public VirtualNode router;

#if UNITY_EDITOR
        [NonSerialized, HideInInspector]
        public bool foldout = false;
        [NonSerialized, HideInInspector]
        public bool showPredicates = false;
#endif

        public NodePath(NavigateNode nodeA, NavigateNode nodeB, Direction directionA, Direction directionB, bool active, bool neighbor) {
            this.nodeA = nodeA;
            this.nodeB = nodeB;
            this.directionA = directionA;
            this.directionB = directionB;
            this.active = true;
            this.neighbor = neighbor;
        }

        public Direction GetDirection(NavigateNode node) {
            if (node == nodeA) return directionA;
            return node == nodeB ? directionB : Direction.Forward;
        }

        public void SetDirection(NavigateNode node, Direction direction) {
            if (node == nodeA) directionA = direction;
            if (node == nodeB) directionB = direction;
        }
        public NavigateNode GetOther(NavigateNode node) {
            return node == nodeA ? nodeB : nodeA;
        }

        public bool IsRelated(NavigateNode node) {
            return node == nodeA || node == nodeB;
        }
    }
}