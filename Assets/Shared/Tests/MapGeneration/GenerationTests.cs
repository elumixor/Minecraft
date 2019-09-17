using System;
using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using Shared.Blocks;
using Shared.GameManagement;
using Shared.Positioning;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;
using Debug = UnityEngine.Debug;
using NAssert = NUnit.Framework.Assert;

namespace Shared.Tests.MapGeneration {
    public class GenerationTests {
        private const float FrameTime = 1f / 60 * 1000;

        private static Stopwatch sw;

        [SetUp]
        public void Setup() => sw = new Stopwatch();
        private static void LogResults() {
            var ts = sw.Elapsed;
            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Debug.Log("RunTime " + elapsedTime);
        }

        [UnityTest]
        public IEnumerator RockChunkGeneration() {
            sw.Start();
            var chunk = Map.GetChunk(new WorldPosition.ChunkPosition(0, 0, 0));
            sw.Stop();
            LogResults();
//            NAssert.Less(elapsed, FrameTime);

            yield return null;


            foreach (var (index, data) in chunk) {
                Assert.AreEqual(data.blockType, BlockType.Rock);
                var lp = new WorldPosition.LocalPosition(index);

//                Debug.Log(lp + " was " + data.adjoiningBlocks);
                if (lp.x * lp.y * lp.z == 0 ||
                    lp.x == WorldPosition.ChunkSize - 1 ||
                    lp.y == WorldPosition.ChunkSize - 1 ||
                    lp.z == WorldPosition.ChunkSize - 1)
                    NAssert.Less(data.adjoiningBlocks, 6);
                else NAssert.AreEqual(data.adjoiningBlocks, 6);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator SimpleChunkGeneration() {
            sw.Start();
            var chunk = Map.GetChunk(new WorldPosition.ChunkPosition(0, 1, 0));
            sw.Stop();
            LogResults();

            yield return null;
        }
    }
}