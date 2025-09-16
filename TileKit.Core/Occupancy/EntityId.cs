namespace TileKit.Core.Occupancy
{
    /// <summary>Stable identifier for an entity occupying the grid.</summary>
    public readonly struct EntityId
    {
        public readonly int Value;
        public EntityId(int v) { Value = v; }
        public override string ToString() => $"E{Value}";
        public override int GetHashCode() => Value;
        public override bool Equals(object? obj) => obj is EntityId other && other.Value == Value;
    }
}