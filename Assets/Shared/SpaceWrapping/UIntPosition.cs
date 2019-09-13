using System;
using UnityEngine;

namespace Shared.SpaceWrapping {
    /// <summary>
    /// Class that is used to store 3D position as an unsigned integer
    /// </summary>
    [Serializable]
    public struct UIntPosition {
        #region Fields and Properties

        public int x;
        public int y;
        public int z;

        public long Index {
            get => Wrapper.Wrap(x, y, z);
            set => Wrapper.Unwrap(value, out x, out y, out z);
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

        #endregion

        #region Constructors

        public UIntPosition(int x1, int y1, int z1) {
            x = x1;
            y = y1;
            z = z1;
        }

        public UIntPosition(int x1, int y1) {
            x = x1;
            y = y1;
            z = 0;
        }

        public UIntPosition(int index) => Wrapper.Unwrap(index, out x, out y, out z);

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

        public static implicit operator (int x, int y, int z)(UIntPosition position) => (position.x, position.y, position.z);

        public static implicit operator UIntPosition((int x, int y, int z) position) =>
            new UIntPosition(position.x, position.y, position.z);

        public static implicit operator Vector3Int(UIntPosition position) => new Vector3Int(position.x, position.y, position.z);

        public static implicit operator UIntPosition(Vector3Int position) => new UIntPosition(position.x, position.y, position.z);

        public static implicit operator Vector3(UIntPosition position) => new Vector3Int(position.x, position.y, position.z);

        public static implicit operator UIntPosition(Vector3 position) =>
            new UIntPosition(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));

        #endregion

        #region Equality

        public static bool operator ==(UIntPosition a, UIntPosition b) => a.x == b.x && a.y == b.y && a.z == b.z;
        public static bool operator !=(UIntPosition a, UIntPosition b) => !(a == b);
        
        public bool Equals(UIntPosition other) => this == other;

        public override bool Equals(object obj) => obj is UIntPosition other && Equals(other);

        public override int GetHashCode() => (int) Index;

        #endregion

        public void Deconstruct(out int px, out int py, out int pz) {
            px = x;
            py = y;
            pz = z;
        }
    }
}