using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Shared.Tests.Parenting {
    public class ParentingTests : TimedTest {
        [UnityTest]
        public IEnumerator InstantiatingObjects() {
            const int count = 50_000;
            var objects = new GameObject[count];

            sw.Start();
            for (var i = 0; i < objects.Length; i++) objects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sw.Stop();
            Debug.Log("Creating");
            LogResults();

            yield return new WaitForSeconds(.1f);

            sw = new Stopwatch();

            sw.Start();
            foreach (var obj in objects) Object.Destroy(obj);

            sw.Stop();
            Debug.Log("Deleting");
            LogResults();
            yield return new WaitForSeconds(.1f);

            var parent = new GameObject("Parent").transform;
            var parent2 = new GameObject("Parent2").transform;
            sw = new Stopwatch();

            sw.Start();
            for (var i = 0; i < objects.Length; i++) {
                objects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                objects[i].transform.SetParent(parent);
            }
            sw.Stop();
            Debug.Log("Creating with parent");
            LogResults();
            yield return new WaitForSeconds(.1f);

            sw = new Stopwatch();

            sw.Start();

            foreach (var obj in objects) obj.transform.SetParent(parent2);
            sw.Stop();
            Debug.Log("Rearranging with parent");
            LogResults();
            yield return new WaitForSeconds(.1f);


            sw = new Stopwatch();

            sw.Start();

            Object.Destroy(parent2);
            sw.Stop();
            Debug.Log("Deleting with parent");
            LogResults();
            yield return new WaitForSeconds(.1f);
        }
    }
}