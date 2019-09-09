using System;
using UnityEngine;
using UnityEngine.AI;

namespace Scenes.WorldScene {
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class GeneratedMesh : MonoBehaviour {
        public int size = 10;

        public Vector3[] vertices;
        public int[] triangles;

        private Mesh mesh;

        private void Start() {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            CreateShape();
            UpdateMesh();
        }
        private void CreateShape() {
            vertices = new Vector3[(size + 1) * (size + 1)];

            for (int i = 0, z = 0; z <= size; z++)
            for (var x = 0; x <= size; x++)
                vertices[i++] = new Vector3(x, Mathf.PerlinNoise(x * .3f, z * .3f) * 2f, z);

            triangles = new int[size * size * 6];

            var vert = 0;
            var tris = 0;

            for (var z = 0; z < size; z++) {
                for (var x = 0; x < size; x++) {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + size + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + size + 1;
                    triangles[tris + 5] = vert + size + 2;

                    vert++;
                    tris += 6;
                }

                vert++;
            }
        }
        public void UpdateMesh() {
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        private void OnDrawGizmos() {
            if (vertices == null) return;

            foreach (var vertex in vertices) {
                Gizmos.DrawSphere(vertex, .1f);
            }
        }
    }
}