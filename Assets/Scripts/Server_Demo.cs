using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Peer;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerObject
{
    public uint id;
    public float m_x;
    public float m_y;
    public float m_z;
    public float velocity_X;
    public float velocity_Y;
    public float velocity_Z;
    public int playerNum;
    public string name;
    public float rotation_x;
    public float rotation_y;
    public float rotation_z;
    public bool inConnectionScene;
    public bool inGameplayScene;
    public playerObject(uint _id)
    {
        this.playerNum = 1;
        m_x = 0.0f;
        m_y = 0.0f;
        m_z = 0.0f;
        id = _id;
        name = "";
        velocity_X = velocity_Y = velocity_Z = 0.0f;
        rotation_x = rotation_y = rotation_z = 0.0f;
    }
}

public class bulletObject
{
    public uint id;
    public Vector3 position;
    public uint owner_id;
    public bulletObject(uint _id,uint _owner)
    {
        position = new Vector3(0, 0, 0);
        id = _id;
        owner_id = _owner;
    }
}
public class pickupObject
{
    public uint id;
    public Vector3 position;
    public int type;

    public pickupObject(uint _id)
    {
        position = new Vector3(0, 0, 0);
        id = _id;
        type = 0;
    }
}

public class WaypointPath
{
    public List<Transform> wayPoints = new List<Transform>();
}

public struct SpawnPoint
{
    public bool active;
    public Vector3 position;
    public SpawnPoint(bool _active)
    {
        active = _active;
        position = new Vector3(0,0,0);
    }
}


public class Server_Demo : MonoBehaviour
{
    public Peer peer { get; private set; }
    private NetworkReader m_NetworkReader;
    private NetworkWriter m_NetworkWriter;
    public static Server_Demo Instance;
    public Text Info;
    private Dictionary<ulong, playerObject> clients = new Dictionary<ulong, playerObject>();
    private List<EnemyAI> enemyList = new List<EnemyAI>();
    private List<bulletObject> bulletList = new List<bulletObject>();
    private uint playerID;
    private uint enemyID;
    private uint pickupID;
    private uint bulletID;
    public List<WaypointPath> waypointPathsList = new List<WaypointPath>();
    public List<SpawnPoint> spawnPointList = new List<SpawnPoint>();
    const int MAX_PLAYERS = 4;
    int totalWaypoints = 0;
    private void Awake()
    {
        Instance = this;
    }

    public void Init(int _port)
    {
        StartServer("127.0.0.1", _port, MAX_PLAYERS);

        //for(int i = 0; i < 5; ++i)
        //{
        //    EnemyAI newAI = new EnemyAI();
        //}
        GameObject[] waypointPathGOs;
        waypointPathGOs = GameObject.FindGameObjectsWithTag("WaypointPath");

        foreach(GameObject waypointPath in waypointPathGOs)
        {

            WaypointPath temp = new WaypointPath();

            foreach(Transform waypoint in waypointPath.transform)
            {
                temp.wayPoints.Add(waypoint);
            }

            waypointPathsList.Add(temp);
        }

        foreach (EnemyAI enemy in GameObject.FindObjectsOfType(typeof(EnemyAI)))
        {
            enemy.pid = enemyID;
            enemy.controller = true;
            enemy.WayPoints = waypointPathsList[0].wayPoints;
            enemy.WaypointIndex = 0;
            enemyList.Add(enemy);
            ++enemyID;
        }

        GameObject[] spawnPoints;
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");

        foreach(GameObject spawn in spawnPoints)
        {
            SpawnPoint spawnpoint = new SpawnPoint();
            spawnpoint.active = true;
            spawnpoint.position = spawn.transform.position;
            Debug.Log("SPawnpoint position: " + spawnpoint.position);
            spawnPointList.Add(spawnpoint);
        }
    }

    public void StopServer()
    {
        if (peer != null)
        {
            peer.Close();
            Debug.LogError("[Server] Shutting down...");
        }
    }

    private void OnDestroy()
    {
        StopServer();
    }

    public int MaxConnections { get; private set; } = -1;

    public bool StartServer(string ip, int port, int maxConnections)
    {
        if (peer == null)
        {
            peer = new Peer();
            peer = CreateServer(ip, port, maxConnections);

            if (peer != null)
            {
                MaxConnections = maxConnections;
                Debug.Log("[Server] Server initialized on port " + port);

                Debug.Log("-------------------------------------------------");
                Debug.Log("|     Max connections: " + maxConnections);
                Debug.Log("|     Max FPS: " + (Application.targetFrameRate != -1 ? Application.targetFrameRate : 1000) + "(" + Time.deltaTime.ToString("f3") + " ms)");
                Debug.Log("|     Tickrate: " + (1 / Time.fixedDeltaTime) + "(" + Time.fixedDeltaTime.ToString("f3") + " ms)");
                Debug.Log("-------------------------------------------------");

                m_NetworkReader = new NetworkReader(peer);
                m_NetworkWriter = new NetworkWriter(peer);

                return true;
            }
            else
            {
                Debug.LogError("[Server] Starting failed...");

                return false;
            }
        }
        else
        {
            return true;
        }
    }

    private void FixedUpdate()
    {
        if (peer != null)
        {
            while (peer.Receive())
            {
                m_NetworkReader.StartReading();
                byte b = m_NetworkReader.ReadByte();

                OnReceivedPacket(b);
            }
        }

        //if(spawnPickUpTimer < 0.0f)
        //{
        //    spawnPickUpTimer = spawnPickUpCoolDown;
        //    SpawnPickUp();
        //}
        //else
        //{
        //    spawnPickUpTimer -= Time.deltaTime;
        //}

        if (Input.GetKeyDown("space"))
        {
            //ChangeScene("Scene2");
            //DestroyEnemy(2);
            ChangeScene("SP4");
        }

        if (Input.GetKeyDown("f"))
        {
            //ChangeScene("Scene2");
            //DestroyEnemy(2);
            enemyList[0].sm.SetNextState("Dead");
        }

        foreach (EnemyAI enemy in enemyList)
        {
            enemy.PlayerList.Clear();
            foreach (playerObject player in clients.Values)
            {
                enemy.PlayerList.Add(player);
            }
        }
        //Debug.Log("Number of AI in SERVER = " + enemyList.Count);
        //Debug.Log("Num of WaypointPaths: " + waypointPathsList.Count);
        //Debug.Log("Total Waypoints: " + totalWaypoints);
    }


    private void OnReceivedPacket(byte packet_id)
    {
        bool IsInternalNetworkPackets = packet_id <= 134;

        if (IsInternalNetworkPackets)
        {
            if (packet_id == (byte)RakNet_Packets_ID.NEW_INCOMING_CONNECTION)
            {
                OnConnected();//добавляем соединение
            }

            if (packet_id == (byte)RakNet_Packets_ID.CONNECTION_LOST || packet_id == (byte)RakNet_Packets_ID.DISCONNECTION_NOTIFICATION)
            {
                Connection conn = FindConnection(peer.incomingGUID);

                if (conn != null)
                {
                    OnDisconnected(FindConnection(peer.incomingGUID));
                }
            }
        }
        else
        {
            switch (packet_id)
            {
                case (byte)Packets_ID.CL_INFO:
                    OnReceivedClientNetInfo(peer.incomingGUID);
                    break;
                case (byte)Packets_ID.ID_INITIALSTATS:
                    OnReceivedClientInitialStats(peer.incomingGUID);
                    break;
                case (byte)Packets_ID.ID_MOVEMENT:
                    OnReceivedClientMovementData(peer.incomingGUID);
                    break;
                case (byte)Packets_ID.ID_SHOOTBULLET:
                    OnReceivedShotData(peer.incomingGUID);
                    break;
                case (byte)Packets_ID.ID_GETCONNECTSCENEINFO:
                    SendConnectionSceneInfo(peer.incomingGUID);
                    break;
            }


        }
    }



    #region Connections
    public List<Connection> connections = new List<Connection>();
    private Dictionary<ulong, Connection> connectionByGUID = new Dictionary<ulong, Connection>();

    public List<ulong> guids = new List<ulong>();

    public Connection FindConnection(ulong guid)
    {
        if (connectionByGUID.TryGetValue(guid, out Connection value))
        {
            return value;
        }
        return null;
    }

    private void AddConnection(Connection connection)
    {
        connections.Add(connection);
        connectionByGUID.Add(connection.guid, connection);
        guids.Add(connection.guid);
    }

    private void RemoveConnection(Connection connection)
    {
        clients.Remove(connection.guid);
        connectionByGUID.Remove(connection.guid);
        connections.Remove(connection);
        guids.Remove(connection.guid);
    }

    public static Connection[] Connections
    {
        get
        {
            return Instance.connections.ToArray();
        }
    }

    public static Connection GetByID(int id)
    {
        if (Connections.Length > 0)
        {
            return Connections[id];
        }

        return null;
    }

    public static Connection GetByIP(string ip)
    {
        foreach (Connection c in Connections)
        {
            if (c.ipaddress == ip)
            {
                return c;
            }
        }

        return null;
    }

    public static Connection GetByName(string name)
    {
        foreach (Connection c in Connections)
        {
            if (c.Info.name == name)
            {
                return c;
            }
        }

        return null;
    }

    public static Connection GetByHWID(string hwid)
    {
        foreach (Connection c in Connections)
        {
            if (c.Info.client_hwid == hwid)
            {
                return c;
            }
        }

        return null;
    }

    #endregion

    #region Events
    private void OnConnected()
    {
        Connection connection = new Connection(peer, peer.incomingGUID, connections.Count);

        //добавляем в список соединений
        AddConnection(connection);

        Debug.Log("[Server] Connection established " + connection.ipaddress);
        //peer.SendData(guid, Peer.Reliability.Reliable, 0, m_NetworkWriter);

        peer.SendPacket(connection, Packets_ID.CL_INFO, m_NetworkWriter);
    }

    private void OnDisconnected(Connection connection)
    {
        if (connection != null)
        {
            try
            {
                Debug.LogError("[Server] " + connection.Info.name + " disconnected [IP: " + connection.ipaddress + "]");

                RemoveConnection(connection);
            }
            catch
            {
                Debug.LogError("[Server] Unassgigned connection destroyed!");
            }
        }
    }

    private void SendConnectionSceneInfo(ulong guid)
    {
        if (m_NetworkWriter.StartWritting())
        {
            playerObject client = clients[guid];
            client.inConnectionScene = true;
            clients[guid] = client;
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_SENDCONNECTSCENEINFO);
            int playersInConnectionScene = 0;


            foreach (playerObject playerObj in clients.Values)
            {
                if (playerObj.inConnectionScene == true && playerObj.id != client.id)
                {
                    ++playersInConnectionScene;
                }
            }

            m_NetworkWriter.Write(playersInConnectionScene);

            foreach (playerObject playerObj in clients.Values)
            {
                if (playerObj.inConnectionScene == true && playerObj.id != client.id)
                {
                    m_NetworkWriter.Write(playerObj.id);
                    m_NetworkWriter.Write(playerObj.name);
                    string status = "";
                    if (playerObj.inGameplayScene)
                        status = "In-Game";
                    else
                        status = "Not In-Game";
                    m_NetworkWriter.Write(status);
                }
            }
            peer.SendData(guid, Peer.Reliability.Reliable, 0, m_NetworkWriter);
        }
    }
    private void OnReceivedShotData(ulong guid)
    {
        playerObject player = clients[guid];
        bulletObject tempObj = new bulletObject(bulletID,player.id);
        tempObj.position.x = m_NetworkReader.ReadFloat();
        tempObj.position.y = m_NetworkReader.ReadFloat();
        tempObj.position.z = m_NetworkReader.ReadFloat();
        Vector3 forward = m_NetworkReader.ReadVector3();
        ++bulletID;
        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_SHOOTBULLET);
            m_NetworkWriter.Write(tempObj.id);
            m_NetworkWriter.Write(tempObj.owner_id);
            m_NetworkWriter.Write(tempObj.position.x);
            m_NetworkWriter.Write(tempObj.position.y);
            m_NetworkWriter.Write(tempObj.position.z);
            m_NetworkWriter.Write(forward);

            foreach (ulong guids in clients.Keys)
            {
                peer.SendData(guids, Peer.Reliability.Reliable, 0, m_NetworkWriter);
            }
            bulletList.Add(tempObj);
        }
    }

    public void DestroyBullet(uint id)
    {
        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_DESTROYBULLET);
            m_NetworkWriter.Write(id);

            foreach (ulong guids in clients.Keys)
            {
                peer.SendData(guids, Peer.Reliability.Reliable, 0, m_NetworkWriter);
            }
        }
    }

    private void OnReceivedClientMovementData(ulong guid)
    {
        playerObject tempObj = clients[guid];
        tempObj.m_x = m_NetworkReader.ReadFloat();
        tempObj.m_y = m_NetworkReader.ReadFloat();
        tempObj.m_z = m_NetworkReader.ReadFloat();
        tempObj.rotation_x = m_NetworkReader.ReadFloat();
        tempObj.rotation_y = m_NetworkReader.ReadFloat();
        tempObj.rotation_z = m_NetworkReader.ReadFloat();
        tempObj.velocity_X = m_NetworkReader.ReadFloat();
        tempObj.velocity_Y = m_NetworkReader.ReadFloat();
        tempObj.velocity_Z = m_NetworkReader.ReadFloat();

        clients[guid] = tempObj;

        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_MOVEMENT);
            m_NetworkWriter.Write(tempObj.id);
            m_NetworkWriter.Write(tempObj.m_x);
            m_NetworkWriter.Write(tempObj.m_y);
            m_NetworkWriter.Write(tempObj.m_z);
            m_NetworkWriter.Write(tempObj.rotation_x);
            m_NetworkWriter.Write(tempObj.rotation_y);
            m_NetworkWriter.Write(tempObj.rotation_z);
            m_NetworkWriter.Write(tempObj.velocity_X);
            m_NetworkWriter.Write(tempObj.velocity_Y);
            m_NetworkWriter.Write(tempObj.velocity_Z);

            SendToAll(guid, m_NetworkWriter, true);
        }
    }

    private void OnReceivedClientInitialStats(ulong guid)
    {
        playerObject tempObj = clients[guid];
        tempObj.name = m_NetworkReader.ReadString();
        tempObj.m_x = m_NetworkReader.ReadFloat();
        tempObj.m_y = m_NetworkReader.ReadFloat();
        tempObj.m_z = m_NetworkReader.ReadFloat();
        tempObj.rotation_x = m_NetworkReader.ReadFloat();
        tempObj.rotation_y = m_NetworkReader.ReadFloat();
        tempObj.rotation_z = m_NetworkReader.ReadFloat();

        clients[guid] = tempObj;

        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_NEWPLAYER);
            m_NetworkWriter.Write(tempObj.id);
            m_NetworkWriter.Write(tempObj.name);
            m_NetworkWriter.Write(tempObj.m_x);
            m_NetworkWriter.Write(tempObj.m_y);
            m_NetworkWriter.Write(tempObj.m_z);
            m_NetworkWriter.Write(tempObj.rotation_x);
            m_NetworkWriter.Write(tempObj.rotation_y);
            m_NetworkWriter.Write(tempObj.rotation_z);
            m_NetworkWriter.Write(tempObj.playerNum);

            SendToAll(guid, m_NetworkWriter, true);
            // peer.SendBroadcast(Peer.Priority.Immediate, Peer.Reliability.Reliable, 0);
        }
    }
    private void SendToAll(ulong guid, NetworkWriter _writer, bool broadcast)
    {
        foreach (ulong guids in clients.Keys)
        {
            if (broadcast)
            {
                if (guids == guid)
                    continue;
            }

            peer.SendData(guids, Peer.Reliability.Reliable, 0, _writer);
        }
    }

    public void DestroyEnemy(uint enemyID)
    {
        if (m_NetworkWriter.StartWritting())
        {
            foreach (EnemyAI enemy in enemyList)
            {
                if (enemy.pid == enemyID)
                {

                    m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_DESTROYENEMY);
                    m_NetworkWriter.Write(enemyID);

                    enemyList.Remove(enemy);
                    Destroy(enemy.gameObject);
                    Debug.Log("Destroying Enemy " + enemyID);
                    foreach (ulong guids in clients.Keys)
                    {
                        peer.SendData(guids, Peer.Reliability.Reliable, 0, m_NetworkWriter);
                    }
                }
            }
        }
    }

    public void spawnPickup(Vector3 position)
    {
        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_SPAWNPICKUP);
            pickupObject pickup = new pickupObject(pickupID);
            ++pickupID;
            pickup.position = position;
            
            // TODO: Spawn random pickups
            m_NetworkWriter.Write(pickup.id);
            m_NetworkWriter.Write(pickup.type);
            m_NetworkWriter.Write(position);


            foreach (ulong guids in clients.Keys)
            {
                peer.SendData(guids, Peer.Reliability.Reliable, 0, m_NetworkWriter);
            }
        }
    }

    public void DmgPlayer(uint playerID,float dmg)
    {
        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_DMGPLAYER);
            m_NetworkWriter.Write(playerID);
            m_NetworkWriter.Write(dmg);


            foreach (ulong guids in clients.Keys)
           {
               peer.SendData(guids, Peer.Reliability.Reliable, 0, m_NetworkWriter);
           }
        }
    }

    public void DmgEnemy(uint enemyID, float dmg)
    {
        foreach (EnemyAI enemy in enemyList)
        {
            if (enemy.pid == enemyID)
            {
                enemy.hp -= dmg;
            }
        }

        // TODO: ADD SCORE OF BulletOwner

        //if (m_NetworkWriter.StartWritting())
        //{
        //    m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_DMGENEMY);
        //    m_NetworkWriter.Write(enemyID);
        //    m_NetworkWriter.Write(dmg);
        //    foreach (ulong guids in clients.Keys)
        //    {
        //        peer.SendData(guids, Peer.Reliability.Reliable, 0, m_NetworkWriter);
        //    }
        //}
    }

    public void DestroyBreakable()
    {

    }

    public void UpdateEnemyInClient(uint enemyID, Vector3 position,Vector3 rotation,string currentState,float hp)
    {
        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_UPDATENEMY);
            m_NetworkWriter.Write(enemyID);
            m_NetworkWriter.Write(position);
            m_NetworkWriter.Write(rotation);
            m_NetworkWriter.Write(currentState);
            m_NetworkWriter.Write(hp);


            foreach (ulong guids in clients.Keys)
            {
                peer.SendData(guids, Peer.Reliability.Reliable, 0, m_NetworkWriter);
            }
        }
    }

    void ChangeScene(string sceneName)
    {
        if (m_NetworkWriter.StartWritting())
        {

            Debug.Log("Changing to " + sceneName);
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_CHANGESCENE);
            m_NetworkWriter.Write(sceneName);

            foreach (ulong guids in clients.Keys)
            {
                peer.SendData(guids, Peer.Reliability.Reliable, 0, m_NetworkWriter);
            }
        }
    }

    private void OnReceivedClientNetInfo(ulong guid)
    {
        Debug.Log("server received data");
        Connection connection = FindConnection(guid);

        if (clients.Count == MAX_PLAYERS)
            return;

        if (connection != null)
        {
            if (connection.Info == null)
            {
                connection.Info = new ClientNetInfo();
                connection.Info.net_id = guid;
                connection.Info.name = m_NetworkReader.ReadString();
                connection.Info.local_id = m_NetworkReader.ReadPackedUInt64();
                connection.Info.client_hwid = m_NetworkReader.ReadString();
                connection.Info.client_version = m_NetworkReader.ReadString();

                Debug.Log("Sent");

                if (m_NetworkWriter.StartWritting())
                {
                    m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_WELCOME);
                    m_NetworkWriter.Write(playerID);

                    Vector3 zeroVector = new Vector3(0, 0, 0);

                    // TODO: move to function that inits gameplay stuff
                    //if (playerID < MAX_PLAYERS && spawnPointList.Count >= playerID)
                    //{
                    //    m_NetworkWriter.Write(spawnPointList[(int)playerID].position);
                    //}
                    //else
                    //m_NetworkWriter.Write(zeroVector);  // in case no spawn point was set

                    //m_NetworkWriter.Write(clients.Count);

                    //foreach (playerObject playerObj in clients.Values)
                    //{
                    //    m_NetworkWriter.Write(playerObj.id);
                    //    m_NetworkWriter.Write(playerObj.m_x);
                    //    m_NetworkWriter.Write(playerObj.m_y);
                    //    m_NetworkWriter.Write(playerObj.m_z);
                    //    m_NetworkWriter.Write(playerObj.rotation_x);
                    //    m_NetworkWriter.Write(playerObj.rotation_y);
                    //    m_NetworkWriter.Write(playerObj.rotation_z);
                    //    //m_NetworkWriter.Write(playerObj.playerNum);
                    //    m_NetworkWriter.Write(playerObj.name);
                    //}

                    //m_NetworkWriter.Write(enemyList.Count);
                    //foreach (EnemyAI enemy in enemyList)
                    //{
                    //    m_NetworkWriter.Write(enemy.pid);
                    //    m_NetworkWriter.Write(enemy.ePosition);
                    //    m_NetworkWriter.Write(enemy.eRotation);
                    //}
                    peer.SendData(guid, Peer.Reliability.Reliable, 0, m_NetworkWriter);
                    //  m_NetworkWriter.Send//sending
                    //m_NetworkWriter.Reset();
                    //Vector3 position = spawnPointList[(int)playerID].position;
                    playerObject newObj = new playerObject(playerID);
                    newObj.playerNum = m_NetworkReader.ReadInt32();
                    newObj.m_x = zeroVector.x;
                    newObj.m_y = zeroVector.y;
                    newObj.m_z = zeroVector.z;
                    clients.Add(guid, newObj);
                    ++playerID;
                    Debug.Log("Added new guy : " + newObj.id);
                }
                //peer.SendPacket(connection, Packets_ID.NET_LOGIN, Reliability.Reliable, m_NetworkWriter);
            }
            else
            {
                peer.SendPacket(connection, Packets_ID.CL_FAKE, Reliability.Reliable, m_NetworkWriter);
                peer.Kick(connection, 1);
            }
        }
    }
    #endregion

    /*
    public InputField Guid;

    public void OnBanClicked()
    {
        Connection connection = FindConnection(ulong.Parse(Guid.text));

        if (connection != null)
        {
            peer.SendPacket(connection, Packets_ID.CL_BANNED, Reliability.Reliable, m_NetworkWriter);
            peer.Kick(connection, 1);
        }
    }
    */
}