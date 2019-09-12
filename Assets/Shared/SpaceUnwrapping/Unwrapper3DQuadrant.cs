using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Shared.SpaceUnwrapping {
    public class Unwrapper3DQuadrant : Unwrapper {
        protected override void Unwrap(int i, out int x, out int y, out int z) {
            if (i == 0) {
                x = z = y = 0;
                return;
            }

            var baseX = BranchForI(i, out var baseValue);
            CoordsByIndexInBranch(i - baseValue, baseX, out x, out y, out z);
        }

        protected override int Wrap(int x, int y, int z) {
            if (z != 0) return Wrap(x, y + z, 0) + z;
            if (y < 0) return Wrap(x, -y, 0) + 1;
            if (y > x) return Wrap(y, y, 0) + YTurnOffset(y, y - x);
            if (y > 0) return Wrap(x, 0, 0) + YOffset(y);

            return BaseByX(x);
        }

        private static readonly Dictionary<int, int> BaseDictionary = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> BranchDictionary = new Dictionary<int, int>();

        private static int P(int y) {
            Debug.Assert(y >= 0, "y >= 0");
            return 2 * y + 1;
        }
        private static int NumberOfPointsAtBranch(int x) => BranchDictionary.ContainsKey(x)
            ? BranchDictionary[x]
            : BranchDictionary[x] = 3 * x * x + x;

        private static int BaseByX(int x) => BaseDictionary.ContainsKey(x)
            ? BaseDictionary[x]
            : BaseDictionary[x] = x * x * (x - 1) + 1;

        private static int BranchForI(int i) {
            Debug.Assert(i >= 0, "i >= 0");
            var baseX = 1;
            while (i >= BaseByX(baseX)) baseX++;
            return baseX - 1;
        }

        private static void CoordsByIndexInBranch(int i, int branch, out int x, out int y, out int z) {
            var straightIndex = branch * branch;
            var nextIndex = (branch + 1) * (branch + 1);
            int branchY;
            int diff;
            if (i < nextIndex) {
                branchY = Sqrt(i);
                x = branch;
                diff = i - branchY * branchY;
            } else {
                var times = (i - straightIndex) / (2 * branch + 1);
                x = branch - times;
                branchY = branch;
                diff = i - straightIndex - times * (2 * branch  + 1);
            }


            if (diff % 2 == 1) {
                z = (diff + 1) / 2;
                y = branchY - z;
            } else {
                z = -(diff + 1) / 2;
                y = branchY + z;
            }
        }

        private static int BranchForI(int i, out int baseValue) {
            Debug.Assert(i >= 0, "i >= 0");
            var baseX = 1;
            while (i >= BaseByX(baseX)) baseX++;
            baseX -= 1;
            baseValue = BaseByX(baseX);
            return baseX;
        }

        private static int YOffset(int y) => (y * y + y) / 2;
        private static int YTurnOffset(int y, int d) => (y + 1) * d;
    }
}