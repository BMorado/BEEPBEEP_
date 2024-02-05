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

public class MapManager : MonoBehaviour
{
    #region Map Components

    // All Tiles needed for the map as GameObject
    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject finishRoom;
    [SerializeField] private GameObject normalRoom;
    [SerializeField] private GameObject shopRoom;
    [SerializeField] private GameObject blankTile;
    [SerializeField] private GameObject horizontal;
    [SerializeField] private GameObject vertical;
    [SerializeField] private GameObject forwardslash;
    [SerializeField] private GameObject backslash;

    #endregion

    # region Global Variables

    // Global Variables for map generation
    [SerializeField] private static int X_MAX = 9; // x-axis; can go + or -
    [SerializeField] private static int Y_MAX = 4; // y-axis; can go + or -
    [SerializeField] private static int MAX_NUMBER_OF_ROOMS = 30; // if too high in relation to X & Y MAX, then infinite-loop (TO-FIX)

    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        Dictionary<Vector2, GameObject> gameBoard = InitializeGameBoard(); // initialize Game Board with start room
        gameBoard = AddRooms(gameBoard); // add all rooms including Finish room
        gameBoard = AddPaths(gameBoard); // add all paths that connect to those rooms added in the line before
        SpawnGameBoard(gameBoard); // output all tiles in the saved dictionary
    }

    /*
    * Randomly adds rooms including the finish room until MAX_NUMBER_OF_ROOMS has been reached 
    */
    private Dictionary<Vector2, GameObject> AddRooms(Dictionary<Vector2, GameObject> gameBoard)
    {
        List<Vector2> availableAdjacentRooms;
        List<Vector2> existingRooms;
        Vector2 roomToAdd; // stores room to add (will randomize from available list)
        bool added; // keeps track of whether a room was added in each iteration
        int count = 0; // for iteration (keeps track of number of rooms as well)
        int randomExistingRoomIndex; // randomly chooses which room to spawn a new one around

        // randomizes the iteration that the final room will be spawned (after the first half is spawned)
        int finalRoomIndex = UnityEngine.Random.Range(MAX_NUMBER_OF_ROOMS / 2, MAX_NUMBER_OF_ROOMS - 2);

        // loop until MAX_NUMBER_OF_ROOMS has been reached
        while (count < MAX_NUMBER_OF_ROOMS - 1)
        {
            // make a list of all keys where GameObject values are not blank tiles;
            // these will be rooms, as it assumes that no paths have been generated yet
            existingRooms = gameBoard.Where(pair => pair.Value != blankTile).Select(pair => pair.Key).ToList();
            randomExistingRoomIndex = UnityEngine.Random.Range(0, existingRooms.Count);
            added = false; // so far, a room has not been added

            // from the randomly selected room, store all available adjacent room spaces
            availableAdjacentRooms = GetAvailableAdjacentRooms(gameBoard, existingRooms[randomExistingRoomIndex]);

            // will not add a room if there are no available adjacent room coords
            if (availableAdjacentRooms != null)
            {
                // adds final room if this iteration is the "Final Room" iteration
                if (finalRoomIndex == count)
                {
                    roomToAdd = availableAdjacentRooms[UnityEngine.Random.Range(0, availableAdjacentRooms.Count - 1)];
                    gameBoard[roomToAdd] = finishRoom; // changes the mapped GameObject to finishRoom
                    added = true; // room was added successfully
                }
                // adds a normal room otherwise
                else
                {
                    roomToAdd = availableAdjacentRooms[UnityEngine.Random.Range(0, availableAdjacentRooms.Count - 1)];
                    gameBoard[roomToAdd] = normalRoom; // changes the mapped GameObject to normalRoom
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


    private Dictionary<Vector2, GameObject> AddPaths(Dictionary<Vector2, GameObject> gameBoard)
    {
        // make a list of all keys where GameObject values are not blank tiles;
        // these will all be rooms still, as it assumes that no paths have been generated yet
        List<Vector2> existingRooms = gameBoard.Where(pair => pair.Value != blankTile).Select(pair => pair.Key).ToList();
        List<Vector2> availableAdjacentPaths;
        Vector2 pathToAdd; // stores path to add (will randomize from available list)
        GameObject pathTile; // variable for different path types
        int localMaxPaths; // stores the instance's number of paths generated per room(iteration)
        int count; // keeps track of iterations


        // loop through list of existing rooms
        foreach (Vector2 room in existingRooms)
        {
            // store all available adjacent path spaces
            availableAdjacentPaths = GetAvailableAdjacentPaths(gameBoard, room);

            // will not add a path if there are no available adjacent path coords
            if (availableAdjacentPaths != null)
            {
                localMaxPaths = UnityEngine.Random.Range(1, availableAdjacentPaths.Count - 1);
                count = 0; // set to first iteration (ie. 0)

                // localMaxPaths is however many iterations each room will go through to spawn a path
                while (count < localMaxPaths)
                {
                    pathToAdd = availableAdjacentPaths[UnityEngine.Random.Range(0, availableAdjacentPaths.Count - 1)];
                    pathTile = GetPathTile(room, pathToAdd);

                    // Check just to make sure pathTile is not null
                    if (pathTile != null)
                    {
                        gameBoard[pathToAdd] = pathTile;
                        availableAdjacentPaths.Remove(pathToAdd); // removes pathToAdd from available list after adding
                    }
                    count++;
                }
            }
        }
        return gameBoard;
    }


    /* 
    * Extension of Adjacent coords, filters out any spaces that are not blank in any of the returned coords 
    */
    private List<Vector2> GetAvailableAdjacentRooms(Dictionary<Vector2, GameObject> gameBoard, Vector2 root)
    {
        List<Vector2> allAdjacentCoords = GetAdjacentCoords(root);
        List<Vector2> availableAdjacentRooms = new List<Vector2>();

        // loop through each of the returned adjacent coords
        foreach (Vector2 coord in allAdjacentCoords)
        {
            // Check if coordinate is in the game board (not including the border)
            if (Math.Abs(coord.x) <= X_MAX && Math.Abs(coord.y) <= Y_MAX)
            {
                // Check if any the coordinate is available (blank means available)
                if (gameBoard[coord] == blankTile)
                {
                    availableAdjacentRooms.Add(coord);
                }
            }
        }

        // if there are no available adjacent rooms available, then method returns null
        if (availableAdjacentRooms.Count == 0)
        {
            availableAdjacentRooms = null;
        }

        return availableAdjacentRooms;
    }


    /* 
    * Extension of Adjacent coords, decides whether there is a potential path space 
    */
    private List<Vector2> GetAvailableAdjacentPaths(Dictionary<Vector2, GameObject> gameBoard, Vector2 root)
    {
        List<Vector2> allAdjacentCoords = GetAdjacentCoords(root);
        List<Vector2> availableAdjacentPaths = new List<Vector2>();
        Vector2 pathPosition = new Vector2(); // keeps track of available path to store

        // loop through each of the returned adjacent coords
        foreach (Vector2 coord in allAdjacentCoords)
        {
            // make sure that the gameboord actually has this coordinate
            if (gameBoard.ContainsKey(coord))
            {
                // check if any of these is already a room tile
                if (gameBoard[coord] != blankTile)
                {
                    pathPosition.x = (coord.x + root.x) / 2; // midpoint equation for x
                    pathPosition.y = (coord.y + root.y) / 2; // midpoint equation for x

                    // if pathposition is not a blankTile, that means a path has already been added on these coords
                    if (gameBoard[pathPosition] == blankTile)
                    {
                        availableAdjacentPaths.Add(pathPosition);

                    }
                }
            }
        }

        // if there are no available adjacent paths available, then method returns null
        if (availableAdjacentPaths.Count == 0)
        {
            availableAdjacentPaths = null;
        }
        return availableAdjacentPaths;
    }


    /* 
    * Adjacent Coords in this case is any 8 directions by 2 (NOT BY 1) 
    */
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


    /* 
    * Based on two coordinates, decide which directional path to return 
    */
    GameObject GetPathTile(Vector2 A, Vector2 B)
    {
        // both have same x means vertical line 
        if (A.x == B.x)
        {
            return vertical;
        }
        // both have same y means horizontal line
        else if (A.y == B.y)
        {
            return horizontal;
        }
        else if (A.x > B.x)
        {
            // x and y are both greater means forwardslash
            if (A.y > B.y)
            {
                return forwardslash;
            }
            // one of x,y is greater, other is smaller means backslash 
            else
            {
                return backslash;
            }
        }
        else if (A.x < B.x)
        {
            // x and y are both lower means forwardslash
            if (A.y < B.y)
            {
                return forwardslash;
            }
            // one of x,y is greater, other is smaller means backslash
            else
            {
                return backslash;
            }
        }
        // Should not hit this state. If it does, there is an error
        else
        {
            Debug.Log("Error, GetPathTile returned null");
            return null;
        }
    }


    /* 
    * Spawns the entire board based on dictionary 
    */
    private void SpawnGameBoard(Dictionary<Vector2, GameObject> gameBoard)
    {
        foreach (KeyValuePair<Vector2, GameObject> tile in gameBoard)
        {
            Instantiate(tile.Value, tile.Key, quaternion.identity);
        }
    }


    /* 
    * Initialize the entire space of the game with the start room and store all coords in dictionary 
    */
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
                gameBoard.Add(position, blankTile); // coordinate is the key, and a blank space is its value
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