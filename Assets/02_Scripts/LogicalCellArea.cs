using System.Collections.Generic;
using UnityEngine;

public class LogicalCellArea : MonoBehaviour
{
    public Vector2Int coordinate;

    public RoomType RoomType;

    public bool CanUseLight;

    public List<Transform> MonsterSpawnPoints;

    private void Start()
    {
        MapManager.Instance.RegisterArea(coordinate, this);
    }
}
