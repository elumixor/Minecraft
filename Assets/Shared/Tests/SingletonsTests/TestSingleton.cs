using Shared.SingletonBehaviour;

namespace Shared.Tests.SingletonsTests {
    public class TestSingleton : SingletonBehaviour<TestSingleton> {
        private int instanceValue = 1;
        public static int StaticValue => 2;
        public static int InstanceValue => Instance.instanceValue;

        public static int AwakeCount { get; set; }
        public static int DestroyCount { get; set; }

        public static TestSingleton GetInstance => Instance;

        protected override void Awake() {
            AwakeCount++;
            base.Awake();
        }

        protected void OnDestroy() {
            DestroyCount++;
        }
    }
}