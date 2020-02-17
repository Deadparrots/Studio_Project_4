using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Peer;
using UnityEngine.UI;
using System;

public struct ShipObject
{
    public uint id;
    public float m_x;
    public float m_y;
    public float velocityX;
    public float velocityY;
    public float rotationVelocity;
    public int shipNum;
    public string name;
    public float rotation;

    public ShipObject(uint _id)
    {
        this.shipNum = 1;
        m_x = velocityX = velocityY = 0.0f;
        m_y = 0.0f;
        id = _id;
        name = "";
        rotation = rotationVelocity = 0.0f;
    }
}

public struct ShotObject
{
    public uint id;
    public Vector2 position;
    public Vector2 velocity;
    public float rotation;

    public ShotObject(uint _id, Vector2 _position, float _rotation)
    {
        position = _position;
        id = _id;
        rotation = _rotation;
        float angle = (rotation + 90) * Mathf.Deg2Rad;
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 3;
    }
}

public class Server_Demo : MonoBehaviour
{
    public Peer peer { get; private set; }
    private NetworkReader m_NetworkReader;
    private NetworkWriter m_NetworkWriter;
    public static Server_Demo Instance;
    public Text Info;

    private Dictionary<ulong, ShipObject> clients = new Dictionary<ulong, ShipObject>();
    private List<ShipObject> AIShips = new List<ShipObject>();
    private List<ShotObject> Shotsfired = new List<ShotObject>();
    private float AISpeed = 0.5f;
    private uint shipID;

    private uint shotcounter;

    private void Awake()
    {
        Instance = this;
    }

    public void Init(int _port)
    {
        StartServer("127.0.0.1", _port,2);

        for(uint i = 0; i <5; ++i)
        {
            ShipObject tempShip = new ShipObject(i);
            tempShip.m_x = i;
            tempShip.m_y = 2;

            AIShips.Add(tempShip);

        }
        shotcounter = 0;
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

        for(int i =0; i < AIShips.Count; ++i)
        {

            foreach (ShipObject shipObj in clients.Values)
            {
                ShipObject aiShip = AIShips[i];
                Vector2 AIposition = new Vector2(aiShip.m_x, aiShip.m_y);
                Vector2 shipPosition = new Vector2(shipObj.m_x, shipObj.m_y);
                if (Vector2.Distance(AIposition, shipPosition) <1f)
                {
                    float step = 1f * Time.deltaTime;
                    Vector2 newPosition = Vector2.MoveTowards(AIposition, shipPosition, step);
                    aiShip.m_x = newPosition.x;
                    aiShip.m_y = newPosition.y;

                    AIShips[i] = aiShip;
                    SendAIPosition();
                }
            }
               
        }

        for(int i = 0; i < Shotsfired.Count; ++i)
        {
            ShotObject shot = Shotsfired[i];
            //shot.position += shot.velocity * (1f * Time.deltaTime);
            float step = shot.velocity.magnitude * Time.deltaTime;
            Vector2 temp = new Vector2(shot.position.x + shot.velocity.x, shot.position.y + shot.velocity.y);
            shot.position = Vector2.MoveTowards(shot.position, temp, step);
            Shotsfired[i] = shot;

        }
        SendShotPosition();

    }

    private void SendShotPosition()
    {
        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_SHOTMOVEMENT);
            m_NetworkWriter.Write(Shotsfired.Count);

            foreach (ShotObject tempObj in Shotsfired)
            {
                m_NetworkWriter.Write(tempObj.id);
                m_NetworkWriter.Write(tempObj.position.x);
                m_NetworkWriter.Write(tempObj.position.y);
            }
            SendToAll(0, m_NetworkWriter, false);
        }
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
            switch(packet_id)
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
                case (byte)Packets_ID.ID_SHOTSREQUEST:
                    OnReceivedShotRequestData(peer.incomingGUID);
                    break;
                case (byte)Packets_ID.ID_HIT:
                    OnRecievedShipDeleteRequest(peer.incomingGUID);
                    break;
                case (byte)Packets_ID.ID_REMOVESHOT:
                    OnRecievedShotDeleteRequest(peer.incomingGUID);
                    break;
                
            }
           
        }
    }

    private void OnRecievedShipDeleteRequest(ulong guid)
    {
        uint tobedeleted = m_NetworkReader.ReadUInt32();

        for (int i = 0; AIShips.Count > i;i++)
        {
            if (AIShips[i].id == tobedeleted)
            {
                AIShips.RemoveAt(i);
                break;
            }
        }

        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_HIT);
            m_NetworkWriter.Write(tobedeleted);
            SendToAll(guid, m_NetworkWriter, false);
        }
    }
    private void OnRecievedShotDeleteRequest(ulong guid)
    {
        uint tobedeleted = m_NetworkReader.ReadUInt32();

        for (int i = 0; Shotsfired.Count > i;i++)
        {
            if (Shotsfired[i].id == tobedeleted)
            {
                Shotsfired.RemoveAt(i);
                break;
            }
        }

        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_REMOVESHOT);
            m_NetworkWriter.Write(tobedeleted);
            SendToAll(guid, m_NetworkWriter, false);
        }
    }


    private void OnReceivedShotRequestData(ulong guid)
    {
        ShipObject tempObj = clients[guid];

        clients[guid] = tempObj;
        shotcounter += 1;
        ShotObject shot = new ShotObject((shotcounter), new Vector2(tempObj.m_x,tempObj.m_y), tempObj.rotation);
        Shotsfired.Add(shot);

        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_NEWSHOT);
            m_NetworkWriter.Write(shot.id);
            m_NetworkWriter.Write(tempObj.m_x);
            m_NetworkWriter.Write(tempObj.m_y);

            m_NetworkWriter.Write(tempObj.rotation);

            SendToAll(guid, m_NetworkWriter, false);
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
    
    private void SendAIPosition()
    {
        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_AISHIPSMOVEMENT);
            m_NetworkWriter.Write(AIShips.Count);

            foreach (ShipObject tempObj in AIShips)
            {
                m_NetworkWriter.Write(tempObj.id);
                m_NetworkWriter.Write(tempObj.m_x);
                m_NetworkWriter.Write(tempObj.m_y);
            }
            SendToAll(0, m_NetworkWriter, false);
        }
    }
    private void OnReceivedClientMovementData(ulong guid)
    {
        ShipObject tempObj = clients[guid];
        tempObj.m_x = m_NetworkReader.ReadFloat();
        tempObj.m_y = m_NetworkReader.ReadFloat();
        tempObj.rotation = m_NetworkReader.ReadFloat();
        tempObj.velocityX = m_NetworkReader.ReadFloat();
        tempObj.velocityY = m_NetworkReader.ReadFloat();
        tempObj.rotationVelocity = m_NetworkReader.ReadFloat();

        clients[guid] = tempObj;

        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_MOVEMENT);
            m_NetworkWriter.Write(tempObj.id);
            m_NetworkWriter.Write(tempObj.m_x);
            m_NetworkWriter.Write(tempObj.m_y);
            m_NetworkWriter.Write(tempObj.rotation);
            m_NetworkWriter.Write(tempObj.velocityX);
            m_NetworkWriter.Write(tempObj.velocityY);
            m_NetworkWriter.Write(tempObj.rotationVelocity);

            SendToAll(guid, m_NetworkWriter, true);
        }
    }

    private void OnReceivedClientInitialStats(ulong guid)
    {
        ShipObject tempObj = clients[guid];
        tempObj.name = m_NetworkReader.ReadString();
        tempObj.m_x = m_NetworkReader.ReadFloat();
        tempObj.m_y = m_NetworkReader.ReadFloat();

        clients[guid] = tempObj;

        if (m_NetworkWriter.StartWritting())
        {
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_NEWSHIP);
            m_NetworkWriter.Write(tempObj.id);
            m_NetworkWriter.Write(tempObj.name);
            m_NetworkWriter.Write(tempObj.m_x);
            m_NetworkWriter.Write(tempObj.m_y);
            m_NetworkWriter.Write(tempObj.shipNum);

            SendToAll(guid, m_NetworkWriter, true);
           // peer.SendBroadcast(Peer.Priority.Immediate, Peer.Reliability.Reliable, 0);
         
        }


    }
    private void SendToAll(ulong guid, NetworkWriter _writer, bool broadcast)
    {
        foreach(ulong guids in clients.Keys)
        {
            if(broadcast)
            {
                if (guids == guid)
                    continue;
            }

            peer.SendData(guids, Peer.Reliability.Reliable, 0, _writer);
        }
    }
    
    private void OnReceivedClientNetInfo(ulong guid)
    {
        Debug.Log("server received data");
        Connection connection = FindConnection(guid);

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
                ++shipID;

                Debug.Log("Sent");

                if (m_NetworkWriter.StartWritting())
                {
                    m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_WELCOME);
                    m_NetworkWriter.Write(shipID);
                    m_NetworkWriter.Write(clients.Count);
                    
                   foreach (ShipObject shipObj in clients.Values)
                    {
                        m_NetworkWriter.Write(shipObj.id);
                        m_NetworkWriter.Write(shipObj.m_x);
                        m_NetworkWriter.Write(shipObj.m_y);
                        m_NetworkWriter.Write(shipObj.shipNum);
                        m_NetworkWriter.Write(shipObj.name);
                    }
                   peer.SendData(guid, Peer.Reliability.Reliable, 0, m_NetworkWriter);
                  //  m_NetworkWriter.Send//sending
                    //m_NetworkWriter.Reset();

                    ShipObject newObj = new ShipObject(shipID);
                    newObj.shipNum = m_NetworkReader.ReadInt32();
                    clients.Add(guid, newObj);

                    Debug.Log("Added new guy : " + newObj.id);


                  
                }

                if (m_NetworkWriter.StartWritting())
                {
                    m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_AISHIPS);
                    m_NetworkWriter.Write(AIShips.Count);

                    foreach (ShipObject shipObj in AIShips)
                    {
                        m_NetworkWriter.Write(shipObj.id);
                        m_NetworkWriter.Write(shipObj.m_x);
                        m_NetworkWriter.Write(shipObj.m_y);

                        Debug.Log(shipObj.m_x);
                    }
                    peer.SendData(guid, Peer.Reliability.Reliable, 0, m_NetworkWriter);
                
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