using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MapPos
{
    public int x;
    public int y;
    public bool isOccupied; // For Dealing with multiple block sized rooms
    public void Reset(int _x, int _y) { x = _x; y = _y; isOccupied = false; }
    public MapPos(int _x, int _y) { x = _x; y = _y; isOccupied = false; }
}

public class LevelGenerator : MonoBehaviour
{
    List<MapPos> map;
    public List<GameObject> rooms; // a list of rooms
    private bool ExitPlaced; // selects a random room at the end
    private bool EntrancePlaced;

    public void Generate(int seed, int gridsize)
    {
        Random.InitState(seed);

        int totalsize = gridsize * gridsize;

        // Clears the map
        map.Clear();
        for (int _y = 0; gridsize > _y;_y++) // Fills the map with empty MapPoses
        {
            for (int _x = 0; gridsize > _x;_x++)
            {
                if (_x + (gridsize * _y) > map.Count) // if doesn't exist
                {
                    map.Add(new MapPos(_x, _y));
                    continue;
                }

                map[_x + (gridsize * _y)].Reset(_x, _y);
            }
        }

        int StartPos = Random.Range(0 , gridsize); // room where player starts.

        
    }

    private Vector3 GetPosition(int _x, int _y) // Converts the 2D position to 3D
    {
        return new Vector3(_x * 10, 0, _y * 10);
    }
}
