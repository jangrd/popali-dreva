using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Ground { Dirt = 0, Grass = 1, Asphalt = 2, Gravel = 3 }

[System.Serializable]
public class DualTile
{
    public Tile tile;
    public Ground topLeft, topRight, bottomLeft, bottomRight;
}

public class DualGrid : MonoBehaviour
{
    public Tilemap dataMap;
    public Tilemap displayMap;
    public TileBase[] dataTiles = new TileBase[4]; // index = Ground enum value
    public DualTile[] tileset;
    public Tile missingTile; // optional, magenta is generated if empty

    Dictionary<int, Tile> lookup;
    HashSet<int> reported = new HashSet<int>();

    static int Key(Ground tl, Ground tr, Ground bl, Ground br)
        => (int)tl | ((int)tr << 2) | ((int)bl << 4) | ((int)br << 6);

    void Start() { Refresh(); }

    void Build()
    {
        lookup = new Dictionary<int, Tile>();
        foreach (var e in tileset)
        {
            int k = Key(e.topLeft, e.topRight, e.bottomLeft, e.bottomRight);
            if (lookup.ContainsKey(k))
                Debug.LogWarning($"Duplicate tileset entry: TL={e.topLeft} TR={e.topRight} BL={e.bottomLeft} BR={e.bottomRight}");
            lookup[k] = e.tile;
        }

        if (missingTile == null)
        {
            var tex = new Texture2D(1, 1) { filterMode = FilterMode.Point };
            tex.SetPixel(0, 0, Color.magenta);
            tex.Apply();
            float ppu = 1f / dataMap.layoutGrid.cellSize.x;
            var sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), ppu);
            missingTile = ScriptableObject.CreateInstance<Tile>();
            missingTile.sprite = sprite;
        }
    }

    public void Refresh()
    {
        Build();
        reported.Clear();
        displayMap.ClearAllTiles();
        dataMap.CompressBounds();
        var b = dataMap.cellBounds;

        for (int x = b.xMin; x <= b.xMax; x++)
            for (int y = b.yMin; y <= b.yMax; y++)
            {
                Ground tl = GroundAt(x - 1, y);
                Ground tr = GroundAt(x, y);
                Ground bl = GroundAt(x - 1, y - 1);
                Ground br = GroundAt(x, y - 1);

                int k = Key(tl, tr, bl, br);
                if (!lookup.TryGetValue(k, out Tile t))
                {
                    t = missingTile;
                    if (reported.Add(k))
                        Debug.LogWarning($"Missing tile: TL={tl} TR={tr} BL={bl} BR={br} (first seen at display cell {x},{y})");
                }
                displayMap.SetTile(new Vector3Int(x, y, 0), t);
            }
    }

    Ground GroundAt(int x, int y)
    {
        TileBase t = dataMap.GetTile(new Vector3Int(x, y, 0));
        for (int i = 0; i < dataTiles.Length; i++)
            if (t != null && t == dataTiles[i]) return (Ground)i;
        return Ground.Dirt; // empty or unknown counts as dirt
    }
}