using System;
using System.Collections.Generic;
using TileKit.Core.Grid;
using TileKit.Core.Map;

namespace TileKit.Core.Path
{
    /// <summary>Deterministic A* pathfinder on TileMap with integer costs.</summary>
    public sealed class Pathfinder
    {
        private readonly TileMap _map;
        private readonly bool _diagonals;
        private readonly TilePos[] _neighbors;

        public Pathfinder(TileMap map, bool diagonals = false)
        {
            _map = map; _diagonals = diagonals;
            _neighbors = diagonals ?
                new TilePos[] { Dir.N, Dir.S, Dir.W, Dir.E, Dir.NW, Dir.NE, Dir.SW, Dir.SE } :
                new TilePos[] { Dir.N, Dir.S, Dir.W, Dir.E };
        }

        private static int Heuristic(TilePos a, TilePos b)
            => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        /// <summary>Find path from start to goal. Writes up to buffer.Length steps into buffer.</summary>
        public bool TryFind(TilePos start, TilePos goal, Span<TilePos> buffer, out int len)
        {
            len = 0;
            if (start.Equals(goal)) { if (buffer.Length > 0) { buffer[0] = start; len = 1; return true; } return false; }

            var open = new PriorityQueue<TilePos>();
            var cameFrom = new Dictionary<TilePos, TilePos>();
            var g = new Dictionary<TilePos, int> { [start] = 0 };
            var f = new Dictionary<TilePos, int> { [start] = Heuristic(start, goal) };
            open.Enqueue(start, f[start]);

            while (open.TryDequeue(out var cur, out _))
            {
                if (cur.Equals(goal))
                {
                    var rev = new List<TilePos> { cur };
                    while (cameFrom.TryGetValue(cur, out var prev)) { cur = prev; rev.Add(cur); }
                    rev.Reverse();
                    len = Math.Min(rev.Count, buffer.Length);
                    for (int i = 0; i < len; i++) buffer[i] = rev[i];
                    return true;
                }

                foreach (var d in _neighbors)
                {
                    var nxt = new TilePos(cur.X + d.X, cur.Y + d.Y);
                    if (!_map.Terrain.InBounds(nxt)) continue;
                    int step = _map.MovementCost(nxt);
                    if (step <= 0) continue;
                    int tentative = g[cur] + step;
                    if (!g.TryGetValue(nxt, out var best) || tentative < best)
                    {
                        cameFrom[nxt] = cur;
                        g[nxt] = tentative;
                        int score = tentative + Heuristic(nxt, goal);
                        f[nxt] = score;
                        open.Enqueue(nxt, score);
                    }
                }
            }
            return false;
        }
    }
}