namespace TileKit.Core.Grid
{
    /// <summary>Common neighbor directions for 4- or 8-connected grids.</summary>
    public static class Dir
    {
        public static readonly TilePos N = new TilePos(0, -1);
        public static readonly TilePos S = new TilePos(0, 1);
        public static readonly TilePos W = new TilePos(-1, 0);
        public static readonly TilePos E = new TilePos(1, 0);

        public static readonly TilePos NW = new TilePos(-1, -1);
        public static readonly TilePos NE = new TilePos(1, -1);
        public static readonly TilePos SW = new TilePos(-1, 1);
        public static readonly TilePos SE = new TilePos(1, 1);

        public static readonly TilePos[] Cardinal = { N, S, W, E };
        public static readonly TilePos[] Ordinal = { NW, NE, SW, SE };
    }
}