using System.Collections.Generic;

namespace TileKit.Core.Grid
{
    /// <summary>Axis-aligned rectangle iterator for grid areas.</summary>
    public readonly struct TileRect
    {
        public readonly int X, Y, Width, Height;
        public TileRect(int x, int y, int width, int height) { X = x; Y = y; Width = width; Height = height; }

        public IEnumerable<TilePos> Positions()
        {
            for (int yy = Y; yy < Y + Height; yy++)
                for (int xx = X; xx < X + Width; xx++)
                    yield return new TilePos(xx, yy);
        }
    }
}