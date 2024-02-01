using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Unity.Collections;
using Unity.VisualScripting;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;

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

    [SerializeField] private static int X_MAX = 3;
    [SerializeField] private static int Y_MAX = 3;
    private static int MAX_NUMBER_OF_ROOMS = 5;
    private Vector2[] rooms = new Vector2[MAX_NUMBER_OF_ROOMS];
    private int roomCounter = 0;
    private List<Vector2> paths = new List<Vector2>();

    #endregion

    public void Awake() {

    }
    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("problem");
        // UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

        int x = UnityEngine.Random.Range((X_MAX * -1), X_MAX);
        int y = UnityEngine.Random.Range((Y_MAX * -1), Y_MAX);

        rooms[roomCounter] = new Vector2(x, y);
        Instantiate(startRoom, rooms[roomCounter], quaternion.identity);
        roomCounter++;

        // roomHashMap.Add(rooms[roomCounter], null);

        int count = 0;
        int temp = 0;
        int finalRoomIndex = UnityEngine.Random.Range(MAX_NUMBER_OF_ROOMS / 2, MAX_NUMBER_OF_ROOMS - 2);
        while (count < MAX_NUMBER_OF_ROOMS - 1)
        {
            temp = roomCounter;
            if (finalRoomIndex == count)
            {
                SpawnAdjacentRoom(finishRoom, rooms[UnityEngine.Random.Range(0, roomCounter)]);
            }
            else
            {
                SpawnAdjacentRoom(normalRoom, rooms[UnityEngine.Random.Range(0, roomCounter)]);
            }

            if (temp != roomCounter)
            {
                count++;
            }
        }

        List<Vector2> availableAdjacentPaths;
        int localMaxPaths;
        int index;
        foreach (Vector2 room in rooms)
        {

            availableAdjacentPaths = GetAvailableAdjacentPaths(room);
            
            Debug.Log(room);
            String m = "Before {";
            foreach (Vector2 v in availableAdjacentPaths)
            {
                m += "( " + v.x + ", " + v.y + ") ";
            }
            Debug.Log(m + "}");

            if (availableAdjacentPaths.Count != 0)
            {
                localMaxPaths = UnityEngine.Random.Range(1, availableAdjacentPaths.Count - 1);
                Debug.Log("Max Paths: " + localMaxPaths);
                count = 0;
                while (count < localMaxPaths)
                {
                    index = UnityEngine.Random.Range(0, availableAdjacentPaths.Count - 1);
                    Debug.Log("Index: " + index);

                    GameObject pathTile = GetPathTile(room, availableAdjacentPaths[index]);
                    Instantiate(pathTile, availableAdjacentPaths[index], quaternion.identity);
                    paths.Add(availableAdjacentPaths[index]);
                    m = "paths: {";
                    foreach (Vector2 v in paths)
                    {
                        m += "( " + v.x + ", " + v.y + ") ";
                    }
                    Debug.Log(m + "}");
                    availableAdjacentPaths.Remove(availableAdjacentPaths[index]);
                    count++;
                }
            }
            m = "After {";
            foreach (Vector2 v in availableAdjacentPaths)
            {
                m += "( " + v.x + ", " + v.y + ") ";
            }
            Debug.Log(m + "}");
        }
    }

    private void SpawnAdjacentRoom(GameObject prefabTile, Vector2 root)
    {
        List<Vector2> adjacentCoords = GetAvailableAdjacentRooms(root);
        if (adjacentCoords.Count != 0)
        {
            int index = UnityEngine.Random.Range(0, adjacentCoords.Count - 1);
            Instantiate(prefabTile, adjacentCoords[index], quaternion.identity);
            rooms[roomCounter] = adjacentCoords[index];
            roomCounter++;
        }
    }

    // private void SpawnAdjacentPath(GameObject prefabTile, Vector2 root)
    // {
    //     List<Vector2> adjacentCoords = GetAvailableAdjacentPaths(root);
    //     if (adjacentCoords.Count != 0)
    //     {
    //         int index = UnityEngine.Random.Range(0, adjacentCoords.Count - 1);

    //         Instantiate(prefabTile, adjacentCoords[index], quaternion.identity);
    //         rooms[roomCounter] = adjacentCoords[index];
    //         roomCounter++;
    //     }
    // }

    private List<Vector2> GetAvailableAdjacentRooms(Vector2 root)
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
            if (availableAdjacentCoords.Count == 0)
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

    private List<Vector2> GetAvailableAdjacentPaths(Vector2 root)
    {
        List<Vector2> allAdjacentCoords = GetAdjacentCoords(root);
        List<Vector2> availableAdjacentCoords = new List<Vector2>();
        Vector2 temp;

        foreach (Vector2 coord in allAdjacentCoords)
        {
            if (Array.IndexOf(rooms, coord) != -1)
            {
                if (!paths.Contains(coord))
                {
                    temp = coord;
                    temp.x = (temp.x + root.x) / 2;
                    temp.y = (temp.y + root.y) / 2;

                    availableAdjacentCoords.Add(temp);
                }
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

    GameObject GetPathTile(Vector2 A, Vector2 B)
    {

        if (A.x == B.x)
        {
            return vertical;
        }
        else if (A.y == B.y)
        {
            return horizontal;
        }
        else if (A.x > B.x)
        {
            if (A.y > B.y)
            {
                return forwardslash;
            }
            else
            {
                return backslash;
            }
        }
        else if (A.x < B.x)
        {
            if (A.y < B.y)
            {
                return forwardslash;
            }
            else
            {
                return backslash;
            }
        }
        else
        {
            return blank;
        }
    }
}