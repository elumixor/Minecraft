namespace Scenes.WorldScene.Block {
    public enum BlockType {
        Ground,
        Rock,
        Water,
        Sand
        // define additional enum types below
    }

    public static class BlockTypeExtensions {
        public static BlockData BlockData(this BlockType blockType) => Settings.BlockDataContainer[blockType];
    }
}