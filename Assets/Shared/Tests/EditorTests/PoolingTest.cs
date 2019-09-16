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
            NUnit.Framework.Assert.Catch<NullReferenceException>(() => Pooler.Request("someTag"));

            var instance = UnityEngine.Object.FindObjectOfType<Pooler>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(UnityEngine.Object.FindObjectsOfType<Pooler>().Length, 1);
        }
    }
}