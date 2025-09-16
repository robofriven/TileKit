#if UNITY_2021_3_OR_NEWER
using UnityEngine;
using TileKit.Core.Map;
using TileKit.Core.Grid;

public class TileMapView : MonoBehaviour
{
    [SerializeField] private int tileSize = 16;
    [SerializeField] private Color[] terrainPalette;

    private TileMap? _map;
    private Texture2D? _tex;
    private SpriteRenderer? _sr;

    public void SetMap(TileMap map)
    {
        _map = map;
        if (_sr == null) _sr = GetComponent<SpriteRenderer>();
        _tex = new Texture2D(map.Width, map.Height, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        _sr.sprite = Sprite.Create(_tex, new Rect(0,0,map.Width, map.Height), new Vector2(0.5f,0.5f), 1f);
        _sr.transform.localScale = new Vector3(tileSize, tileSize, 1);
        Redraw();
    }

    public void Redraw()
    {
        if (_map == null || _tex == null || terrainPalette == null || terrainPalette.Length == 0) return;
        for (int y=0; y<_map.Height; y++)
        for (int x=0; x<_map.Width; x++)
        {
            var v = _map.Terrain.Get(new TilePos(x,y));
            var c = terrainPalette[Mathf.Clamp(v, 0, terrainPalette.Length-1)];
            _tex.SetPixel(x, y, c);
        }
        _tex.Apply(false);
    }
}
#endif