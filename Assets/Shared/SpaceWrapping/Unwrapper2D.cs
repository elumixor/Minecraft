using System;

namespace Shared.SpaceWrapping {
    public class Unwrapper2D : Unwrapper {
        protected override void Unwrap(int i, out int x, out int y, out int z) {
            z = 0;
            if (i == 0) {
                x = y = 0;
                return;
            }

            var remainder = i % 4; // 0 -> 4th, 1 -> 1st, 2 -> 2nd, 3 -> 3rd
            var qI = i - (remainder == 0 ? 3 : remainder - 1);
            var branch = (Sqrt(qI) + 1) / 2;
            var branchBaseValue = (2 * branch - 1) * (2 * branch - 1);
            var steps = (qI - branchBaseValue) / 4;

            var hasTurned = steps > branch;

            var qY = hasTurned ? branch : steps;
            var qX = hasTurned ? branch - (steps - branch) : branch;

            x = remainder == 1 ? qX : remainder == 3 ? -qX : remainder == 2 ? -qY : qY;
            y = remainder == 1 ? qY : remainder == 3 ? -qY : remainder == 2 ? qX : -qX;
        }

        protected override int Wrap(int x, int y, int z) {
            if (x == 0 && y == 0) return 0;

            // Positions in first quadrant
            var qX = Math.Abs(x);
            var qY = Math.Abs(y);

            var baseI = 2 * qX - 1;
            return baseI * baseI + Math.Max(0, qY - qX) + qY + Quadrant(x, y);
        }

        private static int Quadrant(int x, int y) {
            if (x > 0 && y >= 0) return 0;
            if (y > 0 && x <= 0) return 1;
            if (x < 0 && y <= 0) return 2;
//          if (y < 0 && x >= 0)
            return 3;
        }
    }
}