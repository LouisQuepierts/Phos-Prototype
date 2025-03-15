using System;
using System.Collections.Generic;

namespace Phos.Navigate {
    [Serializable]
    public enum Direction {
        Forward, Backward, Right, Left, None
    }

    public static class Directions {
        public static readonly IReadOnlyList<Direction> Value = new List<Direction>(new Direction[]{
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
                _ => Direction.None
            };
        }
    }
}