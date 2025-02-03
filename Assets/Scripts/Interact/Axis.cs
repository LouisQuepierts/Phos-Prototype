using System;
using UnityEngine;

namespace Phos.Interact {
    [Serializable]
    public enum Axis {
        XP, XN,
        YP, YN,
        ZP, ZN
    }

    public static class AxisExtensions {
        public static Axis Opposite(this Axis axis) {
            return axis switch {
                Axis.XP => Axis.XN,
                Axis.YP => Axis.YN,
                Axis.ZP => Axis.ZN,
                Axis.XN => Axis.XP,
                Axis.YN => Axis.YP,
                Axis.ZN => Axis.ZP,
                _ => throw new NotImplementedException()
            };
        }

        public static Vector3 Direction(this Axis axis) {
            return axis switch {
                Axis.XP => Vector3.right,
                Axis.YP => Vector3.up,
                Axis.ZP => Vector3.forward,
                Axis.XN => Vector3.left,
                Axis.YN => Vector3.down,
                Axis.ZN => Vector3.back,
                _ => Vector3.zero
            };
        }

        public static Vector3 Direction(this Axis axis, Transform transform) {
            return axis switch {
                Axis.XP => transform.right,
                Axis.YP => transform.up,
                Axis.ZP => transform.forward,
                Axis.XN => -transform.right,
                Axis.YN => -transform.up,
                Axis.ZN => -transform.forward,
                _ => Vector3.zero
            };
        }
    }
}