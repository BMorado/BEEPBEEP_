
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
public class MapManagerUpdated : MonoBehaviour
{
    #region  Game Objects
    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject finishRoom;
    [SerializeField] private GameObject normalRoom;
    [SerializeField] private GameObject shopRoom;
    [SerializeField] private GameObject blank;
    [SerializeField] private GameObject horizontal;
    [SerializeField] private GameObject  vertical;
    [SerializeField] private GameObject forwardslash;
    [SerializeField] private GameObject backslash;
    #endregion
    
    [SerializeField] private int maxRooms = 20;
    public int placedRooms;
    public int placedHalls;
    private readonly Vector2[] _space =
    {
        new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
        new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1)
    };
    
    // holds all the rooms that have been placed 
    private  HashSet<Vector2> _placedRooms = new HashSet<Vector2>();
    private  HashSet<Vector2> _placedHalls = new HashSet<Vector2>();
    // Start is called before the first frame update
    void Start()
    {
        Stopwatch time = new Stopwatch();
        time.Start();
        // make random start room position 
        Vector2 startPos = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        // spawn start room 
        Instantiate(startRoom, startPos, quaternion.identity);
        // add start room to set
        _placedRooms.Add(startPos);
        GetAroundRoom(startPos);
        time.Stop();
        Debug.Log(time.ElapsedMilliseconds);
    }
    void PlaceRoom(GameObject roomType,Vector2 pos)
    {
        
        if (!_placedRooms.Contains(pos))
        {
            // just so we can see how many rooms were placed
            placedRooms++;
            Instantiate(roomType, pos, quaternion.identity);
            // add room to set 
            _placedRooms.Add(pos);
            // MaxRooms is not the max rooms it more the amount of passes 
            while (_placedRooms.Count < maxRooms)
            {
                GetAroundRoom(pos);
            }
        }
    }
    void  PlaceHall(GameObject hallType,Vector2 pos)
    {
        // only place a hall if there is not a hall already there
        if (!_placedHalls.Contains(pos))
        {
            // this is just so we can see how many halls were placed
            placedHalls++;
            Instantiate(hallType, pos, quaternion.identity);
            // add hall to the set
            _placedHalls.Add(pos);
        }
    }
    void GetAroundRoom(Vector2 pos)
    {
        // for each space around a room check all the blocks around it 
        foreach (Vector2 space in _space)
        {
            int hallType = Array.IndexOf(_space, space);
            int rng = Random.Range(0, 100);
            // if there is NOT a placed room or hall around spawn a hall and a room only if the odds allow it
            if (!_placedRooms.Contains(space) && (rng <50)&&!_placedHalls.Contains(space))
            {
                // the worst possible way to determine what hall needs to be placed
                if (hallType is 0 or 4)
                {
                    PlaceHall(vertical,pos+space);
                }
                else if(hallType is 1 or 5)
                {
                    PlaceHall(forwardslash,pos+space);
                }
                else if (hallType is 2 or 6)
                {
                    PlaceHall(horizontal,pos+space);
                }
                else if (hallType is 3 or 7)
                {
                    PlaceHall(backslash,pos+space);
                }
                else
                {
                    PlaceHall(blank,pos+space);
                }
                // if we have a hall i know i have to have a room so place the room at the end of the hall 
                PlaceRoom(normalRoom,pos+(space*2));
            }
        }
    }
}
