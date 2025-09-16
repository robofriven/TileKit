using System;
using System.Collections.Generic;
using TileKit.Core.Grid;

namespace TileKit.Core.Map
{
    /// <summary>Container for terrain plus named layers and a movement cost callback.</summary>
    public sealed class TileMap
    {
        public readonly int Width;
        public readonly int Height;
        public TileLayer Terrain { get; }
        public Dictionary<string, TileLayer> Layers { get; } = new Dictionary<string, TileLayer>();

        /// <summary>Return positive integer movement cost; return 0 for impassable.</summary>
        public Func<TilePos, int> MovementCost { get; set; }

        public TileMap(int width, int height, byte terrainFill = 0)
        {
            Width = width; Height = height;
            Terrain = new TileLayer(width, height, terrainFill);
            MovementCost = _ => 1;
        }
    }
}