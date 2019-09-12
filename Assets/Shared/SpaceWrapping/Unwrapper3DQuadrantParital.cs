using System.Collections.Generic;

namespace Shared.SpaceWrapping {
    public class Unwrapper3DQuadrantParital : Unwrapper {
        protected override void Unwrap(int i, out int x, out int y, out int z) {
            if (i == 0) {
                x = z = y = 0;
                return;
            }

            var baseX = 1;
            while (i >= BaseByX(baseX)) baseX++;
            baseX -= 1;
            var baseValue = BaseByX(baseX);
            var inBranch = i - baseValue;

            var maxStepsInBranch = baseX + baseX - 1;

            var steps = 0;
            var stepValue = 0;

            while (inBranch >= stepValue) {
                if (steps > maxStepsInBranch) break;
                steps++;
                stepValue += steps <= baseX ? steps : (baseX + 1);
            }

            stepValue -= steps <= baseX ? steps : (baseX + 1);
            steps -= 1;

            if (steps > baseX) {
                x = baseX - (steps - baseX);
                y = baseX;
            } else {
                x = baseX;
                y = steps;
            }

            z = i - (stepValue + baseValue);
            y -= z;
        }

        protected override int Wrap(int x, int y, int z) {
            if (z != 0) return Wrap(x, y + z, 0) + z;
            if (y > x) return Wrap(y, y, 0) + YTurnOffset(y, y - x);
            if (y > 0) return Wrap(x, 0, 0) + YOffset(y);
            return BaseByX(x);
        }

        private static readonly Dictionary<int, int> BaseDictionary = new Dictionary<int, int>();

        private static int BaseByX(int x) =>
            BaseDictionary.ContainsKey(x)
                ? BaseDictionary[x]
                : BaseDictionary[x] = (x * x * x - x + 2) / 2;

        private static int YOffset(int y) => (y * y + y) / 2;
        private static int YTurnOffset(int y, int d) => (y + 1) * d;
    }
}