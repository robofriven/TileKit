\# TileKit.Core



A tiny, deterministic tile engine for grid-based sims.  

Pure C# (`netstandard2.1`). No Unity dependencies.



---



\## Namespaces

\- `TileKit.Core.Grid`

\- `TileKit.Core.Map`

\- `TileKit.Core.Occupancy`

\- `TileKit.Core.Path`

\- `TileKit.Core.RNG`

\- `TileKit.Core.Serialization`



---



\## TileKit.Core.Grid



\### `TilePos`

Immutable grid coordinate.



\*\*Fields\*\*

\- `int X`, `int Y`



\*\*Operators\*\*

\- `+`, `-` → add or subtract positions



\*\*Methods\*\*

\- `bool Equals(TilePos other)`

\- `override bool Equals(object obj)`

\- `override int GetHashCode()`

\- `override string ToString()` → `"(x,y)"`



---



\### `Dir`

Common neighbor directions.



\*\*Static Fields\*\*

\- `TilePos N, S, E, W`

\- `TilePos NE, NW, SE, SW`

\- `TilePos\[] Cardinal` (N, S, E, W)

\- `TilePos\[] Ordinal` (diagonals)



---



\### `TileRect`

Axis-aligned rectangle over positions.



\*\*Fields\*\*

\- `int X, Y, Width, Height`



\*\*Methods\*\*

\- `IEnumerable<TilePos> Positions()`



---



\## TileKit.Core.Map



\### `TileLayer`

Dense 2D layer storing `byte` values.



\*\*Constructor\*\*

\- `TileLayer(int width, int height, byte fill = 0)`



\*\*Fields\*\*

\- `int Width`, `int Height`



\*\*Methods\*\*

\- `bool InBounds(TilePos p)`

\- `byte Get(TilePos p)`

\- `void Set(TilePos p, byte value)`



---



\### `TileMap`

Container for terrain + named layers + movement cost.



\*\*Constructor\*\*

\- `TileMap(int width, int height, byte terrainFill = 0)`



\*\*Fields\*\*

\- `int Width`, `int Height`

\- `TileLayer Terrain`

\- `Dictionary<string, TileLayer> Layers`

\- `Func<TilePos, int> MovementCost`  

&nbsp; Return positive int = movement cost; 0 = blocked.



---



\## TileKit.Core.Occupancy



\### `EntityId`

Stable integer ID for entities.



\*\*Fields\*\*

\- `int Value`



\*\*Methods\*\*

\- `override string ToString()` → `"E{Value}"`

\- Equality \& hashing implemented.



---



\### `OccupancyMap`

Single-occupant-per-tile tracker.



\*\*Methods\*\*

\- `bool IsFree(TilePos p)`

\- `bool TryPlace(EntityId id, TilePos p)`

\- `bool TryMove(EntityId id, TilePos from, TilePos to)`

\- `bool Remove(TilePos p)`

\- `bool TryGet(TilePos p, out EntityId id)`

\- `void Clear()`



---



\## TileKit.Core.Path



\### `Pathfinder`

Deterministic A\* pathfinding.



\*\*Constructor\*\*

\- `Pathfinder(TileMap map, bool diagonals = false)`



\*\*Methods\*\*

\- `bool TryFind(TilePos start, TilePos goal, Span<TilePos> buffer, out int len)`  

&nbsp; Writes path into `buffer`. Returns true if path exists.  

&nbsp; `len` = number of steps (including start \& goal).



\*\*Notes\*\*

\- Manhattan heuristic.

\- Tiles with cost ≤ 0 are blocked.



---



\### `PriorityQueue<T>` \*(internal utility)\*

Binary heap. Exposed but mostly for `Pathfinder`.



\- `void Enqueue(T item, int priority)`

\- `bool TryDequeue(out T item, out int priority)`

\- `int Count { get; }`



---



\## TileKit.Core.RNG



\### `Rng`

Deterministic SplitMix64 RNG.



\*\*Constructor\*\*

\- `Rng(ulong seed)`



\*\*Methods\*\*

\- `int NextInt(int minInclusive, int maxExclusive)`

\- `TilePos RandomNeighbor(TilePos p, bool diagonals = false)`



---



\## TileKit.Core.Serialization



\### `TileSerde`

Binary save/load for `TileLayer`.



\*\*Methods\*\*

\- `byte\[] SaveLayer(TileLayer layer)`  

&nbsp; Format: width, height, then raw bytes row-major.

\- `TileLayer LoadLayer(ReadOnlySpan<byte> data)`



---



\## Usage Examples



\### Pathfinding

```csharp

var map = new TileMap(32, 20, terrainFill: 0);

map.MovementCost = p => map.Terrain.Get(p) == 3 ? 0 : 1;



var pf = new Pathfinder(map);

Span<TilePos> buf = stackalloc TilePos\[128];



if (pf.TryFind(new TilePos(2,2), new TilePos(10,10), buf, out var len))

{

&nbsp;   for (int i = 0; i < len; i++)

&nbsp;       Console.WriteLine(buf\[i]);

}



