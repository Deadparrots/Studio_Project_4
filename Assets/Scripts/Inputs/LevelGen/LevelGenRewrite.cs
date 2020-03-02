using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenRewrite : MonoBehaviour
{

    private struct RoomRequirements
    {
        public int ConnectorIndex;
        public RoomRewrite.Rotation Rotation;
        public GameObject Room;
        public Vector2Int Position;

        public RoomRequirements(int _ConnectorIndex, RoomRewrite.Rotation _Rotation, GameObject _Room, Vector2Int _Position)
        {
            ConnectorIndex = _ConnectorIndex;
            Rotation = _Rotation;
            Room = _Room;
            Position = _Position;
        }
    }

    // will probably do on free time
    List<bool> map = new List<bool>();
    public List<GameObject> prefabs = new List<GameObject>();
    private List<RoomRewrite> GeneratedRooms = new List<RoomRewrite>();

    public GameObject Spawn; // The GameObject where the players start.
    public GameObject Exit; // The GameObject where the game goes to the next level.

    public GameObject wall = null; // used to fill in a doorway that has been blocked
    private int gridsize;

    private void Start()
    {
        //TODO: Move this function call to the client when it recieves a generate map packet.
        GenerateMap(10, 1);
    }

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

        //int StartPos = Random.Range(0, gridsize * gridsize); // room where generator starts.
        int MapStartPos = ((gridsize * gridsize) / 2) + (gridsize / 2); // Changed to be the middle of the map

        GameObject StartingRoom = Instantiate(prefabs[Random.Range(0, prefabs.Count)]);
        StartingRoom.transform.position = ConvertToWorldPos(MapStartPos);
        RoomRewrite StartingRoomInfo = StartingRoom.GetComponent<RoomRewrite>();
        StartingRoomInfo.position = new Vector2Int(MapStartPos % gridsize, MapStartPos / gridsize);
        WriteToMap(StartingRoomInfo.position, StartingRoomInfo.SizeX, StartingRoomInfo.SizeY);

        GeneratedRooms.Add(StartingRoomInfo);
        PrintInfo(StartingRoomInfo);
        Generate(StartingRoomInfo);

        // Spawn the spawnpoint and endpoint
        int SpawnPoint = Random.Range(0, GeneratedRooms.Count);
        GameObject temporary = Instantiate(Spawn);
        temporary.transform.position = ConvertToWorldPos(GeneratedRooms[SpawnPoint].position)
            + new Vector3(GeneratedRooms[SpawnPoint].SizeX * 5, 0, GeneratedRooms[SpawnPoint].SizeY * 5);
    

        int EndPoint = Random.Range(0, GeneratedRooms.Count);
        temporary = Instantiate(Exit);
        temporary.transform.position = ConvertToWorldPos(GeneratedRooms[EndPoint].position)
            + new Vector3(GeneratedRooms[EndPoint].SizeX * 5, 0, GeneratedRooms[EndPoint].SizeY * 5);

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
            List<RoomRequirements> validRooms = new List<RoomRequirements>();

             // do a loop or whatever to add to list
            for (int i = 0; prefabs.Count > i;i++)
            {
                RoomRewrite nextroom = prefabs[i].GetComponent<RoomRewrite>();
                if (CheckPositionValidRoom(Nextposition, nextroom.SizeX, nextroom.SizeY)) // if the room can fit within the position
                {
                    for (int rotations = 0; 4 > rotations;rotations++)
                    {
                        RoomRewrite.Rotation rotation = (RoomRewrite.Rotation)rotations;
                        for (int nextconnector = 0; nextroom.Connectors.Count > nextconnector;nextconnector++)
                        {
                            if (nextroom.Connectors[nextconnector].Exit[(int)rotation] == RequiredRotationForConnector) // if correct connector
                            {
                                    if (CheckPositionValidRoom(Nextposition - nextroom.Connectors[nextconnector].rotatedpositions[(int)rotation], nextroom.SizeX, nextroom.SizeY))
                                    {
                                        validRooms.Add(new RoomRequirements(nextconnector, rotation, prefabs[i], Nextposition - nextroom.Connectors[nextconnector].rotatedpositions[(int)rotation]));
                                        continue;
                                    }
                            }
                        }
                    }
                }
            }

            if (validRooms.Count != 0)
            {
                int index = Random.Range(0, validRooms.Count);
                GameObject tobeSpawned = Instantiate(validRooms[index].Room);
                RoomRewrite tobeSpawnedRoom = tobeSpawned.GetComponent<RoomRewrite>();

                tobeSpawnedRoom.position = validRooms[index].Position;
                tobeSpawnedRoom.rotation = validRooms[index].Rotation;
                tobeSpawnedRoom.Connectors[validRooms[index].ConnectorIndex].processed = true;

                tobeSpawned.transform.position = ConvertToWorldPos(validRooms[index].Position);
                GameObject tobeRotated = tobeSpawned.transform.Find("Plane").gameObject;
                tobeRotated.transform.rotation = Quaternion.Euler(0, 90 * (int)validRooms[index].Rotation, 0);

                WriteToMap(Nextposition, tobeSpawnedRoom.SizeX, tobeSpawnedRoom.SizeY);
                GeneratedRooms.Add(tobeSpawnedRoom);
                PrintInfo(tobeSpawnedRoom);
                Generate(tobeSpawnedRoom); // Starts Work on next room
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

                if (index > (map.Count - 1) || 0 > index)
                    return false;

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
        Debug.Log(room.name + " has been generated at " + room.position + " with a rotation of " + room.rotation.ToString());
        for (int count = 0; room.Connectors.Count > count; count++)
        {
            if (!room.Connectors[count].processed)
                Debug.Log("A Room Should be Generated at " + (room.GetConnectorMapExitPosition(count)));
        }
    }

    private void SpawnWall(RoomRewrite room, int index)
    {
        GameObject newwall = Instantiate(wall);
        Vector3 temp = new Vector3(room.Connectors[index].rotatedpositions[(int)room.rotation].x * 10, 0, room.Connectors[index].rotatedpositions[(int)room.rotation].y * 10);
        switch (room.Connectors[index].Exit[(int)(room.rotation)])
        {
            case RoomRewrite.Rotation.Up:
                newwall.transform.position = ConvertToWorldPos(room.position) + new Vector3(5, 1.5f, 9.5f) + temp;
                break;
            case RoomRewrite.Rotation.Down:
                newwall.transform.position = ConvertToWorldPos(room.position) + new Vector3(5, 1.5f, 0.5f) + temp;
                break;
            case RoomRewrite.Rotation.Left:
                newwall.transform.position = ConvertToWorldPos(room.position) + new Vector3(0.5f, 1.5f, 5) + temp;
                newwall.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case RoomRewrite.Rotation.Right:
                newwall.transform.position = ConvertToWorldPos(room.position) + new Vector3(9.5f, 1.5f, 5) + temp;
                newwall.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
        }
    }
}