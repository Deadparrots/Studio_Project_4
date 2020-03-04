using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
public class Client_Demo : MonoBehaviour
{
    public ClientNetInfo m_ClientNetInfo = new ClientNetInfo();
    public enum ClientState { DISCONNECTED, CONNECTED }
    public ClientState m_State = ClientState.DISCONNECTED;
    public Text Info;
    public Peer peer { get; private set; }

    [SerializeField] private GameObject playerReference;
    [SerializeField] private GameObject enemyReference;
    [SerializeField] private GameObject healthPickupReference;
    [SerializeField] private GameObject bulletReference;


    private List<PlayerManager> playersList = new List<PlayerManager>();
    private List<EnemyAI> enemyList = new List<EnemyAI>();
    private List<PickupManager> pickupList = new List<PickupManager>();
    private List<BulletManager> bulletList = new List<BulletManager>();

    //private List<ShipManager> shipList = new List<ShipManager>();
    //private List<MissileManager> missileList = new List<MissileManager>();
    //private List<PickUpsManager> pickUpList = new List<PickUpsManager>();
    private NetworkReader m_NetworkReader;
    private NetworkWriter m_NetworkWriter;
    public static Client_Demo Instance;
    private bool sendMsg = false;
    private ulong serveruid = 0;
    private float delta = 0.0f;
    private string userName;
    Camera m_MainCamera;
    SceneManagement sceneMgr;
    private void Awake()
    {
        Instance = this;
        sceneMgr = gameObject.GetComponent<SceneManagement>();
    }

    public SceneManagement GetSceneManager()
    {
        return sceneMgr;
    }


    public void Init(string _ip, int _port, string _name)
    {
        userName = _name;
        Connect(_ip, _port);
        sceneMgr.ToMainmenu();
    }

    public bool IsRunning
    {
        get
        {
            return peer != null;
        }
    }


    protected void Update()
    {
        delta += Time.deltaTime;
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    GetComponent<GameData>().inventorychoice = 1;
        //}
        //else if(Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    GetComponent<GameData>().inventorychoice = 2;
        //}
        if (delta > 0.5f)
        {
            SendMovement();
            delta = 0;
        }
        if (Input.GetKeyDown(InputManager.Weapon1))
        {
            Sendgun();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerManager me = playersList[0];
            me.position = new Vector3(10, 10, 0);
        }

        PlayerManager player = gameObject.GetComponent<PlayerManager>();
        if(player.previve == true && player.hp <= 0)
        {
            player.previve = false;
            player.hp = 100.0f;
        }


    }
    private void Sendgun()
    {
        if (m_NetworkWriter.StartWritting())
        {
            PlayerManager me = playersList[0];
            if (me == null)
                return;
            // TODO: use playerManager's childObject 
            me.isShooting = true;
            Vector3 bulletPos = new Vector3(0, 0, 0);
            foreach (Transform child in me.gameObject.transform)
            {
                if (child.name == "Body")
                {
                    bulletPos = child.gameObject.transform.position + (child.gameObject.transform.forward.normalized);
                    m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_SHOOTBULLET);
                    m_NetworkWriter.Write(bulletPos.x);
                    m_NetworkWriter.Write(bulletPos.y);
                    m_NetworkWriter.Write(bulletPos.z);
                    m_NetworkWriter.Write(child.gameObject.transform.forward);
                    m_NetworkWriter.Write(me.isShooting);
                    m_NetworkWriter.Send(serveruid, Peer.Priority.Immediate, Peer.Reliability.Reliable, 0);
                }
            }
        }
    }
    private void SendMovement()
    {
        if (m_NetworkWriter.StartWritting())
        {
            PlayerManager me = playersList[0];
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_MOVEMENT);

            // step 9 : Instead of sending x,y,w ..... , send the server version instead (x,y,w,velocity, angular velocity)


            m_NetworkWriter.Write(me.position.x);
            m_NetworkWriter.Write(me.position.y);
            m_NetworkWriter.Write(me.position.z);
            m_NetworkWriter.Write(me.pRotation.x);
            m_NetworkWriter.Write(me.pRotation.y);
            m_NetworkWriter.Write(me.pRotation.z);
            m_NetworkWriter.Write(me.velocity.x);
            m_NetworkWriter.Write(me.velocity.y);
            m_NetworkWriter.Write(me.velocity.z);
            m_NetworkWriter.Send(serveruid, Peer.Priority.Immediate, Peer.Reliability.Reliable, 0);
        }
    }

    public void GetConnectionSceneInfo()
    {
        if (m_NetworkWriter.StartWritting())
        {
            PlayerManager me = playersList[0];
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_GETCONNECTSCENEINFO);

            // step 9 : Instead of sending x,y,w ..... , send the server version instead (x,y,w,velocity, angular velocity)
            m_NetworkWriter.Send(serveruid, Peer.Priority.Immediate, Peer.Reliability.Reliable, 0);
        }
    }

    public void GetGameplaySceneInfo()
    {
        if (m_NetworkWriter.StartWritting())
        {
            PlayerManager me = playersList[0];
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_GETGAMEPLAYSCENEINFO);
            m_NetworkWriter.Send(serveruid, Peer.Priority.Immediate, Peer.Reliability.Reliable, 0);
        }
    }
    #region Connect/Disconnect
    public void Connect(string ip, int port, int retries, int retry_delay, int timeout)
    {
    CREATE_PEER:
        tmp_Banned = tmp_Fake = false;
        if (peer == null)
        {
            peer = Peer.CreateConnection(ip, port, retries, retry_delay, timeout);

            if (peer != null)
            {
                Debug.Log("[Client] Preparing to receiving...");
                m_NetworkReader = new NetworkReader(peer);
                m_NetworkWriter = new NetworkWriter(peer);
            }
        }
        else
        {
            peer.Close();
            peer = null;

            goto CREATE_PEER;
        }
    }

    public void Connect(string ip, int port)
    {
        Connect(ip, port, 30, 500, 30);
    }


    public void Disconnect()
    {
        if (m_State == ClientState.CONNECTED)
        {
            OnDisconnected("");
            peer.Close();
            peer = null;
        }
    }
    #endregion

    private unsafe void FixedUpdate()
    {
        m_State = peer != null ? ClientState.CONNECTED : ClientState.DISCONNECTED;

        if (peer != null)
        {
            while (peer.Receive())
            {
                m_NetworkReader.StartReading();
                byte b = m_NetworkReader.ReadByte();

                OnReceivedPacket(b);
            }
        }

        //Debug.Log("Player " + playersList[0].pid + " HP: " + playersList[0].hp);
    }


    private bool tmp_Banned = false, tmp_Fake = false;


    /// <summary>
    /// Parsing packet
    /// </summary>
    /// <param name="packet_id">PACKET ID  - SEE Packets_ID.cs</param>

    private void OnReceivedPacket(byte packet_id)
    {
        bool IsInternalNetworkPackets = packet_id <= 134;

        if (IsInternalNetworkPackets)
        {
            if (packet_id == (byte)Peer.RakNet_Packets_ID.CONNECTION_REQUEST_ACCEPTED)
            {
                OnConnected(peer.incomingAddress);
            }

            if (packet_id == (byte)Peer.RakNet_Packets_ID.CONNECTION_ATTEMPT_FAILED)
            {
                OnDisconnected("Connection attempt failed");
            }

            if (packet_id == (byte)Peer.RakNet_Packets_ID.INCOMPATIBLE_PROTOCOL_VERSION)
            {
                OnDisconnected("Incompatible protocol version");
            }

            if (packet_id == (byte)Peer.RakNet_Packets_ID.CONNECTION_LOST)
            {
                OnDisconnected("Time out");
            }

            if (packet_id == (byte)Peer.RakNet_Packets_ID.NO_FREE_INCOMING_CONNECTIONS)
            {
                OnDisconnected("Server is full.");
            }

            if (packet_id == (byte)Peer.RakNet_Packets_ID.DISCONNECTION_NOTIFICATION && !tmp_Banned && !tmp_Fake)
            {
                OnDisconnected("You are kicked!");
            }
        }
        else
        {
            switch (packet_id)
            {
                case (byte)Packets_ID.CL_INFO:
                    if (m_NetworkWriter.StartWritting())
                    {
                        GameObject playerObj = Instantiate(playerReference);
                        int playerNum = playerObj.GetComponentInChildren<PlayerMovement>().Init(true);
                        playerObj.GetComponent<PlayerManager>().SetPlayer(true); ;
                        playersList.Add(playerObj.GetComponent<PlayerManager>());

                        m_MainCamera = Camera.main;
                        LockRelativePosition lrpScript = m_MainCamera.GetComponent<LockRelativePosition>();

                        foreach (Transform child in playerObj.transform)
                        {
                            if (child.name == "Body")
                            {
                                lrpScript.Reference = child.gameObject;
                            }
                        }


                        m_NetworkWriter.WritePacketID((byte)Packets_ID.CL_INFO);
                        m_NetworkWriter.Write(m_ClientNetInfo.name);
                        m_NetworkWriter.WritePackedUInt64(m_ClientNetInfo.local_id);
                        m_NetworkWriter.Write(m_ClientNetInfo.client_hwid);
                        m_NetworkWriter.Write(m_ClientNetInfo.client_version);
                        m_NetworkWriter.Write(playerNum);
                        serveruid = peer.incomingGUID;
                        m_NetworkWriter.Send(peer.incomingGUID, Peer.Priority.Immediate, Peer.Reliability.Reliable, 0);//sending
                    }
                    break;
                case (byte)Packets_ID.NET_REGISTER:
                    {
                        bool success = m_NetworkReader.ReadBoolean();

                        if (success)
                        {
                            GameObject obj = GameObject.FindGameObjectWithTag("UIMaster");
                            obj.GetComponent<UIController>().SetToLogin();
                        }
                        else
                            Debug.Log("Username Already Exists");
                    }
                    break;
                case (byte)Packets_ID.CL_ACCEPTED:
                    m_ClientNetInfo.net_id = m_NetworkReader.ReadPackedUInt64();
                    Debug.Log("[Client] Accepted connection by server... [ID: " + m_ClientNetInfo.net_id + "]");
                    break;
                case (byte)Packets_ID.ID_WELCOME:
                    Debug.Log("welcome!!");
                    uint id = m_NetworkReader.ReadUInt32();
                    PlayerManager mgr = playersList[0];
                    mgr.pid = id;
                    //mgr.position = m_NetworkReader.ReadVector3();
                    //int playerCount = m_NetworkReader.ReadInt32();

                    //for (int i = 0; i < playerCount; ++i)
                    //{
                    //    GameObject otherPlayer = Instantiate(playerReference);
                    //    PlayerManager otherManager = otherPlayer.GetComponent<PlayerManager>();
                    //    otherManager.pid = m_NetworkReader.ReadUInt32();
                    //    otherManager.position = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());
                    //    otherManager.pRotation = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());
                    //    otherManager.pName = m_NetworkReader.ReadString();
                    //    playersList.Add(otherManager);
                    //}

                    //int enemyCount = m_NetworkReader.ReadInt32();

                    //for (int i = 0; i < enemyCount; ++i)
                    //{
                    //    GameObject enemy = Instantiate(enemyReference);
                    //    EnemyAI enemyManager = enemy.GetComponent<EnemyAI>();
                    //    enemyManager.pid = m_NetworkReader.ReadUInt32();
                    //    Debug.Log("ID: " + enemyManager.pid);
                    //    enemyManager.ePosition = m_NetworkReader.ReadVector3();
                    //    enemyManager.eRotation = m_NetworkReader.ReadVector3();
                    //    GameObject enemyUICanvas = GameObject.Find("EnemyUI");
                    //    BillBoard billboard = enemyUICanvas.GetComponent<BillBoard>();
                    //    billboard.camera = m_MainCamera;
                    //    enemyList.Add(enemyManager);
                    //}
                    SendInitialStats();
                    break;

                case (byte)Packets_ID.ID_MOVEMENT:
                    {
                        uint playerID = m_NetworkReader.ReadUInt32();

                        foreach (PlayerManager player in playersList)
                        {
                            if (player.pid == playerID)
                            {
                                //  ship.position = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), 0);
                                //  ship.pRotation = m_NetworkReader.ReadFloat();

                                // Step 8 : Instead of using ship.position, use server_pos and serverRotation
                                // set server position, server rotation
                                player.server_pos = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());
                                player.serverRotation = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());
                                Debug.Log("Server rotation: " + player.serverRotation);
                                // Lab 7 Task 1 : Read Extrapolation Data velocity x, velocity y & angular velocity
                                // set velocity and rotation velocity of ship (look at ship Manager)

                                player.velocity = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());
                            }
                        }
                    }

                    break;

                case (byte)Packets_ID.ID_NEWPLAYER:
                    {
                        GameObject playerObj = Instantiate(playerReference);
                        PlayerManager otherManager = playerObj.GetComponent<PlayerManager>();
                        otherManager.pid = m_NetworkReader.ReadUInt32();
                        playerObj.GetComponentInChildren<PlayerMovement>().Init(false);
                        otherManager.pName = m_NetworkReader.ReadString();
                        otherManager.position = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());
                        otherManager.pRotation = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());

                        playersList.Add(otherManager);
                    }
                    break;

                case (byte)Packets_ID.ID_CHANGESCENE:
                    {
                        string sceneName = m_NetworkReader.ReadString();
                        if (Application.CanStreamedLevelBeLoaded(sceneName))
                        {
                            Scene nextScene = SceneManager.GetSceneByName(sceneName);
                            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                            SceneManager.MoveGameObjectToScene(this.gameObject, nextScene);

                            //foreach (PlayerManager player in playersList)
                            //{
                            //    SceneManager.MoveGameObjectToScene(player.gameObject, nextScene);
                            //}
                        }
                        else
                        {
                            Debug.Log("Stream Level cannot be loaded!");
                            // load another scene?
                        }
                    }
                    break;

                case (byte)Packets_ID.ID_DESTROYENEMY:
                    {
                        uint enemyID = m_NetworkReader.ReadUInt32();
                        foreach (EnemyAI enemy in enemyList)
                        {
                            if (enemy.pid == enemyID)
                            {
                                enemyList.Remove(enemy);
                                Destroy(enemy.gameObject);
                                Debug.Log("Destroyed Enemy in Client");
                                break;
                            }
                        }
                    }
                    break;


                case (byte)Packets_ID.ID_DMGPLAYER:
                    {
                        uint playerID = m_NetworkReader.ReadUInt32();
                        float dmg = m_NetworkReader.ReadFloat();
                        foreach (PlayerManager player in playersList)
                        {
                            if (player.pid == playerID)
                            {
                                Debug.Log(dmg + "dmg dealt to player " + playerID);
                                player.hp -= dmg;
                                Uimanager UI = gameObject.GetComponent<Uimanager>();
                                UI.Hpdown(25.0f);
                                break;
                            }
                        }
                    }
                    break;

                case (byte)Packets_ID.ID_ADDSCORE:
                    {
                        uint playerID = m_NetworkReader.ReadUInt32();
                        float score = m_NetworkReader.ReadFloat();
                        foreach (PlayerManager player in playersList)
                        {
                            if (player.pid == playerID)
                            {
                                Debug.Log(score + "score added to player " + playerID);
                                player.Pscore += score;
                                Uimanager UI = gameObject.GetComponent<Uimanager>();
                                UI.Scoreup(25.0f);
                                break;
                            }
                        }
                    }
                    break;

                case (byte)Packets_ID.ID_ADDMONEY:
                    {
                        uint playerID = m_NetworkReader.ReadUInt32();
                        float money = m_NetworkReader.ReadFloat();
                        foreach (PlayerManager player in playersList)
                        {
                            if (player.pid == playerID)
                            {
                                Debug.Log(money + "money added to player " + playerID);
                                player.Pmoney += money;
                                Uimanager UI = gameObject.GetComponent<Uimanager>();
                                UI.Moneyup(25.0f);
                                break;
                            }
                        }
                    }
                    break;

                case (byte)Packets_ID.ID_REDUCEMONEY:
                    {
                        uint playerID = m_NetworkReader.ReadUInt32();
                        float money = m_NetworkReader.ReadFloat();
                        foreach (PlayerManager player in playersList)
                        {
                            if (player.pid == playerID)
                            {
                                Debug.Log(money + "money taken from player " + playerID);
                                player.Pmoney -= money;
                                Uimanager UI = gameObject.GetComponent<Uimanager>();
                                UI.Moneydown(1000.0f);
                                break;
                            }
                        }
                    }
                    break;

                case (byte)Packets_ID.ID_BOUGHTREVIVE:
                    {
                        uint playerID = m_NetworkReader.ReadUInt32();
                        bool revive = m_NetworkReader.ReadBoolean();
                        foreach (PlayerManager player in playersList)
                        {
                            if (player.pid == playerID)
                            {
                                player.previve = true;
                                Uimanager UI = gameObject.GetComponent<Uimanager>();
                                UI.Moneydown(1000.0f);
                                break;
                            }
                        }
                    }
                    break;

                case (byte)Packets_ID.ID_DMGENEMY:
                    {
                        uint enemyID = m_NetworkReader.ReadUInt32();
                        float dmg = m_NetworkReader.ReadFloat();
                        foreach (EnemyAI enemy in enemyList)
                        {
                            if (enemy.pid == enemyID)
                            {
                                enemy.hp -= dmg;
                                Debug.Log(dmg + "dmg dealt to enemy " + enemyID);
                                break;
                            }
                        }
                    }
                    break;

                case (byte)Packets_ID.ID_UPDATENEMY:
                    {
                        uint enemyID = m_NetworkReader.ReadUInt32();
                        Vector3 position = m_NetworkReader.ReadVector3();
                        Vector3 rotation = m_NetworkReader.ReadVector3();
                        string currentState = m_NetworkReader.ReadString();
                        float hp = m_NetworkReader.ReadFloat();
                        foreach (EnemyAI enemy in enemyList)
                        {
                            if (enemy.pid == enemyID)
                            {
                                enemy.position = position;
                                enemy.gameObject.transform.eulerAngles = rotation;
                                enemy.sm.SetCurrentState(currentState);
                                enemy.hp = hp;
                                //Debug.Log("Enemy HP: " + hp);
                                break;
                            }
                        }
                    }
                    break;

                case (byte)Packets_ID.ID_SPAWNPICKUP:
                    {
                        uint pickupID = m_NetworkReader.ReadUInt32();
                        int type = m_NetworkReader.ReadInt32();
                        Vector3 position = m_NetworkReader.ReadVector3();


                        if (type == 0)
                        {
                            GameObject pickupObject = Instantiate(healthPickupReference);
                            PickupManager pickupManager = pickupObject.GetComponent<PickupManager>();
                            pickupManager.pPosition = position;
                            pickupManager.pid = pickupID;
                            pickupList.Add(pickupManager);
                        }
                    }
                    break;
                case (byte)Packets_ID.ID_SHOOTBULLET:
                    {
                        uint bid = m_NetworkReader.ReadUInt32();    // bulletid
                        uint boid = m_NetworkReader.ReadUInt32();   // ownerid
                        Vector3 bulletPos = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());
                        GameObject bullet = Instantiate(bulletReference);
                        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
                        bulletRigidbody.position = bulletPos;
                        BulletManager bulletManager = bullet.GetComponent<BulletManager>();
                        Vector3 forward = m_NetworkReader.ReadVector3();
                        bulletRigidbody.AddForce(forward * 500);
                        bulletManager.pid = bid;
                        bulletManager.ownerID = boid;
                        bulletList.Add(bulletManager);
                    }
                    break;

                case (byte)Packets_ID.ID_DESTROYBULLET:
                    {
                        uint bid = m_NetworkReader.ReadUInt32();

                        foreach(BulletManager bullet in bulletList)
                        {
                            if(bullet.pid == bid)
                            {
                                Destroy(bullet.gameObject);
                            }
                        }
                    }
                    break;


                case (byte)Packets_ID.ID_SENDCONNECTSCENEINFO:
                    {
                        int playerCount = m_NetworkReader.ReadInt32();
                        GameObject[] playerDisplayList;
                        playerDisplayList = GameObject.FindGameObjectsWithTag("PlayerConnectionDisplay");
                        PlayerManager me = playersList[0];
                        Text clientName = playerDisplayList[0].transform.Find("PlayerName").GetComponent<Text>();
                        clientName.text = "" + me.pName;

                        Text clientStatus = playerDisplayList[0].transform.Find("Status").GetComponent<Text>();
                        clientStatus.text = "" + "Not In-Game";

                        for (int i = 0; i < playerCount; ++i)
                        {
                            GameObject otherPlayer = Instantiate(playerReference);
                            PlayerManager otherManager = otherPlayer.GetComponent<PlayerManager>();
                            otherManager.pid = m_NetworkReader.ReadUInt32();
                            otherManager.pName = m_NetworkReader.ReadString();
                            string status = m_NetworkReader.ReadString();


                            Text playerName = playerDisplayList[i + 1].transform.Find("PlayerName").GetComponent<Text>();
                            playerName.text = "" + otherManager.pName;
                            Text displayStatus = playerDisplayList[i + 1].transform.Find("Status").GetComponent<Text>();
                            displayStatus.text = "" + status;
                            playersList.Add(otherManager);
                        }
                    }
                    break;



                case (byte)Packets_ID.ID_NEWCONNECTPLAYER:
                    {
                        GameObject[] playerDisplayList;
                        playerDisplayList = GameObject.FindGameObjectsWithTag("PlayerConnectionDisplay");

                        GameObject otherPlayer = Instantiate(playerReference);
                        PlayerManager otherManager = otherPlayer.GetComponent<PlayerManager>();
                        otherManager.pid = m_NetworkReader.ReadUInt32();
                        otherManager.pName = m_NetworkReader.ReadString();

                        foreach (GameObject display in playerDisplayList)
                        {
                            Text name = display.transform.Find("PlayerName").GetComponent<Text>();
                            if (name.text == "")
                            {
                                name.text = "" + otherManager.pName;
                                Text status = display.transform.Find("Status").GetComponent<Text>();
                                status.text = "" + "Not In-Game";
                                break;
                            }
                        }
                    }
                    break;



                case (byte)Packets_ID.ID_SENDGAMEPLAYSCENEINFO:
                    {
                        PlayerManager me = playersList[0];
                        me.position = m_NetworkReader.ReadVector3();
                        me.pRotation = m_NetworkReader.ReadVector3();

                        int playerCount = m_NetworkReader.ReadInt32();

                        for (int i = 0; i < playerCount; ++i)
                        {
                            uint pid = m_NetworkReader.ReadUInt32();
                            Vector3 position = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());
                            Vector3 rotation = new Vector3(m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat(), m_NetworkReader.ReadFloat());

                            foreach(PlayerManager player in playersList)
                            {
                                if(player.pid == pid)
                                {
                                    player.position = position;
                                    player.pRotation = rotation;
                                    // TODO: Set health
                                    break;
                                }
                            }
                        }
                    }
                    break;


                case (byte)Packets_ID.ID_NEWGAMEPLAYPLAYER:
                    {
                        uint pid = m_NetworkReader.ReadUInt32();
                        Vector3 position = m_NetworkReader.ReadVector3();
                        Vector3 rotation = m_NetworkReader.ReadVector3();

                        foreach(PlayerManager player in playersList)
                        {
                            if(player.pid == pid)
                            {
                                player.position = position;
                                player.pRotation = rotation;
                                break;
                            }
                        }
                    }
                    break;

                case (byte)Packets_ID.ID_DESTROYHEALTHPICKUP:
                    {
                        uint playerPID = m_NetworkReader.ReadUInt32();
                        uint pickupPID = m_NetworkReader.ReadUInt32();

                        foreach (PlayerManager player in playersList)
                        {
                            if (player.pid == playerPID)
                            {
                                foreach(PickupManager pickup in pickupList)
                                {
                                    if(pickup.pid == pickupPID)
                                    {
                                        player.hp += pickup.GetHeal();
                                        pickupList.Remove(pickup);
                                        Destroy(pickup.gameObject);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    break;
                    //end
            }
        }
    }

    private void SendInitialStats()
    {
        if (m_NetworkWriter.StartWritting())
        {
            PlayerManager me = playersList[0];
            me.pName = userName;
            m_NetworkWriter.WritePacketID((byte)Packets_ID.ID_INITIALSTATS);
            m_NetworkWriter.Write(me.pName);
            m_NetworkWriter.Write(me.position.x);
            m_NetworkWriter.Write(me.position.y);
            m_NetworkWriter.Write(me.position.z);
            m_NetworkWriter.Write(me.pRotation.x);
            m_NetworkWriter.Write(me.pRotation.y);
            m_NetworkWriter.Write(me.pRotation.z);

            m_NetworkWriter.Send(serveruid, Peer.Priority.Immediate, Peer.Reliability.Reliable, 0);
        }


    }
    private void OnConnected(string address)
    {
        Debug.Log("[Client] Connected to " + address);

        //формируем/готовим информацию клиента
        m_ClientNetInfo.name = "Player_" + Environment.MachineName;
        m_ClientNetInfo.local_id = peer.incomingGUID;
        m_ClientNetInfo.client_hwid = SystemInfo.deviceUniqueIdentifier;
        m_ClientNetInfo.client_version = Application.version;
    }

    private void OnDisconnected(string reason)
    {
        Debug.LogError("[Client] Disconnected" + (reason.Length > 0 ? " with reason: " + reason : "..."));
    }
}
