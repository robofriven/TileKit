using System.Collections.Generic;
using TileKit.Core.Grid;

namespace TileKit.Core.Occupancy
{
    /// <summary>Tracks single-occupant per tile. Use higher-level systems for stacks.</summary>
    public sealed class OccupancyMap
    {
        private readonly Dictionary<TilePos, EntityId> _occ = new Dictionary<TilePos, EntityId>();

        public bool IsFree(TilePos p) => !_occ.ContainsKey(p);

        public bool TryPlace(EntityId id, TilePos p)
        {
            if (_occ.ContainsKey(p)) return false;
            _occ[p] = id; return true;
        }

        public bool TryMove(EntityId id, TilePos from, TilePos to)
        {
            if (!_occ.TryGetValue(from, out var cur) || cur.Value != id.Value) return false;
            if (_occ.ContainsKey(to)) return false;
            _occ.Remove(from); _occ[to] = id; return true;
        }

        public bool Remove(TilePos p) => _occ.Remove(p);

        public bool TryGet(TilePos p, out EntityId id) => _occ.TryGetValue(p, out id);
        public void Clear() => _occ.Clear();
    }
}