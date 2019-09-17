using System;
using System.Diagnostics;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

namespace Shared.Tests {
    public class TimedTest {
        protected static Stopwatch sw;
        [SetUp]
        public void Setup() => sw = new Stopwatch();
        protected static void LogResults() {
            var ts = sw.Elapsed;
            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:0000}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Debug.Log("RunTime " + elapsedTime);
        }
    }
}