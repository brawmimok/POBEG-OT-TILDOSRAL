/*
using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public List<RoomPrefab> roomPrefabs;
    public float roomWidth = 11.5f;
    public float roomDepth = 15f;
    public int mapWidth = 5;
    public int mapHeight = 5;

    private Dictionary<Vector2Int, GameObject> placedRooms = new();

    private void Start()
    {
        //GenerateRandomMap();
    }

    private void GenerateRandomMap()
    {
        var startPosition = Vector2Int.zero;
        PlaceRoom(startPosition, "CorridorZ");

        for (var y = 0; y < mapHeight; y++)
        {
            for (var x = 0; x < mapWidth; x++)
            {
                var currentPos = new Vector2Int(x, y);
                if (placedRooms.ContainsKey(currentPos)) continue;

                var roomType = GetRandomRoomType();

                if (roomType == "CorridorX" && Random.Range(0, 2) == 0)
                    roomType = "CorridorZ";

                if (!CanPlaceRoom(currentPos, roomType))
                {
                    continue;
                }

                PlaceRoom(currentPos, roomType);
            }
        }
    }

    private bool CanPlaceRoom(Vector2Int gridPos, string roomType)
    {
        Vector2Int[] neighbors = {
            gridPos + Vector2Int.up,
            gridPos + Vector2Int.down,
            gridPos + Vector2Int.left,
            gridPos + Vector2Int.right
        };

        foreach (var neighbor in neighbors)
        {
            if (placedRooms.ContainsKey(neighbor))
            {
                var neighboringRoom = placedRooms[neighbor];

                if (roomType == "CorridorZ" && !HasExit(neighboringRoom, "Z"))
                {
                    return false;
                }

                if (roomType == "CorridorX" && !HasExit(neighboringRoom, "X"))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool HasExit(GameObject room, string axis)
    {

        if (axis == "Z" && room.name.Contains("CorridorZ"))
        {
            return true;
        }

        if (axis == "X" && room.name.Contains("CorridorX"))
        {
            return true;
        }

        return false;
    }

    private void PlaceRoom(Vector2Int gridPos, string roomName)
    {
        if (placedRooms.ContainsKey(gridPos)) return;

        var prefab = roomPrefabs.Find(r => r.name == roomName).prefab;
        if (prefab == null)
        {
            Debug.LogWarning($"Prefab not found: {roomName}");
            return;
        }

        var rotation = Quaternion.identity;
        if (roomName == "CorridorX")
            rotation = Quaternion.Euler(0, 90, 0);

        var worldPos = new Vector3(gridPos.x * roomWidth, 0, gridPos.y * roomDepth);
        var instance = Instantiate(prefab, worldPos, rotation, transform);
        placedRooms.Add(gridPos, instance);
    }

    private string GetRandomRoomType()
    {
        var rand = Random.Range(0, 100);
        if (rand < 90) return "CorridorZ";  // 60% шанс на коридор по Z
        else if (rand < 6) return "CorridorX"; // 20% шанс на коридор по X
        else return "Intersection";  // 20% шанс на развилку
    }
}
*/