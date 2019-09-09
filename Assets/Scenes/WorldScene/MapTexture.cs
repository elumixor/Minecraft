using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.WorldScene {
    [ExecuteInEditMode]
    public class MapTexture : MonoBehaviour {
        [SerializeField] private Image image;

        [SerializeField, Range(10, 500)] private int width = 10;
        [SerializeField, Range(10, 500)] private int height = 10;

        [SerializeField, Range(1, 5)] private int octaves;

        [SerializeField, Range(0.0001f, 100f)] private float scale = 10f;
        [SerializeField, Range(0.0001f, 1)] private float persistance = 10f;
        [SerializeField, Range(0.0001f, 5)] private float lacunarity = 10f;


        // Start is called before the first frame update
        private void Start() {
            image = GetComponent<Image>();
            GenerateTexture();
        }

        private void Reset() => Start();

        // Update is called once per frame
        public float[,] GenerateTexture() {
            var texture = new Texture2D(width, height)
                {filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp};

            var points = new float[width, height];
            var colorMap = new Color[width * height];

            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++) {
                var amplitude = 1f;
                var frequency = 1f;
                var noiseHeight = 0f;

                for (var i = 0; i < octaves; i++) {
                    var xValue = x / scale * frequency;
                    var yValue = y / scale * frequency;
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
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, points[x, y]);
            }

            image.material.mainTexture = texture;
            texture.SetPixels(colorMap);
            texture.Apply();
            image.SetAllDirty();
            return points;
        }

        private void OnValidate() => GenerateTexture();
    }
}