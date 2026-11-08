using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class DualTilesetFiller
{
    // sprite index (row * 4 + col) for each mask 0..15
    static readonly int[] SpriteIndex = { 12, 15, 8, 9, 0, 11, 14, 7, 13, 4, 1, 10, 3, 2, 5, 6 };

    // sheet name, material when bit is set, material when bit is clear
    static readonly (string name, Ground set, Ground clear)[] Sheets =
    {
        ("DirtGrass",     Ground.Grass,   Ground.Dirt),
        ("DirtGravel",    Ground.Gravel,  Ground.Dirt),
        ("DirtAsphalt",   Ground.Asphalt, Ground.Dirt),
        ("GrassAsphalt",  Ground.Asphalt, Ground.Grass),
        ("GrassGravel",   Ground.Gravel,  Ground.Grass),
        ("AsphaltGravel", Ground.Asphalt, Ground.Gravel),
    };

    [MenuItem("Tools/Fill Dual Tileset")]
    static void Fill()
    {
        var go = Selection.activeGameObject;
        var dg = go != null ? go.GetComponent<DualGrid>() : null;
        if (dg == null) { Debug.LogError("Select the Grid object that has DualGrid first."); return; }

        var list = new List<DualTile>();
        var seen = new HashSet<int>();

        foreach (var s in Sheets)
            for (int mask = 0; mask < 16; mask++)
            {
                Tile tile = FindTile(s.name, SpriteIndex[mask]);
                if (tile == null) { Debug.LogError($"Tile asset not found: {s.name}_{SpriteIndex[mask]}"); continue; }

                Ground tl = (mask & 1) != 0 ? s.set : s.clear;
                Ground tr = (mask & 2) != 0 ? s.set : s.clear;
                Ground bl = (mask & 4) != 0 ? s.set : s.clear;
                Ground br = (mask & 8) != 0 ? s.set : s.clear;

                int key = (int)tl | ((int)tr << 2) | ((int)bl << 4) | ((int)br << 6);
                if (!seen.Add(key)) continue; // uniform tiles appear in several sheets

                list.Add(new DualTile { tile = tile, topLeft = tl, topRight = tr, bottomLeft = bl, bottomRight = br });
            }

        Undo.RecordObject(dg, "Fill Dual Tileset");
        dg.tileset = list.ToArray();
        EditorUtility.SetDirty(dg);
        Debug.Log($"Dual tileset filled: {list.Count} entries.");
    }

    static Tile FindTile(string sheet, int index)
    {
        string wanted = $"{sheet}_{index}";
        foreach (var guid in AssetDatabase.FindAssets(wanted + " t:Tile"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path) == wanted)
                return AssetDatabase.LoadAssetAtPath<Tile>(path);
        }
        return null;
    }
}