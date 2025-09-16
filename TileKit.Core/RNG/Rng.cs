using TileKit.Core.Grid;

namespace TileKit.Core.RNG
{
    /// <summary>Deterministic SplitMix64-based RNG; platform-stable.</summary>
    public sealed class Rng
    {
        private ulong _s;

        public Rng(ulong seed)
        {
            _s = seed == 0 ? 0x9E3779B97F4A7C15UL : seed;
        }

        private ulong NextU64()
        {
            _s += 0x9E3779B97F4A7C15UL;
            ulong z = _s;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }

        public int NextInt(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive) return minInclusive;
            var span = (ulong)(maxExclusive - minInclusive);
            return (int)(NextU64() % span) + minInclusive;
        }

        public TilePos RandomNeighbor(TilePos p, bool diagonals = false)
        {
            var arr = diagonals
                ? new TilePos[] { Dir.N, Dir.S, Dir.W, Dir.E, Dir.NW, Dir.NE, Dir.SW, Dir.SE }
                : new TilePos[] { Dir.N, Dir.S, Dir.W, Dir.E };
            var d = arr[NextInt(0, arr.Length)];
            return new TilePos(p.X + d.X, p.Y + d.Y);
        }
    }
}