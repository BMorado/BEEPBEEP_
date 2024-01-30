using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Unity.Collections;
using Unity.VisualScripting;
using System;
using System.Runtime.InteropServices;

public class MapManager : MonoBehaviour
{
    #region mapComponents
    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject finishRoom;
    [SerializeField] private GameObject normalRoom;
    [SerializeField] private GameObject shopRoom;
    [SerializeField] private GameObject blank;
    [SerializeField] private GameObject horizontal;
    [SerializeField] private GameObject vertical;
    [SerializeField] private GameObject forwardslash;
    [SerializeField] private GameObject backslash;

    private static int X_MAX = 20;
    private static int Y_MAX = 20;
    private static int MAX_NUMBER_OF_ROOMS = 40;
    private Vector2[] rooms = new Vector2[MAX_NUMBER_OF_ROOMS];
    private int roomCounter = 0;
    private Dictionary<Vector2, List<Vector2>> roomHashMap;

    #endregion

    // Start is called before the first frame update
    public void Start()
    {

        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

        int x = UnityEngine.Random.Range((X_MAX * -1), X_MAX);
        int y = UnityEngine.Random.Range((Y_MAX * -1), Y_MAX);

        Debug.Log(x + ", " + y);

        rooms[roomCounter] = new Vector2(x, y);
        Instantiate(startRoom, rooms[roomCounter], quaternion.identity);
        roomCounter++;

        // roomHashMap.Add(rooms[roomCounter], null);

        int count = 0;
        while (count < MAX_NUMBER_OF_ROOMS - 1) {
            SpawnAdjacent(normalRoom, rooms[UnityEngine.Random.Range(0, roomCounter)]);
            count ++;
        }
    }

    public void SpawnAdjacent(GameObject prefabTile, Vector2 root)
    {
        List<Vector2> adjacentCoords = GetAvailableAdjacent(root);
        int index = UnityEngine.Random.Range(0, adjacentCoords.Count - 1);
        Instantiate(prefabTile, adjacentCoords[index], quaternion.identity);
        rooms[roomCounter] = adjacentCoords[index];
        roomCounter++;

    }

    public List<Vector2> GetAvailableAdjacent(Vector2 root)
    {
        List<Vector2> allAdjacentCoords = GetAdjacentCoords(root);
        List<Vector2> availableAdjacentCoords = new List<Vector2>();

        foreach (Vector2 coord in allAdjacentCoords)
        {
            if (Math.Abs(coord.x) < X_MAX && Math.Abs(coord.y) < Y_MAX)
            {
                availableAdjacentCoords.Add(coord);
            }
        }

        foreach (Vector2 coord in rooms)
        {
            if (availableAdjacentCoords.Count == 1)
            {
                break;
            }
            else if (availableAdjacentCoords.Contains(coord))
            {
                availableAdjacentCoords.Remove(coord);
            }
        }

        return availableAdjacentCoords;
    }

    List<Vector2> GetAdjacentCoords(Vector2 position)
    {
        List<Vector2> adjacentCoords = new List<Vector2>();

        adjacentCoords.Add(new Vector2(position.x, position.y + 2)); // Add top coordinate
        adjacentCoords.Add(new Vector2(position.x, position.y - 2)); // Add bottom coordinate
        adjacentCoords.Add(new Vector2(position.x - 2, position.y)); // Add left coordinate
        adjacentCoords.Add(new Vector2(position.x + 2, position.y)); // Add right coordinate
        adjacentCoords.Add(new Vector2(position.x + 2, position.y - 2)); // Add bottom right coordinate
        adjacentCoords.Add(new Vector2(position.x - 2, position.y + 2)); // Add top left coordinate
        adjacentCoords.Add(new Vector2(position.x - 2, position.y - 2)); // Add bottom left coordinate
        adjacentCoords.Add(new Vector2(position.x + 2, position.y + 2)); // Add top right coordinate

        return adjacentCoords;
    }
}
