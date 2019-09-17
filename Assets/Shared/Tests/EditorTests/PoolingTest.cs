using System;
using System.Collections;
using NUnit.Framework;
using Shared.Pooling;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Shared.Tests.EditorTests {
    public class PoolingTest {
        // A Test behaves as an ordinary method
        [Test]
        public void PoolingTestSimplePasses() {
            // Use the Assert class to test conditions
            NUnit.Framework.Assert.Catch<NullReferenceException>(() => BlockPooler.Request());

            var instance = UnityEngine.Object.FindObjectOfType<BlockPooler>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(UnityEngine.Object.FindObjectsOfType<BlockPooler>().Length, 1);
        }
    }
}