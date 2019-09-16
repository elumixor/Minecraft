using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Shared.SingletonBehaviour;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Shared.Tests.SingletonsTests {
    public class LifeCycleTests {
        private static int i = 0;
        [SetUp]
        public void Setup() {
            Debug.Log("setup " + i++);
            TestSingleton.AwakeCount = 0;
            TestSingleton.DestroyCount = 0;
        }

        [TearDown]
        public void Teardown() {
            TestSingleton.AwakeCount = 0;
            TestSingleton.DestroyCount = 0;
        }

        [Test]
        public void ValueGetters() {
            Assert.AreEqual(TestSingleton.StaticValue, 2);
            Assert.AreEqual(TestSingleton.InstanceValue, 1);
        }

        [UnityTest]
        public IEnumerator MultipleValuesDestroyed() {
            var k1 = new GameObject("K1").AddComponent<TestSingleton>();
            var k2 = new GameObject("K2").AddComponent<TestSingleton>();

            yield return null;

            Assert.AreEqual(TestSingleton.AwakeCount, 2);
            Assert.AreEqual(TestSingleton.DestroyCount, 1);

            Assert.IsNull(k2);
            Assert.IsNotNull(k1);
            yield return null;
        }

        [UnityTest]
        public IEnumerator StaysThroughoutMultipleScenes() {
            SceneManager.LoadScene("SceneWithoutSingleton");
            yield return new WaitForSeconds(.1f);

            Assert.AreEqual(Object.FindObjectsOfType<TestSingleton>().Length, 0);
            Assert.AreEqual(TestSingleton.AwakeCount, 0);
            var instance1 = TestSingleton.GetInstance;
            Assert.AreEqual(TestSingleton.AwakeCount, 1);
            Assert.IsNotNull(instance1);

            SceneManager.LoadScene("SceneWithoutSingleton2");
            yield return new WaitForSeconds(.1f);


            var instance2 = TestSingleton.GetInstance;
            Assert.AreEqual(TestSingleton.AwakeCount, 1);
            Assert.AreEqual(TestSingleton.DestroyCount, 0);
            Assert.IsNotNull(instance2);
            Assert.AreEqual(instance1, instance2);
            yield return null;

            SceneManager.LoadScene("SceneWithSingleton");
            yield return new WaitForSeconds(.1f);


            Assert.AreEqual(TestSingleton.AwakeCount, 2);
            Assert.AreEqual(TestSingleton.DestroyCount, 1);
            var instance3 = TestSingleton.GetInstance;
            Assert.IsNotNull(instance3);
            Assert.AreEqual(instance1, instance3);
            yield return null;
        }


        [UnityTest]
        public IEnumerator StartFromTestWithSingletonAndReturnBack() {
            SceneManager.LoadScene("SceneWithSingleton");
            yield return new WaitForSeconds(.1f);

            var instance1 = TestSingleton.GetInstance;
            Assert.IsNotNull(instance1);
            var instance2 = Object.FindObjectOfType<TestSingleton>();
            Assert.IsNotNull(instance2);
            Assert.AreEqual(instance1, instance2);

            SceneManager.LoadScene("SceneWithoutSingleton");
            yield return new WaitForSeconds(.1f);

            instance1 = TestSingleton.GetInstance;
            Assert.IsNotNull(instance1);
            instance2 = Object.FindObjectOfType<TestSingleton>();
            Assert.IsNotNull(instance2);
            Assert.AreEqual(instance1, instance2);


            SceneManager.LoadScene("SceneWithSingleton");
            yield return new WaitForSeconds(.1f);

            instance1 = TestSingleton.GetInstance;
            Assert.IsNotNull(instance1);
            instance2 = Object.FindObjectOfType<TestSingleton>();
            Assert.IsNotNull(instance2);
            Assert.AreEqual(instance1, instance2);
        }
    }
}