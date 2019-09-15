using System.Collections;
using NUnit.Framework;
using Shared.Positioning;
using Shared.SpaceWrapping;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Shared.Tests.Position {
    public class LocalPositionTests {
        [Test]
        public void CheckingPositions() {
            for (var z = 0u; z < WorldPosition.ChunkSize; z++)
            for (var y = 0u; y < WorldPosition.ChunkSize; y++)
            for (var x = 0u; x < WorldPosition.ChunkSize; x++) {
                var lp = new WorldPosition.LocalPosition(x, y, z);
                Assert.AreEqual(lp.x, x);
                Assert.AreEqual(lp.y, y);
                Assert.AreEqual(lp.z, z);

                Assert.AreEqual(lp, new WorldPosition.LocalPosition(lp.Index));
            }
        }
    }
}