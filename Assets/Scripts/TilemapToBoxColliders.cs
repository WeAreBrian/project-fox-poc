using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToBoxColliders : MonoBehaviour
{
    private Tilemap m_TileMap;
    [Header("Hover for tooltip")]
    [SerializeField]
    [Tooltip("MUST: Slot in the ChainBoxCollider prefab into this variable from the inspector. Search in Assets when clicking the variable. This script should be placed on tilemaps that require a collisions with the chain")]
    private GameObject m_ChainBoxCollider;   //MUST: Slot in the ChainBoxCollider prefab into this variable from the inspector. Search in Assets when clicking the variable.
    //This script should be placed on tilemaps that require a collisions with the chain


    private void Awake()
    {
        m_TileMap = GetComponent<Tilemap>();
        BoundsInt m_Bounds = m_TileMap.cellBounds;
        TileBase[] m_AllTiles = m_TileMap.GetTilesBlock(m_Bounds);

        for (int x = m_Bounds.xMin; x < m_Bounds.xMax; x++)
        {
            for (int y = m_Bounds.yMin; y < m_Bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase m_Tile = m_AllTiles[(x - m_Bounds.xMin) + (y - m_Bounds.yMin) * m_Bounds.size.x];

                if (m_Tile != null && IsCornerTile(x, y, m_AllTiles, m_Bounds))
                {
                    Vector3 m_WorldPosition = m_TileMap.CellToWorld(cellPosition);
                    GameObject m_Collider = Instantiate(m_ChainBoxCollider, m_WorldPosition, Quaternion.identity);
                    m_Collider.transform.parent = transform;
                }
            }
        }
    }

    private bool IsCornerTile(int x, int y, TileBase[] allTiles, BoundsInt bounds)
    {
        if (IsAlone(x, y, allTiles, bounds))
        {
            return true;
        }

        if (IsEndOfLine(x, y, allTiles, bounds))
        {
            return true;
        }

        if (IsCornerOfShape(x, y, allTiles, bounds))
        {
            return true;
        }

        return false;
    }

    private bool IsAlone(int x, int y, TileBase[] allTiles, BoundsInt bounds)
    {
        Vector2Int[] neighborPositions = new Vector2Int[]
        {
        new Vector2Int(x - 1, y),     // left
        new Vector2Int(x + 1, y),     // right
        new Vector2Int(x, y - 1),     // bottom
        new Vector2Int(x, y + 1)      // top
        };

        foreach (Vector2Int pos in neighborPositions)
        {
            if (GetTileAt(pos.x, pos.y, allTiles, bounds) != null)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsEndOfLine(int x, int y, TileBase[] allTiles, BoundsInt bounds)
    {
        Vector2Int[] neighborPositions = new Vector2Int[]
        {
        new Vector2Int(x - 1, y),     // left
        new Vector2Int(x + 1, y),     // right
        new Vector2Int(x, y - 1),     // bottom
        new Vector2Int(x, y + 1)      // top
        };

        int occupiedNeighborCount = 0;

        foreach (Vector2Int pos in neighborPositions)
        {
            if (GetTileAt(pos.x, pos.y, allTiles, bounds) != null)
            {
                occupiedNeighborCount++;
            }
        }

        return occupiedNeighborCount == 1;
    }

    private bool IsCornerOfShape(int x, int y, TileBase[] allTiles, BoundsInt bounds)
    {
        Vector2Int[] neighborPositions = new Vector2Int[]
        {
        new Vector2Int(x - 1, y),     // left
        new Vector2Int(x + 1, y),     // right
        new Vector2Int(x, y - 1),     // bottom
        new Vector2Int(x, y + 1)      // top
        };

        List<TileBase> neighbors = new List<TileBase>();

        foreach (Vector2Int pos in neighborPositions)
        {
            TileBase neighbor = GetTileAt(pos.x, pos.y, allTiles, bounds);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        //If there are exactly 2 neighbors and they are either: 
        //bottom and right, bottom and left, top and left, top and right.
        return neighbors.Count == 2 &&
               ((GetTileAt(x, y - 1, allTiles, bounds) != null && GetTileAt(x + 1, y, allTiles, bounds) != null) || // bottom and right
                (GetTileAt(x, y - 1, allTiles, bounds) != null && GetTileAt(x - 1, y, allTiles, bounds) != null) || // bottom and left
                (GetTileAt(x, y + 1, allTiles, bounds) != null && GetTileAt(x - 1, y, allTiles, bounds) != null) || // top and left
                (GetTileAt(x, y + 1, allTiles, bounds) != null && GetTileAt(x + 1, y, allTiles, bounds) != null));  // top and right
    }

    private TileBase GetTileAt(int x, int y, TileBase[] allTiles, BoundsInt bounds)
    {
        //If this position is out of bounds, it's considered empty
        if (x < bounds.xMin || x >= bounds.xMax || y < bounds.yMin || y >= bounds.yMax)
            return null;

        return allTiles[(x - bounds.xMin) + (y - bounds.yMin) * bounds.size.x];
    }

}
