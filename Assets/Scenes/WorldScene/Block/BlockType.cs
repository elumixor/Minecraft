namespace Scenes.WorldScene.Block {
    public enum BlockType {
        Grass,
        Metal,
        Water,
        Wood
        // define additional enum types below
    }

    public static class BlockTypeExtensions {
        public static BlockData BlockData(this BlockType blockType) => Settings.BlockDataContainer[blockType];
    }
}