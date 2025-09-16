using System;
using TileKit.Core.Grid;

namespace TileKit.Core.Map
{
    /// <summary>A dense tile layer. Use byte for compact terrain/material ids.</summary>
    public sealed class TileLayer
    {
        private readonly byte[] _data;
        public readonly int Width;
        public readonly int Height;

        public TileLayer(int width, int height, byte fill = 0)
        {
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            Width = width; Height = height;
            _data = new byte[width * height];
            if (fill != 0) Array.Fill(_data, fill);
        }

        private int Idx(TilePos p) => p.Y * Width + p.X;
        public bool InBounds(TilePos p) => unchecked((uint)p.X < (uint)Width && (uint)p.Y < (uint)Height);

        public byte Get(TilePos p) => _data[Idx(p)];
        public void Set(TilePos p, byte v) => _data[Idx(p)] = v;
    }
}