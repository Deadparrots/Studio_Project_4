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
    List<MapPos> map = new List<MapPos>();
    public List<GameObject> rooms = new List<GameObject>(); // a list of rooms
    private bool ExitPlaced = false; // selects a random room at the end
    private bool EntrancePlaced = false;

    private void Start()
    {
        Generate(0, 3);
    }

    public void Generate(int seed, int gridsize)
    {
        Random.InitState(seed);

        int totalsize = gridsize * gridsize;

        // Clears the map
        if (map.Count != 0)
            map.Clear();
        for (int _y = 0; gridsize > _y;_y++) // Fills the map with empty MapPoses
        {
            for (int _x = 0; gridsize > _x;_x++)
            {
                map.Add(new MapPos(_x, _y));
            }
        }

        int StartPos = Random.Range(0 , gridsize); // room where player starts.
        GameObject temp = Instantiate(rooms[Random.Range(0, rooms.Count)]);
        temp.transform.position = GetPosition2(StartPos, gridsize);
        Debug.Log(temp.transform.position);

    }

    

    private Vector3 GetPosition(int _x, int _y) // Converts the 2D position to 3D
    {
        return new Vector3(_x * 10, 0, _y * 10);
    }

    private Vector3 GetPosition2(int combined, int gridsize) // Converts a combined position to 2d
    {
        int y = combined / gridsize;
        int x = combined % gridsize;
        return GetPosition(x, y);
    }
}
