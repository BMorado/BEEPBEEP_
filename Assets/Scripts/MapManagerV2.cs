using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Unity.Collections;
using Unity.VisualScripting;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;

public class MapManagerV2 : MonoBehaviour
{
    #region Map Components

    // All Tiles needed for the map as GameObject
    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject finishRoom;
    [SerializeField] private GameObject normalRoom;
    [SerializeField] private GameObject shopRoom;
    [SerializeField] private GameObject blank;
    [SerializeField] private GameObject horizontal;
    [SerializeField] private GameObject vertical;
    [SerializeField] private GameObject forwardslash;
    [SerializeField] private GameObject backslash;

    #endregion

    # region Global Variables

    // Global Variables for map generation
    [SerializeField] private static int X_MAX = 2;
    [SerializeField] private static int Y_MAX = 2;
    [SerializeField] private static int MAX_NUMBER_OF_ROOMS = 5;
    private int roomCounter = 0;
    private Dictionary<Vector2, GameObject> gameBoard = new Dictionary<Vector2, GameObject>();

    #endregion

    public void Awake()
    {

    }

    // Start is called before the first frame update
    public void Start()
    {
        InitializeGameBoard();

        SpawnGameBoard();

    }

    // private void SpawnAdjacentRoom(GameObject prefabTile, Vector2 root)
    // {
    //     List<Vector2> adjacentCoords = GetAvailableAdjacentRooms(root);
    //     if (adjacentCoords.Count != 0)
    //     {
    //         int index = UnityEngine.Random.Range(0, adjacentCoords.Count - 1);
    //         Instantiate(prefabTile, adjacentCoords[index], quaternion.identity);
    //         rooms[roomCounter] = adjacentCoords[index];
    //         roomCounter++;
    //     }
    // }

    // // private void SpawnAdjacentPath(GameObject prefabTile, Vector2 root)
    // // {
    // //     List<Vector2> adjacentCoords = GetAvailableAdjacentPaths(root);
    // //     if (adjacentCoords.Count != 0)
    // //     {
    // //         int index = UnityEngine.Random.Range(0, adjacentCoords.Count - 1);

    // //         Instantiate(prefabTile, adjacentCoords[index], quaternion.identity);
    // //         rooms[roomCounter] = adjacentCoords[index];
    // //         roomCounter++;
    // //     }
    // // }

    private List<Vector2> GetAvailableAdjacentRooms(Vector2 root)
    {
        List<Vector2> allAdjacentCoords = GetAdjacentCoords(root);
        List<Vector2> availableAdjacentCoords = new List<Vector2>();

        foreach (Vector2 coord in allAdjacentCoords)
        {
            // Check if coordinate is in the game board (not including the border)
            if (Math.Abs(coord.x) <= X_MAX && Math.Abs(coord.y) <= Y_MAX)
            {
                // Check if any the coordinate is available (blank means available)
                if (gameBoard[coord] == blank)
                {
                    availableAdjacentCoords.Add(coord);
                }
            }
        }

        // if there are no available adjacent rooms available, then method returns null
        if (availableAdjacentCoords.Count == 0)
        {
            availableAdjacentCoords = null;
        }

        return availableAdjacentCoords;
    }

    // private List<Vector2> GetAvailableAdjacentPaths(Vector2 root)
    // {
    //     List<Vector2> allAdjacentCoords = GetAdjacentCoords(root);
    //     List<Vector2> availableAdjacentCoords = new List<Vector2>();
    //     Vector2 temp;

    //     foreach (Vector2 coord in allAdjacentCoords)
    //     {
    //         if (Array.IndexOf(rooms, coord) != -1)
    //         {
    //             if (!paths.Contains(coord))
    //             {
    //                 temp = coord;
    //                 temp.x = (temp.x + root.x) / 2;
    //                 temp.y = (temp.y + root.y) / 2;

    //                 availableAdjacentCoords.Add(temp);
    //             }
    //         }
    //     }
    //     return availableAdjacentCoords;
    // }

    // 

    /* Adjacent Coords in this case is any 8 directions by 2 (NOT BY 1) */
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

    // GameObject GetPathTile(Vector2 A, Vector2 B)
    // {

    //     if (A.x == B.x)
    //     {
    //         return vertical;
    //     }
    //     else if (A.y == B.y)
    //     {
    //         return horizontal;
    //     }
    //     else if (A.x > B.x)
    //     {
    //         if (A.y > B.y)
    //         {
    //             return forwardslash;
    //         }
    //         else
    //         {
    //             return backslash;
    //         }
    //     }
    //     else if (A.x < B.x)
    //     {
    //         if (A.y < B.y)
    //         {
    //             return forwardslash;
    //         }
    //         else
    //         {
    //             return backslash;
    //         }
    //     }
    //     else
    //     {
    //         return blank;
    //     }
    // }

    /* Spawns the entire board based on dictionary */
    private void SpawnGameBoard()
    {
        foreach (KeyValuePair<Vector2, GameObject> tile in gameBoard)
        {
            Instantiate(tile.Value, tile.Key, quaternion.identity);
        }
    }

    /* Initialize the entire space of the game with the start room and store all coords in dictionary */
    private void InitializeGameBoard()
    {

        Vector2 position; // temporary position while looping

        // Start loop from (-X_MAX - 1) to (X_MAX + 1); +-1 is the border (filled with blanks)
        for (int i = ((X_MAX * -1) - 1); i < (X_MAX + 2); i++)
        {
            // Start loop from (-Y_MAX - 1) to (Y_MAX + 1); +-1 is the border (filled with blanks)
            for (int j = ((Y_MAX * -1) - 1); j < (Y_MAX + 2); j++)
            {
                position.x = i; // x-axis of each coordinate
                position.y = j; // y-axis of each coordinate
                gameBoard.Add(position, blank); // coordinate is the key, and a blank space is its value
            }
        }

        // Decides Start Room location and changes tile in Dictionary
        int x = UnityEngine.Random.Range((X_MAX * -1), X_MAX); // x-coord
        int y = UnityEngine.Random.Range((Y_MAX * -1), Y_MAX); // y-coord
        Vector2 startPos = new Vector2(x, y); // creates Vector2 object for the start position
        gameBoard[startPos] = startRoom; // replaces key with start Room
    }
}