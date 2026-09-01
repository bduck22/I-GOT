using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Vector2Int mapsize;

    public MapCellData[][] CurrentMapData;

    private void Awake()
    {
        Instance = this;
    }

    void ResetMapData()
    {
        CurrentMapData = new MapCellData[mapsize.y][];
        for(int i = 0; i < CurrentMapData.Length; i++)
        {
            CurrentMapData[i] = new MapCellData[mapsize.x];

            for (int j = 0; j < mapsize.x; j++) {
                CurrentMapData[i][j] = new MapCellData();
            }
        }
    }

    private void Start()
    {
        ResetMapData();
    }

    //public Vector2 GetPlayerCell()
    //{
        
    //}
}


[Serializable]
public class MapCellData
{
    public CellType Type;
    public RoomType RoomType;

    public bool CanUseLight;
    public bool OnLight;

    public bool HasRoomMonster;

    public Transform Tile;

    public List<Transform> MonsterSpawnPoints;

    public MapCellData()
    {
        Type = CellType.Wall;
        RoomType = RoomType.Empty;
        CanUseLight = true;
        OnLight = false;
        HasRoomMonster = false;
        MonsterSpawnPoints = new List<Transform>();
    }
}

public enum CellType
{
    Wall,
    Room
}

public enum RoomType
{
    Empty,
    Normal,
    Main,
    Electric
}