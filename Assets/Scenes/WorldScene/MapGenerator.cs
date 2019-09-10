using System;
using Shared;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.WorldScene {
    [ExecuteInEditMode]
    public class MapGenerator : SingletonBehaviour<MapGenerator> {
        [SerializeField] private Image image;

        [SerializeField, Range(10, 500)] private int width = 10;
        [SerializeField, Range(10, 500)] private int height = 10;

        [SerializeField, Range(1, 5)] private int octaves;

        [SerializeField, Range(0.0001f, 100f)] private float scale = 10f;
        [SerializeField, Range(0.0001f, 1)] private float persistance = 10f;
        [SerializeField, Range(0.0001f, 5)] private float lacunarity = 10f;

        [SerializeField] private float offsetX;
        [SerializeField] private float offsetY;

        [SerializeField] private float seed;

        // Start is called before the first frame update
        private void Start() {
            image = GetComponent<Image>();
            UpdateTexture(Generate(width, height, octaves, scale, offsetX, offsetY, seed, persistance, lacunarity));
        }

        private void Reset() => Start();

        public static float[,] Generate(int width, int height, float scale, float offsetX, float offsetY, float seed) =>
            Generate(width, height, Instance.octaves, scale, offsetX, offsetY, seed, Instance.persistance, Instance.lacunarity);

        public static float[,] Generate(int width, int height, int octaves, float scale, float offsetX, float offsetY, float seed,
            float persistance, float lacunarity) {
            var points = new float[width, height];

            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++) {
                var amplitude = 1f;
                var frequency = 1f;
                var noiseHeight = 0f;

                for (var i = 0; i < octaves; i++) {
                    var xValue = (x + offsetX + seed) / scale * frequency;
                    var yValue = (y + offsetY + seed) / scale * frequency;
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

        private void OnValidate() =>
            UpdateTexture(Generate(width, height, octaves, scale, offsetX, offsetY, seed, persistance, lacunarity));


        private void UpdateTexture(float[,] points) {
            var texture = new Texture2D(width, height)
                {filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp};
            var colorMap = new Color[width * height];

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, points[x, y]);

            image.material.mainTexture = texture;
            texture.SetPixels(colorMap);
            texture.Apply();
            image.SetAllDirty();
        }
    }
}