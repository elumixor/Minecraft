using System;
using UnityEngine;

namespace Shared.SpaceWrapping {
    /// <summary>
    /// Class that is used to store 3D position as an unsigned integer
    /// </summary>
    [Serializable]
    public struct UIntPosition {
        #region Fields and Properties
        public readonly int x;
        public readonly int y;
        public readonly int z;

        private long? index;

        public long Index {
            get {
                if (index.HasValue) return index.Value;

                var value = Wrapper.Wrap(x, y, z);
                index = value;
                return value;
            }
        }
        #endregion

        #region Predefined Vectors
        /// <summary>
        /// 0 0 0
        /// </summary>
        public static UIntPosition Zero { get; } = new UIntPosition();

        /// <summary>
        /// 1 1 1
        /// </summary>
        public static UIntPosition One { get; } = new UIntPosition(1, 1, 1);

        /// <summary>
        /// 1 0 0
        /// </summary>
        public static UIntPosition Right { get; } = new UIntPosition(1, 0, 0);

        /// <summary>
        /// -1 0 0
        /// </summary>
        public static UIntPosition Left { get; } = new UIntPosition(-1, 0, 0);

        /// <summary>
        /// 0 1 0
        /// </summary>
        public static UIntPosition Up { get; } = new UIntPosition(0, 1, 0);

        /// <summary>
        /// 0 -1 0
        /// </summary>
        public static UIntPosition Down { get; } = new UIntPosition(0, -1, 0);

        /// <summary>
        /// 0 0 1
        /// </summary>
        public static UIntPosition Forward { get; } = new UIntPosition(0, 0, 1);

        /// <summary>
        /// 0 0 -1
        /// </summary>
        public static UIntPosition Back { get; } = new UIntPosition(0, 0, -1);

        public static readonly UIntPosition[] AdjoiningVectors = {
            Right, Left, Up, Down, Forward, Back
        };
        #endregion

        #region Constructors
        public UIntPosition(int x1, int y1, int z1) {
            x = x1;
            y = y1;
            z = z1;
            index = null;
        }

        public UIntPosition(long i) {
            Wrapper.Unwrap(i, out x, out y, out z);
            index = i;
        }
        #endregion

        #region Operators
        public static UIntPosition operator -(UIntPosition position) => position * -1;
        public static UIntPosition operator --(UIntPosition position) => position - One;
        public static UIntPosition operator ++(UIntPosition position) => position + One;

        public static UIntPosition operator *(UIntPosition position, int value) =>
            new UIntPosition(position.x * value, position.y * value, position.z * value);

        public static UIntPosition operator /(UIntPosition position, int value) =>
            new UIntPosition(position.x / value, position.y / value, position.z / value);


        public static UIntPosition operator +(UIntPosition a, UIntPosition b) =>
            new UIntPosition(a.x + b.x, a.y + b.y, a.z + b.z);

        public static UIntPosition operator -(UIntPosition a, UIntPosition b) => a + -b;
        #endregion

        #region Conversions
        public static implicit operator (int x, int y, int z)(UIntPosition position) =>
            (position.x, position.y, position.z);

        public static implicit operator UIntPosition((int x, int y, int z) position) =>
            new UIntPosition(position.x, position.y, position.z);

        public static implicit operator Vector3Int(UIntPosition position) =>
            new Vector3Int(position.x, position.y, position.z);

        public static implicit operator UIntPosition(Vector3Int position) =>
            new UIntPosition(position.x, position.y, position.z);

        public static implicit operator Vector3(UIntPosition position) =>
            new Vector3(position.x, position.y, position.z);

        public static UIntPosition Ceil(Vector3 position) =>
            new UIntPosition(Mathf.CeilToInt(position.x), Mathf.CeilToInt(position.y), Mathf.CeilToInt(position.z));
        public static UIntPosition Floor(Vector3 position) =>
            new UIntPosition(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), Mathf.FloorToInt(position.z));
        public static UIntPosition Round(Vector3 position) =>
            new UIntPosition(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        #endregion

        #region Equality
        public static bool operator ==(UIntPosition a, UIntPosition b) => a.x == b.x && a.y == b.y && a.z == b.z;
        public static bool operator !=(UIntPosition a, UIntPosition b) => !(a == b);

        public bool Equals(UIntPosition other) => this == other;

        public override bool Equals(object obj) => obj is UIntPosition other && Equals(other);

        public override int GetHashCode() => (int) Index;
        #endregion

        #region Max
        public UIntPosition MaxX(int max) => x < max ? new UIntPosition(max, y, z) : this;
        public UIntPosition MaxY(int max) => y < max ? new UIntPosition(x, max, z) : this;
        public UIntPosition MaxZ(int max) => z < max ? new UIntPosition(x, y, max) : this;
        public UIntPosition Max(UIntPosition max) => Max(max.x, max.y, max.z);

        public UIntPosition Max(int xMax, int yMax, int zMax) =>
            new UIntPosition(x < xMax ? xMax : x, y < yMax ? yMax : y, z < zMax ? zMax : z);
        #endregion

        #region Min
        public UIntPosition MinX(int min) => x > min ? new UIntPosition(min, y, z) : this;
        public UIntPosition MinY(int min) => y > min ? new UIntPosition(x, min, z) : this;
        public UIntPosition MinZ(int min) => z > min ? new UIntPosition(x, y, min) : this;


        public UIntPosition Min(UIntPosition min) => Min(min.x, min.y, min.z);

        public UIntPosition Min(int xMin, int yMin, int zMin) =>
            new UIntPosition(x > xMin ? xMin : x, y > yMin ? yMin : y, z > zMin ? zMin : z);
        #endregion

        #region Clamp
        public UIntPosition ClampX(int min, int max) => MinX(max).MaxX(max);
        public UIntPosition ClampY(int min, int max) => MinY(min).MaxY(max);
        public UIntPosition ClampZ(int min, int max) => MinZ(min).MaxZ(max);
        public UIntPosition Clamp(UIntPosition min, UIntPosition max) => Min(min).Max(max);

        public UIntPosition Clamp(int xMin, int xMax, int yMin, int yMax, int zMin, int zMax) =>
            Min(xMin, yMin, zMin).Max(xMax, yMax, yMax);
        #endregion


        public void Deconstruct(out int px, out int py, out int pz) {
            px = x;
            py = y;
            pz = z;
        }

        public override string ToString() {
            return $"UIntPosition ({x}, {y}, {z})";
        }
    }
}