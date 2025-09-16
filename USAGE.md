# TileKit.Core — Usage Guide

This is the “how do I actually use it” doc. You don’t need internet, Unity Tilemap, or a goat sacrifice.
Just TileKit.Core, a SpriteRenderer, and a pulse.

---

## 0) Install

**Option A: Embedded package (recommended)**
- Create `Packages/com.yourname.tilekit/`
- Put the `TileKit.Core/` folder inside (include `TileKit.Core.csproj` if you want to build outside Unity)
- Add a minimal `package.json` if you want Hub/UPM to list it (optional)

**Option B: Source in Assets**
- Drop `TileKit.Core/` under `Assets/TileKit/TileKit.Core`
- It’s plain C#; compiles fine in Unity

**Option C: DLL**
- Build `TileKit.Core.csproj` to a DLL
- Put in `Assets/Plugins/TileKit/`
- Iteration is worse, but works

---

## 1) Minimal Unity setup

Create an empty GameObject: `TileMapView`  
Add components:
- **SpriteRenderer** (leave **Sprite** = None)
- **TileMapView** (the tiny adapter script)

In `TileMapView`:
- Set **Tile Size**: `16` or `32`
- Fill **Terrain Palette** with visible colors (and set **alpha to 1**; Unity defaults to invisible)

Create a script on the same object: `TileBootstrap.cs`

```csharp
using UnityEngine;
using TileKit.Core.Grid;
using TileKit.Core.Map;
using TileKit.Core.Path;
using TileKit.Core.Occupancy;

[RequireComponent(typeof(TileMapView), typeof(SpriteRenderer))]
public class TileBootstrap : MonoBehaviour
{
    [SerializeField] int width = 48;
    [SerializeField] int height = 32;

    TileMap map;
    Pathfinder path;
    OccupancyMap occ;
    TileMapView view;

    void Start()
    {
        view = GetComponent<TileMapView>();

        // Build terrain
        map = new TileMap(width, height, terrainFill: 0);
        for (int y=0; y<height; y++)
        for (int x=0; x<width; x++)
        {
            var p = new TilePos(x,y);
            if (x==0 || y==0 || x==width-1 || y==height-1) map.Terrain.Set(p, 3);    // wall
            else if (x >= width/3 && x < width/3+3)        map.Terrain.Set(p, 1);    // forest stripe
            else                                           map.Terrain.Set(p, 0);    // grass
        }

        // Costs: 0=blocked
        map.MovementCost = p => map.Terrain.Get(p) switch { 0 => 1, 1 => 3, 2 => 6, 3 => 0, _ => 1 };

        path = new Pathfinder(map);
        occ  = new OccupancyMap();

        view.SetMap(map);
        view.Redraw();

        var cam = Camera.main;
        if (cam) { cam.orthographic = true; cam.transform.position = new Vector3(0,0,-10); }

        // optional: paint a sample path to prove life
        DemoPath();
    }

    void DemoPath()
    {
        var start = new TilePos(2,2);
        var goal  = new TilePos(width-3, height-3);
        System.Span<TilePos> buf = stackalloc TilePos[1024];
        if (path.TryFind(start, goal, buf, out var len))
        {
            for (int i=0; i<len; i++) map.Terrain.Set(buf[i], 1);
            view.Redraw();
        }
    }
}
```

Hit Play. If you see a colored rectangle with a stripe and border, the world is alive.

---

## 2) Convert mouse position to tile (click-to-select)

```csharp
public static class TileUtil
{
    public static TilePos WorldToTile(Vector3 world, int tileSize)
        => new TilePos(Mathf.FloorToInt(world.x / tileSize), Mathf.FloorToInt(world.y / tileSize));

    public static bool TryGetMouseTile(Camera cam, int tileSize, out TilePos pos)
    {
        var world = cam.ScreenToWorldPoint(Input.mousePosition);
        pos = WorldToTile(world, tileSize);
        return true;
    }
}
```

Usage in a MonoBehaviour:

```csharp
void Update()
{
    if (!Camera.main) return;
    if (Input.GetMouseButtonDown(0))
    {
        if (TileUtil.TryGetMouseTile(Camera.main, 16, out var tile))
            Debug.Log($"Clicked tile {tile}");
    }
}
```

---

## 3) Draw dynamic overlays (cursor highlight)

```csharp
[SerializeField] Color highlightColor = new Color(1,1,0,1); // yellow

TilePos? lastHover;

void Update()
{
    if (!Camera.main) return;

    if (TileUtil.TryGetMouseTile(Camera.main, 16, out var tile))
    {
        map.Terrain.Set(tile, 2); // just demo tint
        view.Redraw();
        lastHover = tile;
    }
}
```

---

## 4) Move an entity along a path

```csharp
[SerializeField] Transform token;
EntityId id = new EntityId(1);

void Start()
{
    var start = new TilePos(2,2);
    occ.TryPlace(id, start);
    token.position = new Vector3(start.X * 16, start.Y * 16, 0);
}

IEnumerator Walk(TilePos goal)
{
    Span<TilePos> buf = stackalloc TilePos[256];
    if (!path.TryFind(TileFromWorld(token.position), goal, buf, out var len)) yield break;

    for (int i=1; i<len; i++)
    {
        var from = buf[i-1];
        var to   = buf[i];
        if (!occ.TryMove(id, from, to)) yield break;

        var toWorld = new Vector3(to.X * 16, to.Y * 16, 0);
        float t = 0, dur = 0.1f;
        var fromWorld = token.position;
        while (t < 1f) { t += Time.deltaTime / dur; token.position = Vector3.Lerp(fromWorld, toWorld, t); yield return null; }
    }
}

TilePos TileFromWorld(Vector3 pos) => new TilePos(Mathf.RoundToInt(pos.x/16f), Mathf.RoundToInt(pos.y/16f));
```

---

## 5) Save/load

```csharp
using TileKit.Core.Serialization;

var bytes = TileSerde.SaveLayer(map.Terrain);
// write to disk

var loaded = TileSerde.LoadLayer(bytes);
map = new TileMap(loaded.Width, loaded.Height);
map.Terrain = loaded;
view.SetMap(map);
view.Redraw();
```

---

## 6) Tips & gotchas

- Costs are integers. Return 0 from `MovementCost` to block.
- Use `Rng(seed)` for reproducible randomness.
- Unity Color defaults to alpha 0 — make sure palette colors are opaque.
- Must call `SetMap` then `Redraw`.
- Orthographic camera, z = -10.

---

## 7) Cheat sheet

```csharp
var p = new TilePos(3,5);
var q = p + Dir.E;

var layer = new TileLayer(64,64);
layer.Set(p, 1);

var map = new TileMap(64,64);
map.MovementCost = pos => layer.Get(pos) == 3 ? 0 : 1;

var pf = new Pathfinder(map);
Span<TilePos> buf = stackalloc TilePos[256];
pf.TryFind(new TilePos(2,2), new TilePos(10,10), buf, out var len);

var occ = new OccupancyMap();
var id = new EntityId(7);
occ.TryPlace(id, new TilePos(2,2));
occ.TryMove(id, new TilePos(2,2), new TilePos(3,2));

var rng = new TileKit.Core.RNG.Rng(123456);
int r = rng.NextInt(0, 10);
```
