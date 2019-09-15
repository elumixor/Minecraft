using System;
using System.Collections.Generic;

namespace Shared.SpaceWrapping {
    /// <summary>
    /// Class for wrapping 3D space into 1D and vice versa
    /// </summary>
    public static class Wrapper {
        /// <summary>
        /// Storage for <see cref="Wrap"/> ((x, y, z) -> index) requests
        /// </summary>
        private static readonly SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, ulong>>> IndexCacher =
            new SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, ulong>>>();

        /// <summary>
        /// Storage for <see cref="Unwrap"/> (index -> (x, y, z)) requests
        /// </summary>
        private static readonly SortedDictionary<ulong, (int x, int y, int z)> PositionCacher =
            new SortedDictionary<ulong, (int x, int y, int z)>();

        private static readonly Dictionary<uint, ulong> BaseCacher = new Dictionary<uint, ulong>();

        // todo: change algorithm and optimize (un-)wrapping, perhaps less casting

        public static ulong Wrap(int x, int y, int z) {
            if (!IndexCacher.TryGetValue(x, out var yDict)) {
                var zDict = (IndexCacher[x] = new SortedDictionary<int, SortedDictionary<int, ulong>>())
                    [y] = new SortedDictionary<int, ulong>();
                return zDict[z] = WrapFunction(x, y, z);
            } else {
                if (!yDict.TryGetValue(y, out var zDict))
                    return (yDict[y] = new SortedDictionary<int, ulong>())[z] = WrapFunction(x, y, z);

                return zDict.TryGetValue(z, out var result) ? result : zDict[z] = WrapFunction(x, y, z);
            }
        }
        public static void Unwrap(ulong i, out int x, out int y, out int z) {
            if (!PositionCacher.TryGetValue(i, out var position)) {
                UnwrapFunction(i, out x, out y, out z);
                PositionCacher[i] = (x, y, z);
            } else {
                x = position.x;
                y = position.y;
                z = position.z;
            }
        }


        private static ulong WrapFunction(int x, int y, int z) {
            if (x == 0 && y == 0) return (ulong) (z <= 0 ? z * -6 : z * 6 - 1);

            if (y < 0 && x >= 0) return WrapFunction(-y, x, z) + 3;
            if (x < 0 && y <= 0) return WrapFunction(-x, -y, z) + 2;
            if (y > 0 && x <= 0) return WrapFunction(y, -x, z) + 1;

            return WrapQuadrant(x, y, z) * 6 - 5;
        }

        public static void UnwrapFunction(ulong i, out int x, out int y, out int z) {
            if (i == 0) {
                x = z = y = 0;
                return;
            }

            var remainder = i % 6;
            if (remainder == 0) {
                x = y = 0;
                z = -(int) (i / 6);
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
                x = -(int) qY;
                y = (int) qX;
            } else if (remainder == 3) {
                x = -(int) qX;
                y = -(int) qY;
            } else if (remainder == 4) {
                x = (int) qY;
                y = -(int) qX;
            } else {
                x = (int) qX;
                y = (int) qY;
            }
        }

        private static void CoordsByIndexInBranch(ulong i, uint branch, out uint x, out uint y, out int z) {
            var straightIndex = branch * branch;
            var nextIndex = (branch + 1) * (branch + 1);
            uint branchY;
            ulong diff;
            if (i < nextIndex) {
                branchY = (uint) i.Sqrt();
                x = branch;
                diff = i - branchY * branchY;
            } else {
                var times = (uint) (i - straightIndex) / (2 * branch + 1);
                x = branch - times;
                branchY = branch;
                diff = i - straightIndex - times * (2 * branch + 1);
            }


            if (diff % 2 == 1) {
                z = (int) ((diff + 1) / 2);
                y = (uint) (branchY - z);
            } else {
                z = -(int) ((diff + 1) / 2);
                y = (uint) (branchY + z);
            }
        }

        private static uint BranchForI(ulong i, out ulong baseValue) {
            var baseX = 1u;
            while (i >= BaseByX(baseX)) baseX++;
            baseX -= 1;
            baseValue = BaseByX(baseX);
            return baseX;
        }

        private static ulong WrapQuadrant(int x, int y, int z) {
            if (z != 0) {
                var zAbs = Math.Abs(z);
                return WrapQuadrant(x, y + zAbs, 0) + (ulong) (2 * zAbs - (z > 0 ? 1 : 0));
            }

            if (y > x) return WrapQuadrant(y, y, 0) + YTurnOffset(y, y - x);
            if (y > 0) return WrapQuadrant(x, 0, 0) + YOffset(y);

            return BaseByX((uint) x);
        }

        private static ulong BaseByX(uint x) => BaseCacher.ContainsKey(x)
            ? BaseCacher[x]
            : BaseCacher[x] = x * x * (x - 1) + 1;

        private static ulong YOffset(int y) => (ulong) (y * y);
        private static ulong YTurnOffset(int y, int d) => (ulong) ((2 * y + 1) * d);
    }
}