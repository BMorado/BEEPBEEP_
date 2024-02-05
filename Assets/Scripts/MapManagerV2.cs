using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Unity.Collections;
using Unity.VisualScripting;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;
using System.Linq;

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
    [SerializeField] private static int X_MAX = 5;
    [SerializeField] private static int Y_MAX = 5;
    [SerializeField] private static int MAX_NUMBER_OF_ROOMS = 10;

    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        Dictionary<Vector2, GameObject> gameBoard = InitializeGameBoard();
        gameBoard = AddRooms(gameBoard);
        SpawnGameBoard(gameBoard);

    }

    private Dictionary<Vector2, GameObject> AddRooms(Dictionary<Vector2, GameObject> gameBoard)
    {
        List<Vector2> availableAdjacentRooms;
        List<Vector2> existingRooms;
        bool added; // keeps track of whether a room was added in each iteration
        int count = 0; // for iteration (keeps track of number of rooms as well)
        int randomExistingRoomIndex; // randomly chooses which room to spawn a new one around
        int adjacentRoomIndex; // random index of available adjacent rooms

        // randomizes the iteration that the final room will be spawned (after the first half is spawned)
        int finalRoomIndex = UnityEngine.Random.Range(MAX_NUMBER_OF_ROOMS / 2, MAX_NUMBER_OF_ROOMS - 2);

        while (count < MAX_NUMBER_OF_ROOMS - 1)
        {
            // make a list of all keys where GameObject values are not blank tiles;
            // these will be rooms, as it assumes that no paths have been generated yet
            existingRooms = gameBoard.Where(pair => pair.Value != blank).Select(pair => pair.Key).ToList();
            randomExistingRoomIndex = UnityEngine.Random.Range(0, existingRooms.Count);
            added = false; // so far, a room has not been added

            // from the randomly selected room, store all available adjacent room spaces
            availableAdjacentRooms = GetAvailableAdjacentRooms(gameBoard, existingRooms[randomExistingRoomIndex]);

            // will not add a room if there are no available adjacent coords
            if (availableAdjacentRooms != null)
            {
                // adds final room if this iteration is the "Final Room" iteration
                if (finalRoomIndex == count)
                {
                    adjacentRoomIndex = UnityEngine.Random.Range(0, availableAdjacentRooms.Count - 1);
                    gameBoard[availableAdjacentRooms[adjacentRoomIndex]] = finishRoom; // changes the mapped GameObject to finishRoom
                    added = true; // room was added successfully
                }
                // adds a normal room otherwise
                else
                {
                    adjacentRoomIndex = UnityEngine.Random.Range(0, availableAdjacentRooms.Count - 1);
                    gameBoard[availableAdjacentRooms[adjacentRoomIndex]] = normalRoom; // changes the mapped GameObject to normalRoom
                    added = true; // room was added successfully
                }
            }

            // count (of rooms) will not increase until iteration actually adds a room
            if (added)
            {
                count++;
            }
        }
        return gameBoard;
    }

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

    private List<Vector2> GetAvailableAdjacentRooms(Dictionary<Vector2, GameObject> gameBoard, Vector2 root)
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
    private void SpawnGameBoard(Dictionary<Vector2, GameObject> gameBoard)
    {
        foreach (KeyValuePair<Vector2, GameObject> tile in gameBoard)
        {
            Instantiate(tile.Value, tile.Key, quaternion.identity);
        }
    }

    /* Initialize the entire space of the game with the start room and store all coords in dictionary */
    private Dictionary<Vector2, GameObject> InitializeGameBoard()
    {
        Dictionary<Vector2, GameObject> gameBoard = new Dictionary<Vector2, GameObject>(); // instantiate Dictionary and store in local variable
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

        return gameBoard;
    }
}