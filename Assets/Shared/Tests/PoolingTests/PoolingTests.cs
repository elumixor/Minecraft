using System;
using System.Collections;
using NUnit.Framework;
using Shared.Pooling;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;
using Object = UnityEngine.Object;

namespace Shared.Tests.PoolingTests {
    public class PoolingTests {
        [UnityTest]
        public IEnumerator SmokeTest() {
            Assert.IsNull(BlockPooler.Request());
            var instance = Object.FindObjectOfType<BlockPooler>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(instance.transform.childCount, 0);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SceneAwakeTest() {
            SceneManager.LoadScene("PoolingTestScene");
            yield return new WaitForSeconds(.1f);
            var instance = Object.FindObjectOfType<BlockPooler>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(instance.transform.childCount, 100);

            Assert.IsNotNull(BlockPooler.Request());

            yield return null;
        }


        [UnityTest]
        public IEnumerator MultipleScenes() {
            SceneManager.LoadScene("PoolingTestScene");
            yield return new WaitForSeconds(.1f);
            var instance = Object.FindObjectOfType<BlockPooler>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(instance.transform.childCount, 100);

            var request = BlockPooler.Request();
            Assert.AreEqual(instance.transform.childCount, 100);
            request.transform.SetParent(null);
            Assert.AreEqual(instance.transform.childCount, 99);
            request.transform.SetParent(instance.transform);
            Assert.AreEqual(instance.transform.childCount, 100);
            Assert.IsNotNull(request);
            BlockPooler.Return(request);
            Assert.AreEqual(instance.transform.childCount, 100);
            yield return null;

            SceneManager.LoadScene("PoolingTestScene2");
            instance = Object.FindObjectOfType<BlockPooler>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(instance.transform.childCount, 100);

            Assert.IsNotNull(BlockPooler.Request());
            yield return null;


            SceneManager.LoadScene("PoolingTestScene");
            instance = Object.FindObjectOfType<BlockPooler>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(instance.transform.childCount, 100);

            Assert.IsNotNull(BlockPooler.Request());
            yield return null;

        }

        [UnityTest]
        public IEnumerator StartFromTestWithSingletonAndReturnBack() {
            SceneManager.LoadScene("PoolingTestScene");
            yield return new WaitForSeconds(.1f);

            var instance1 = Object.FindObjectOfType<BlockPooler>();
            Assert.IsNotNull(instance1);

            SceneManager.LoadScene("PoolingTestScene2");
            yield return new WaitForSeconds(.1f);

            var instance2 = Object.FindObjectOfType<BlockPooler>();
            Assert.IsNotNull(instance2);
            Assert.AreEqual(instance1, instance2);

            SceneManager.LoadScene("PoolingTestScene");
            yield return new WaitForSeconds(.1f);


            var instance3 = Object.FindObjectOfType<BlockPooler>();
            Assert.IsNotNull(instance3);
            Assert.AreEqual(instance1, instance3);
            Assert.AreEqual(instance2, instance3);
        }
    }
}