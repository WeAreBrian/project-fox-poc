using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToBoxColliders : MonoBehaviour
{
    private Tilemap m_TileMap;
    [SerializeField]
    private GameObject m_ChainBoxCollider;   //PLZ MAKE SURE THE PREFAB WITH THIS NAME IS SLOTTED IN


    private void Awake()
    {
        m_TileMap = GetComponent<Tilemap>();
        BoundsInt m_Bounds = m_TileMap.cellBounds;
        TileBase[] m_AllTiles = m_TileMap.GetTilesBlock(m_Bounds);

        //checks each cell in x and y of the tilemap bounds
        for (int x = m_Bounds.xMin; x < m_Bounds.xMax; x++)
        {
            for (int y = m_Bounds.yMin; y < m_Bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                //Try get the tile in this cell
                TileBase m_Tile = m_AllTiles[(x - m_Bounds.xMin) + (y - m_Bounds.yMin) * m_Bounds.size.x];

                //If a tile indeed exists in that cell
                if (m_Tile != null)
                {
                    //Spawns Chain Box Collider in the cell
                    Vector3 m_WorldPosition = m_TileMap.CellToWorld(cellPosition);
                    GameObject m_Collider = Instantiate(m_ChainBoxCollider, m_WorldPosition, Quaternion.identity);
                    m_Collider.transform.parent = transform;

                }
            }
        }
    }
}
