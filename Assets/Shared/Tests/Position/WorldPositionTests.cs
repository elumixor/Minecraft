using System.Collections;
using NUnit.Framework;
using Shared.Positioning;
using Shared.SpaceWrapping;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Shared.Tests.Position {
    public class WorldPositionTests {
        [UnityTest]
        public IEnumerator AddingLocalPositions() {
            for (var z = 0u; z < WorldPosition.ChunkSize; z++)
            for (var y = 0u; y < WorldPosition.ChunkSize; y++)
            for (var x = 0u; x < WorldPosition.ChunkSize; x++) {
                var startLocal = new WorldPosition.LocalPosition(x, y, z);
                Assert.AreEqual(startLocal, startLocal.Add(WorldPosition.LocalPosition.Zero, out var off1));
                Assert.AreEqual(off1, WorldPosition.ChunkPosition.Zero);

                var added = startLocal.Add(startLocal, out var off2);
            }

            yield return null;
        }

        [Test]
        public void ConversionFromVector3ToWorldPositionWorks() {
            foreach (var adjoiningVector in WorldPosition.AdjoiningVectors)
                Debug.Log((WorldPosition) (Vector3) adjoiningVector);

            var (chunkPosition, localPosition) = (WorldPosition) Vector3.right;

            Assert.AreEqual(localPosition, WorldPosition.LocalPosition.Right);
            Assert.AreEqual(chunkPosition, WorldPosition.ChunkPosition.Zero);

            var (outChunkPosition, outLocalPosition) = (WorldPosition) Vector3.left;

            Assert.AreEqual(outLocalPosition, new WorldPosition.LocalPosition(WorldPosition.ChunkSize - 1, 0,0));
            Assert.AreEqual(outChunkPosition, WorldPosition.ChunkPosition.Left);
        }
    }
}