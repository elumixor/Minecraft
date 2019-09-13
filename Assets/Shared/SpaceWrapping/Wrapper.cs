using System;
using System.Collections.Generic;

namespace Shared.SpaceWrapping {
    /// <summary>
    /// Class for wrapping 3D space into 1D and vice versa
    /// </summary>
    public static class Wrapper {
        private static readonly Dictionary<int, int> BaseDictionary = new Dictionary<int, int>();

        // todo: change algorithm and optimize (un-)wrapping

        // todo: should return ulong

        public static long Wrap(int x, int y, int z) {
            if (x == 0 && y == 0) return z <= 0 ? z * -6 : z * 6 - 1;

            if (y < 0 && x >= 0) return Wrap(-y, x, z) + 3;
            if (x < 0 && y <= 0) return Wrap(-x, -y, z) + 2;
            if (y > 0 && x <= 0) return Wrap(y, -x, z) + 1;

            return WrapQuadrant(x, y, z) * 6 - 5;
        }

        public static void Unwrap(long i, out int x, out int y, out int z) {
            if (i == 0) {
                x = z = y = 0;
                return;
            }

            var remainder = i % 6;
            if (remainder == 0) {
                x = y = 0;
                z = (int) (-i / 6);
                return;
            }

            if (remainder == 5) {
                x = y = 0;
                z = (int) ((i + 1) / 6);
                return;
            }

            var qI = (i + 5) / 6;
            var baseX = BranchForI(qI, out var baseValue);
            CoordsByIndexInBranch(qI - baseValue, baseX, out var qX, out var qY, out z);
            if (remainder == 2) {
                x = -qY;
                y = qX;
            } else if (remainder == 3) {
                x = -qX;
                y = -qY;
            } else if (remainder == 4) {
                x = qY;
                y = -qX;
            } else {
                x = qX;
                y = qY;
            }
        }

        private static void CoordsByIndexInBranch(long i, int branch, out int x, out int y, out int z) {
            var straightIndex = branch * branch;
            var nextIndex = (branch + 1) * (branch + 1);
            int branchY;
            long diff;
            if (i < nextIndex) {
                branchY = (int) i.Sqrt();
                x = branch;
                diff = i - branchY * branchY;
            } else {
                var times = (i - straightIndex) / (2 * branch + 1);
                x = (int) (branch - times);
                branchY = branch;
                diff = i - straightIndex - times * (2 * branch + 1);
            }


            if (diff % 2 == 1) {
                z = (int) ((diff + 1) / 2);
                y = branchY - z;
            } else {
                z = (int) (-(diff + 1) / 2);
                y = branchY + z;
            }
        }

        private static int BranchForI(long i, out int baseValue) {
            var baseX = 1;
            while (i >= BaseByX(baseX)) baseX++;
            baseX -= 1;
            baseValue = BaseByX(baseX);
            return baseX;
        }

        private static long WrapQuadrant(int x, int y, int z) {
            if (z != 0) {
                var zAbs = Math.Abs(z);
                return WrapQuadrant(x, y + zAbs, 0) + 2 * zAbs - (z > 0 ? 1 : 0);
            }

            if (y > x) return WrapQuadrant(y, y, 0) + YTurnOffset(y, y - x);
            if (y > 0) return WrapQuadrant(x, 0, 0) + YOffset(y);

            return BaseByX(x);
        }

        private static int BaseByX(int x) => BaseDictionary.ContainsKey(x)
            ? BaseDictionary[x]
            : BaseDictionary[x] = x * x * (x - 1) + 1;

        private static int YOffset(int y) => y * y;
        private static int YTurnOffset(int y, int d) => (2 * y + 1) * d;
    }
}