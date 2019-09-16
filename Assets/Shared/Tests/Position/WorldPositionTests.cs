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

            var wp = (WorldPosition) Vector3.left;

            var (chunkPosition, localPosition) = (WorldPosition) Vector3.right;

            Assert.AreEqual(localPosition, WorldPosition.LocalPosition.Right);
            Assert.AreEqual(chunkPosition, WorldPosition.ChunkPosition.Zero);

            var (outChunkPosition, outLocalPosition) = (WorldPosition) Vector3.left;

            Assert.AreEqual(outLocalPosition, new WorldPosition.LocalPosition(WorldPosition.ChunkSize - 1, 0,0));
            Assert.AreEqual(outChunkPosition, WorldPosition.ChunkPosition.Left);


            var v0 = new Vector3(-19.8f, 0f, 0f);
            var (chunk0, local0) = (WorldPosition) v0;

            Assert.AreEqual(chunk0, new WorldPosition.ChunkPosition(-1, 0,0));
            Assert.AreEqual(local0, new WorldPosition.LocalPosition(0, 0,0));


            var v01 = new Vector3(-19.4f, 0f, 0f);
            var (chunk01, local01) = (WorldPosition) v01;

            Assert.AreEqual(chunk01, new WorldPosition.ChunkPosition(-1, 0,0));
            Assert.AreEqual(local01, new WorldPosition.LocalPosition(1, 0,0));


            var v011 = new Vector3(-0.3f, -0.3f, 0f);
            var (chunk011, local011) = (WorldPosition) v011;

            Assert.AreEqual(chunk011, new WorldPosition.ChunkPosition(0, 0,0));
            Assert.AreEqual(local011, new WorldPosition.LocalPosition(0, 0,0));

            var v = new Vector3(-2.3f, 0f, 0f);
            var (chunk, local) = (WorldPosition) v;

            Assert.AreEqual(chunk, new WorldPosition.ChunkPosition(-1, 0,0));
            Assert.AreEqual(local, new WorldPosition.LocalPosition(WorldPosition.ChunkSize - 2, 0,0));


            var v1 = new Vector3(-1.9f, 0f, -0.0003f);
            var (chunk1, local1) = (WorldPosition) v1;

            Assert.AreEqual(chunk1, new WorldPosition.ChunkPosition(-1, 0,0));
            Assert.AreEqual(local1, new WorldPosition.LocalPosition(WorldPosition.ChunkSize - 2, 0,0));
        }
    }
}