using System;
using UnityEngine.InputSystem.Utilities;

namespace Phos.Navigate {
    [Serializable]
    public enum Direction {
        Forward, Backward, Right, Left
    }

    public static class Directions {
        public static readonly ReadOnlyArray<Direction> Value = new(new Direction[]{
            Direction.Forward, Direction.Backward, Direction.Right, Direction.Left
        });
    }

    public static class Extensions {
        public static Direction Opposite(this Direction direction) {
            return direction switch {
                Direction.Forward => Direction.Backward,
                Direction.Backward => Direction.Forward,
                Direction.Right => Direction.Left,
                Direction.Left => Direction.Right,
                _ => throw new NotImplementedException()
            };
        }
    }
}