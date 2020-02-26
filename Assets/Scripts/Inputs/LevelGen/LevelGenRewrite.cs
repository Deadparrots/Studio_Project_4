using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenRewrite : MonoBehaviour
{

    // will probably do on free time
    List<bool> map = new List<bool>();
    public List<GameObject> prefabs = new List<GameObject>();

    public GameObject wall = null; // used to fill in a doorway that has been blocked
    private int gridsize;

    public void GenerateMap(int _GridSize, int _Seed)
    {
        gridsize = _GridSize;
        Generate(_Seed);
    }

    private void Generate(int Seed)
    {
        Random.InitState(Seed);

        int totalsize = gridsize * gridsize;

        // Resets the Map
        {
            if (map.Count != 0)
                map.Clear();
            for (int _y = 0; gridsize > _y; _y++) // Fills the map with empty MapPoses
                for (int _x = 0; gridsize > _x; _x++)
                    map.Add(false);
         }

        int StartPos = Random.Range(0, gridsize * gridsize); // room where generator starts.

        GameObject StartingRoom = Instantiate(prefabs[Random.Range(0, prefabs.Count)]);
        StartingRoom.transform.position = ConvertToWorldPos(StartPos);
        RoomRewrite StartingRoomInfo = StartingRoom.GetComponent<RoomRewrite>();
        StartingRoomInfo.position = new Vector2Int(StartPos % gridsize, StartPos / gridsize);
        WriteToMap(StartingRoomInfo.position, StartingRoomInfo.SizeX, StartingRoomInfo.SizeY);

        PrintInfo(StartingRoomInfo);
    }

    private void Generate(RoomRewrite room)
    {
        for (int connector = 0; room.Connectors.Count > connector;connector++)
        {
            if (room.Connectors[connector].processed) // One Connector should always be processed.
                continue;

            Vector2Int position = room.GetConnectorMapPosition(connector);
            int mapindex = (position.y * gridsize) + position.x;
            if (position.x > gridsize || position.y > gridsize || 0 > position.x || 0 > position.y) // if position will be out of bounds, spawn a wall
            {
                SpawnWall(room, connector);
                continue;
            }

            Vector2Int Nextposition = room.GetConnectorMapExitPosition(connector); // This is where a room will spawn.
            RoomRewrite.Rotation RequiredRotationForConnector = 
                (RoomRewrite.Rotation)(((int)(room.Connectors[connector].Exit[(int)room.rotation]) + 2 )% 4); // + 2 since the opposite direction is +2, %4 to wraparound the values

            // Plan is to check through all available rooms for whether they can fit, add them to a list and random out one of them.
            List<GameObject> validRooms = new List<GameObject>();
            {
                // do a loop or whatever to add to list
            }

            if (validRooms.Count != 0)
            {
                GameObject tobeSpawned = Instantiate(validRooms[Random.Range(0, validRooms.Count)]);
                RoomRewrite tobeSpawnedRoom = tobeSpawned.GetComponent<RoomRewrite>();
                tobeSpawned.transform.position = ConvertToWorldPos(position);
                GameObject tobeRotated = tobeSpawned.transform.Find("Plane").gameObject;
                tobeRotated.transform.rotation = Quaternion.Euler(0, 90 * (int)tobeSpawnedRoom.rotation, 0);
                PrintInfo(room);
                Generate(tobeSpawnedRoom);
            }
            else
            {
                SpawnWall(room, connector);
                continue;
            }

        }
    }

    private bool CheckPositionValidRoom(Vector2Int position, int SizeX, int SizeY)
    {
        for (int y = 0; SizeY > y; y++)
        {
            for (int x = 0; SizeX > x; x++)
            {
                int index = ((position.y + y) * gridsize) + (position.x + x);
                if (map[index]) // returns false if the map position is taken
                    return false;
            }
        }
        return true;
    }

    private bool CheckPositionValid(Vector2Int position)
    {
        int index = ((position.y) * gridsize) + (position.x);
        return !map[index];
    }

    private void WriteToMap(Vector2Int position, int SizeX, int SizeY)
    {
        for (int y = 0; SizeY > y; y++)
        {
            for (int x = 0; SizeX > x; x++)
            {
                int index = ((position.y + y) * gridsize) + (position.x + x);
                map[index] = true;
            }
        }
    }

    private Vector3 ConvertToWorldPos(int sumvalue)
    {
        int y = sumvalue / gridsize;
        int x = sumvalue % gridsize;
        return new Vector3(x * 10f, 0, y * 10f);
    }

    private Vector3 ConvertToWorldPos(Vector2 position)
    {
        return new Vector3(position.x * 10f, 0, position.y * 10f);
    }

    private Vector3 ConvertToWorldPos(int x, int y)
    {
        return new Vector3(x * 10f, 0, y * 10f);
    }

    private void PrintInfo(RoomRewrite room)
    {
        Debug.Log(room.name + " has been generated");
        for (int count = 0; room.Connectors.Count > count; count++)
        {
            if (!room.Connectors[count].processed)
                Debug.Log("A Room Should be Generated at " + (room.position + room.GetConnectorMapExitPosition(count)));
        }
    }

    private void SpawnWall(RoomRewrite room, int index)
    {
        GameObject newwall = Instantiate(wall);

        switch (room.Connectors[index].Exit[(int)(room.rotation)])
        {
            case RoomRewrite.Rotation.Up:
                newwall.transform.position = ConvertToWorldPos(room.position) + new Vector3(5, 1.5f, 9.5f);
                break;
            case RoomRewrite.Rotation.Down:
                newwall.transform.position = ConvertToWorldPos(room.position) + new Vector3(5, 1.5f, 0.5f);
                break;
            case RoomRewrite.Rotation.Left:
                newwall.transform.position = ConvertToWorldPos(room.position) + new Vector3(0.5f, 1.5f, 5);
                newwall.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case RoomRewrite.Rotation.Right:
                newwall.transform.position = ConvertToWorldPos(room.position) + new Vector3(9.5f, 1.5f, 5);
                newwall.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
        }
    }
}