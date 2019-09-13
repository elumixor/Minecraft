using UnityEngine;

namespace Shared.GameManagement {
    public static class PerlinGenerator {
        public static float[,] Generate(int width, int height, int octaves, float scale,
            float offsetX, float offsetY, float persistance, float lacunarity) {
            var points = new float[width, height];

            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++) {
                var amplitude = 1f;
                var frequency = 1f;
                var noiseHeight = 0f;

                for (var i = 0; i < octaves; i++) {
                    var xValue = (x + offsetX) / scale * frequency;
                    var yValue = (y + offsetY) / scale * frequency;
                    noiseHeight += (Mathf.PerlinNoise(xValue, yValue) * 2 - 1) * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxHeight) maxHeight = noiseHeight;
                if (noiseHeight < minHeight) minHeight = noiseHeight;

                points[x, y] = noiseHeight;
            }

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++) {
                points[x, y] = Mathf.InverseLerp(minHeight, maxHeight, points[x, y]);
            }

            return points;
        }
    }
}