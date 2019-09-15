using Shared.Blocks;
using Shared.Blocks.BlockDataContainer;
using Shared.SingletonBehaviour;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shared.GameManagement {
    public class Settings : SingletonBehaviour<Settings> {
        [SerializeField] private BlockDataContainer blockDataContainer;
        [SerializeField, Range(0.1f, 10f)] private float gridUnitWidth;
        public static BlockDataContainer BlockDataContainer => Instance.blockDataContainer;
        public static float GridUnitWidth => Instance.gridUnitWidth;
    }
}