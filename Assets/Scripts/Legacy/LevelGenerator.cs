using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    List<bool> map = new List<bool>();
    public List<GameObject> rooms = new List<GameObject>(); // a list of rooms
    private List<GameObject> generatedrooms = new List<GameObject>();
    private bool ExitPlaced = false; // selects a random room at the end
    private bool EntrancePlaced = false;

    private int gridsize;

    private void Start()
    {
        //Temp
        gridsize = 10;
        Generate(2);
    }

    public void Generate(int seed)
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
                map.Add( false);
            }
        }

        int StartPos = Random.Range(0 , gridsize * gridsize); // room where generator starts.
        GameObject temp = Instantiate(rooms[Random.Range(0, rooms.Count)]);
        temp.transform.position = GetPositionCombinedInt(StartPos);
        generatedrooms.Add(temp);

        Room temproom = temp.GetComponent<Room>();
        temproom.position = new Vector2Int(StartPos % gridsize, StartPos / gridsize);

        for (int mapy = 0; temproom.SizeY > mapy; mapy++) // overwrites on map
        {
            for (int mapx = 0; temproom.SizeX > mapx; mapx++)
            {
                bool tempMp = map[((mapy + temproom.position.y) * gridsize) + mapx + temproom.position.x];
                tempMp = true;
                map[((mapy + temproom.position.y) * gridsize) + mapx + temproom.position.x] = tempMp;
            }

        }
        Debug.Log("Generated Room:" + temproom.name + " at " + temproom.position);
        Debug.Log("Next Rooms should start at" + (temproom.position + temproom.GetRotatedVectorByIndex(0)));
        Debug.Log("Next Rooms should start at" + (temproom.position + temproom.GetRotatedVectorByIndex(1)));
        Generate(temproom);


    }

    private void Generate(Room room)
    {

        for (int i = 0; room.Connectors.Count > i;i++)
        {
            if (room.ConnectorsProcessed[i])
                continue;

            int connectorPositionX = (room.GetRotatedVectorByIndex(i) + room.position).x;
            int connectorPositionY = (room.GetRotatedVectorByIndex(i) + room.position).y;
            if (connectorPositionX > gridsize || connectorPositionY > gridsize || 0 > connectorPositionX || 0 > connectorPositionY) // if position will be out of bounds
            {
                continue; //replace with adding a wall
            }

            GameObject randomRoom = null;
            Room randomroom = null;
            bool roomisvalid = false;
            int loopbreaker = 0;
            while (!roomisvalid)
            {
                roomisvalid = true;
                randomRoom = Instantiate(rooms[Random.Range(0, rooms.Count)]);
                randomroom = randomRoom.GetComponent<Room>();

                randomroom.position = new Vector2Int(connectorPositionX, connectorPositionY);

                if (randomroom.SizeY + randomroom.position.y > gridsize || randomroom.SizeX + randomroom.position.x > gridsize)
                    continue;

                for (int mapy = 0; randomroom.SizeY > mapy; mapy++) // overwrites on map
                {
                    for (int mapx = 0; randomroom.SizeX > mapx; mapx++)
                    {

                        bool tempMp = map[((mapy + randomroom.position.y) * gridsize) + mapx + randomroom.position.x];
                        if (tempMp)
                        {
                            roomisvalid = false;
                            break;
                        }
                    }
                    if (!roomisvalid)
                        break;

                }

                if (loopbreaker > 8)
                {
                    Destroy(randomRoom);
                    break;
                }
                loopbreaker++;


                if (roomisvalid)
                {
                    for (int mapy = 0; randomroom.SizeY > mapy; mapy++) // overwrites on map
                    {
                        for (int mapx = 0; randomroom.SizeX > mapx; mapx++)
                        {
                            bool tempMp = map[((mapy + randomroom.position.y) * gridsize) + mapx + randomroom.position.x];
                            tempMp = true;
                            map[((mapy + randomroom.position.y) * gridsize) + mapx + randomroom.position.x] = tempMp;
                        }
                       
                    }
                    break;
                }
                else
                {
                    Destroy(randomRoom);
                }

            }

            bool freezeRotation = false; // using this to freeze further rotations;
            for (int numrotations = 0; 4 > numrotations; numrotations++) // Here the rotation and connectors from the new one should be settled
            {
                randomroom.rotation = (Room.Rotation)numrotations;

                for (int ConnectorCounter = 0; randomroom.Connectors.Count > ConnectorCounter;ConnectorCounter++)
                {

                    if (randomroom.GetRotatedVectorByIndex(ConnectorCounter) + room.GetRotatedVectorByIndex(i) == Vector2Int.zero || 
                        (room.SameSize(randomroom) && (randomroom.SizeX == 2 && randomroom.SizeY == 2) 
                        && (randomroom.GetRotatedVectorByIndex(ConnectorCounter) + room.GetRotatedVectorByIndex(i) == new Vector2Int(1,1))
                        && Room.Facing(room,randomroom, i, ConnectorCounter)))
                    {
                        //randomRoom = Instantiate(randomRoom);
                        randomRoom.transform.position = GetPosition(randomroom.position.x, randomroom.position.y);
                        
                        randomroom.position = new Vector2Int(connectorPositionX, connectorPositionY);
                        randomroom.ConnectorsProcessed[ConnectorCounter] = true;
                        room.ConnectorsProcessed[i] = true;
                    
                        GameObject rotate = randomRoom.transform.Find("Plane").gameObject;
                        rotate.transform.rotation = Quaternion.Euler(0, 90 * (int)randomroom.rotation, 0);

                        generatedrooms.Add(randomRoom);
                        Debug.Log("Generated Room:" + randomRoom.name + " at " + randomroom.position);
                        Debug.Log("Room has a rotation of" + randomroom.rotation.ToString());
                        Generate(randomroom);
                        freezeRotation = true;
                        
                    }
                }
                if (freezeRotation)
                    break;
            }


        }

    }

    

    private Vector3 GetPosition(int _x, int _y) // Converts the 2D position to 3D
    {
        return new Vector3(_x * 10, 0, _y * 10);
    }

    private Vector3 GetPositionCombinedInt(int combined) // Converts a combined position to 2d
    {
        int y = combined / gridsize;
        int x = combined % gridsize;
        return GetPosition(x, y);
    }

    private int ToMapPos(int _x, int _y)
    {
        return (_x + (_y * gridsize));
    }
}
