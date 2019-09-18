using Shared.Blocks;
using Shared.SingletonBehaviour;
using UnityEngine;

namespace Shared.GameManagement {
    public class Settings : SingletonBehaviour<Settings> {
        [SerializeField] private BlockData[] blockData;
        [SerializeField] private int[] playerConstructable;

        
        [SerializeField, Range(0.1f, 10f)] private float gridUnitWidth;
        public static float GridUnitWidth => Instance.gridUnitWidth;

        public static BlockData GetBlockData(BlockType blockType) => blockType.ArrayValueIn(Instance.blockData);
    }
}