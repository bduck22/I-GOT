using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public Vector2Int mapsize;

    [SerializeField, HideInInspector]
    public MapCellData[] CurrentMapData;

    public LayerMask CellLayer;

#if UNITY_EDITOR

    private Vector2 mapScroll;

    private int selectedCellIndex = -1;

    private const float CellWidth = 60f;
    private const float CellHeight = 70f;


    // =========================================================
    // Inspector
    // =========================================================

    [OnInspectorGUI]
    private void DrawMapInspector()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            $"Map ({mapsize.x} × {mapsize.y})",
            EditorStyles.boldLabel
        );

        int requiredSize = mapsize.x * mapsize.y;


        // =====================================================
        // 배열 크기 검사
        // =====================================================

        if (CurrentMapData == null ||
            CurrentMapData.Length != requiredSize)
        {
            int currentSize =
                CurrentMapData == null
                    ? 0
                    : CurrentMapData.Length;

            EditorGUILayout.HelpBox(
                $"현재 MapData 크기가 MapSize와 맞지 않습니다.\n" +
                $"Current : {currentSize}\n" +
                $"Required : {requiredSize}",
                MessageType.Warning
            );

            if (GUILayout.Button("Reset Map Data"))
            {
                ResetMapData();

                selectedCellIndex = -1;

                EditorUtility.SetDirty(this);
            }

            return;
        }


        // =====================================================
        // 맵
        // =====================================================

        DrawGrid();


        EditorGUILayout.Space(10);


        // =====================================================
        // 선택된 셀
        // =====================================================

        DrawSelectedCell();
    }


    // =========================================================
    // Grid
    // =========================================================

    private void DrawGrid()
    {
        float height =
            Mathf.Min(
                mapsize.y * (CellHeight + 5f) + 25f,
                450f
            );

        mapScroll = EditorGUILayout.BeginScrollView(
            mapScroll,
            true,
            true,
            GUILayout.Height(height)
        );

        for (int y = 0; y < mapsize.y; y++)
        {
            // 핵심: 남는 공간을 강제로 채우지 않음
            EditorGUILayout.BeginHorizontal(
                GUILayout.ExpandWidth(false)
            );

            for (int x = 0; x < mapsize.x; x++)
            {
                DrawCell(x, y);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }


    // =========================================================
    // Cell 표시
    // =========================================================

    private void DrawCell(int x, int y)
    {
        int index = y * mapsize.x + x;

        MapCellData cell = Get(x, y);

        bool selected =
            selectedCellIndex == index;

        GUIStyle cellStyle =
            new GUIStyle(GUI.skin.box);

        cellStyle.fixedWidth = CellWidth;
        cellStyle.fixedHeight = CellHeight;

        cellStyle.stretchWidth = false;
        cellStyle.stretchHeight = false;

        EditorGUILayout.BeginVertical(
            cellStyle,
            GUILayout.Width(CellWidth),
            GUILayout.Height(CellHeight),
            GUILayout.ExpandWidth(false),
            GUILayout.ExpandHeight(false)
        );

        EditorGUILayout.LabelField(
            $"{x},{y}",
            EditorStyles.miniBoldLabel,
            GUILayout.Width(CellWidth - 8)
        );

        if (cell == null)
        {
            EditorGUILayout.LabelField(
                "NULL",
                EditorStyles.miniLabel
            );

            EditorGUILayout.EndVertical();
            return;
        }

        // Wall
        if (cell.Type == CellType.Wall)
        {
            EditorGUILayout.LabelField(
                "Wall",
                EditorStyles.miniBoldLabel,
                GUILayout.Width(CellWidth - 8)
            );

            EditorGUILayout.EndVertical();
            return;
        }

        // Room
        EditorGUILayout.LabelField(
            cell.RoomType.ToString(),
            EditorStyles.miniLabel,
            GUILayout.Width(CellWidth - 8)
        );

        string lightText =
            !cell.CanUseLight
                ? "-"
                : cell.OnLight
                    ? "L"
                    : "l";

        string monsterText =
            cell.HasRoomMonster
                ? "M"
                : "-";

        EditorGUILayout.LabelField(
            $"{lightText} {monsterText}",
            EditorStyles.miniLabel,
            GUILayout.Width(CellWidth - 8)
        );

        if (GUILayout.Button(
            selected ? "●" : "○",
            GUILayout.Width(CellWidth - 10),
            GUILayout.Height(17)
        ))
        {
            selectedCellIndex = index;

            GUI.FocusControl(null);
        }

        EditorGUILayout.EndVertical();
    }


    // =========================================================
    // 선택된 Cell 상세 정보
    // =========================================================

    private void DrawSelectedCell()
    {
        // 선택된 셀이 없는 경우
        if (selectedCellIndex < 0 ||
            selectedCellIndex >= CurrentMapData.Length)
        {
            EditorGUILayout.HelpBox(
                "수정할 Cell을 선택하세요.",
                MessageType.Info
            );

            return;
        }


        int x =
            selectedCellIndex % mapsize.x;

        int y =
            selectedCellIndex / mapsize.x;


        EditorGUILayout.LabelField(
            $"Selected Cell [{x}, {y}]",
            EditorStyles.boldLabel
        );


        // =====================================================
        // SerializedObject
        // =====================================================

        SerializedObject serializedObject =
            new SerializedObject(this);

        serializedObject.Update();


        SerializedProperty mapArray =
            serializedObject.FindProperty(
                nameof(CurrentMapData)
            );


        if (mapArray == null)
        {
            EditorGUILayout.HelpBox(
                "CurrentMapData를 찾을 수 없습니다.",
                MessageType.Error
            );

            return;
        }


        if (selectedCellIndex >= mapArray.arraySize)
        {
            EditorGUILayout.HelpBox(
                "선택된 Cell의 Index가 배열 범위를 벗어났습니다.",
                MessageType.Error
            );

            return;
        }


        SerializedProperty cell =
            mapArray.GetArrayElementAtIndex(
                selectedCellIndex
            );


        EditorGUILayout.BeginVertical(
            GUI.skin.box
        );


        // =====================================================
        // Type
        // =====================================================

        SerializedProperty type =
            cell.FindPropertyRelative(
                nameof(MapCellData.Type)
            );

        if (type != null)
        {
            EditorGUILayout.PropertyField(
                type
            );
        }


        // =====================================================
        // Room Type
        // =====================================================

        SerializedProperty roomType =
            cell.FindPropertyRelative(
                nameof(MapCellData.RoomType)
            );

        if (roomType != null)
        {
            EditorGUILayout.PropertyField(
                roomType
            );
        }


        EditorGUILayout.Space(5);


        // =====================================================
        // Light
        // =====================================================

        SerializedProperty canUseLight =
            cell.FindPropertyRelative(
                nameof(MapCellData.CanUseLight)
            );

        SerializedProperty onLight =
            cell.FindPropertyRelative(
                nameof(MapCellData.OnLight)
            );


        if (canUseLight != null)
        {
            EditorGUILayout.PropertyField(
                canUseLight
            );
        }


        // CanUseLight가 true일 때만 OnLight 출력
        if (canUseLight != null &&
            canUseLight.boolValue &&
            onLight != null)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(
                onLight
            );

            EditorGUI.indentLevel--;
        }


        EditorGUILayout.Space(5);


        // =====================================================
        // Monster
        // =====================================================

        SerializedProperty hasRoomMonster =
            cell.FindPropertyRelative(
                nameof(MapCellData.HasRoomMonster)
            );


        if (hasRoomMonster != null)
        {
            EditorGUILayout.PropertyField(
                hasRoomMonster
            );
        }


        EditorGUILayout.Space(5);


        // =====================================================
        // Tile
        // =====================================================

        SerializedProperty tile =
            cell.FindPropertyRelative(
                nameof(MapCellData.Tile)
            );


        if (tile != null)
        {
            EditorGUILayout.PropertyField(
                tile
            );
        }


        // =====================================================
        // Monster Spawn Points
        // =====================================================

        SerializedProperty monsterSpawnPoints =
            cell.FindPropertyRelative(
                nameof(MapCellData.MonsterSpawnPoints)
            );


        if (monsterSpawnPoints != null)
        {
            EditorGUILayout.PropertyField(
                monsterSpawnPoints,
                true
            );
        }


        EditorGUILayout.EndVertical();


        // =====================================================
        // 적용
        // =====================================================

        if (serializedObject.ApplyModifiedProperties())
        {
            EditorUtility.SetDirty(this);
        }
    }

#endif




    private void Awake()
    {
        Instance = this;
    }

    void ResetMapData()
    {
        CurrentMapData = new MapCellData[mapsize.y* mapsize.x];
        for (int i = 0; i < mapsize.y; i++)
        {
            for (int j = 0; j < mapsize.x; j++)
            {
                Set(j, i, new MapCellData());
            }
        }
    }

    public MapCellData GetPlayerCell()
    {
        Collider[] hits = Physics.OverlapSphere(
            GameManager.Instance.Player.position,
            0.05f,
            CellLayer);

        if (hits.Length > 0 && hits[0].TryGetComponent(out LogicalCellArea area))
        {
            return Get(area.coordinate.x, area.coordinate.y);
        }
        else return null;
    }

    public MapCellData Get(int x, int y)
    {
        return CurrentMapData[y * mapsize.x + x];
    }

    public void Set(int x, int y, MapCellData data)
    {
        CurrentMapData[y * mapsize.x + x] = data;
    }

    public void RegisterArea(Vector2Int coordinate, LogicalCellArea area)
    {
        int index = coordinate.y * mapsize.x + coordinate.x;
        CurrentMapData[index].Tile = area;
        CurrentMapData[index].Type = CellType.Room;
        CurrentMapData[index].RoomType = area.RoomType;
        CurrentMapData[index].CanUseLight = area.CanUseLight;
        CurrentMapData[index].MonsterSpawnPoints = area.MonsterSpawnPoints;
    }
}

[Serializable]
public class MapCellData
{
    public CellType Type;
    public RoomType RoomType;

    public bool CanUseLight;
    public bool OnLight;

    public bool HasRoomMonster;

    public LogicalCellArea Tile;

    public List<Transform> MonsterSpawnPoints;

    public MapCellData()
    {
        Type = CellType.Wall;
        RoomType = RoomType.Empty;
        CanUseLight = false;
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
    Security,
    Electric
}