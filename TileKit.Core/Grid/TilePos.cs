using System;

namespace TileKit.Core.Grid
{
    /// <summary>Integer grid coordinate used as a value type key.</summary>
    public readonly struct TilePos : IEquatable<TilePos>
    {
        public readonly int X;
        public readonly int Y;

        public TilePos(int x, int y) { X = x; Y = y; }

        public static TilePos operator +(TilePos a, TilePos b) => new TilePos(a.X + b.X, a.Y + b.Y);
        public static TilePos operator -(TilePos a, TilePos b) => new TilePos(a.X - b.X, a.Y - b.Y);

        public bool Equals(TilePos other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is TilePos other && Equals(other);
        public override int GetHashCode() => unchecked((X * 397) ^ Y);
        public override string ToString() => $"({X},{Y})";
    }
}