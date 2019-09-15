using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Shared.SpaceWrapping;
using Assert = UnityEngine.Assertions.Assert;

namespace Shared.Tests.Wrapping
{
    public class WrappingTests
    {
        [UnityTest]
        public IEnumerator WrappingAndUnwrappingYieldsSameResults()        {
            for (var z = -50; z <= 50; z++)
            for (var y = -50; y <= 50; y++)
            for (var x = -50; x <= 50; x++) {
                var index = Wrapper.Wrap(x, y, z);
                Wrapper.Unwrap(index, out var vx, out var vy, out var vz);

                Assert.AreEqual(x, vx);
                Assert.AreEqual(y, vy);
                Assert.AreEqual(z, vz);

                yield return null;
            }
        }
    }
}
