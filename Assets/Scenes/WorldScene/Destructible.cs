using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene {
    public interface IDestructible {
        float Durability { get; }
        GameObject GameObject { get; }
    }

    public static class DestructibleExtensions {
        public static void Destroy(this IDestructible destructible) => Object.Destroy(destructible.GameObject);
    }
}