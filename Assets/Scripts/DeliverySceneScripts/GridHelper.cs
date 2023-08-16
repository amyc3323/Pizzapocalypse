using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridHelper : MonoBehaviour
{
    public static GridHelper instance;
    public Tilemap curbTilemap;
    public Tilemap grassTilemap;
    public Tilemap streetTilemap;
    private void Awake()
    {
        instance = this;
    }

}
