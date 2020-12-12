using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkMessages;
using NetworkObjects;
using System;
using System.Text;
using System.Collections.Generic;

public class NetworkClient : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public string serverIP;
    public ushort serverPort;

    public GameObject playerGO;
    public string playerID;
    public Dictionary<string, GameObject> currentPlayers;
    private List<Players> newPlayers = new List<Players>();
    
    void Start ()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndPoint.Parse(serverIP,serverPort);
        m_Connection = m_Driver.Connect(endpoint);
        newPlayers = new List<string>();
        droppedPlayers = new List<string>();
        currentPlayers = new Dictionary<string, GameObject>();
        initialSetofPlayers = new ListOfPlayers();
    }
    
    void SendToServer(string message){
        var writer = m_Driver.BeginSend(m_Connection);
        NativeArray<byte> bytes = new NativeArray<byte>(Encoding.ASCII.GetBytes(message),Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }

    void OnConnect(){
        Debug.Log("We are now connected to the server");

        //// Example to send a handshake message:
        // HandshakeMsg m = new HandshakeMsg();
        // m.player.id = m_Connection.InternalId.ToString();
        // SendToServer(JsonUtility.ToJson(m));
    }

    void OnData(DataStreamReader stream){
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length,Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch(header.cmd){
            case Commands.HANDSHAKE:
            HandshakeMsg hsMsg = JsonUtility.FromJson<HandshakeMsg>(recMsg);
            Debug.Log("Handshake message received!");
            break;
            case Commands.PLAYER_UPDATE:
            PlayerUpdateMsg puMsg = JsonUtility.FromJson<PlayerUpdateMsg>(recMsg);
            Debug.Log("Player update message received!");
            break;
            case Commands.SERVER_UPDATE:
            ServerUpdateMsg suMsg = JsonUtility.FromJson<ServerUpdateMsg>(recMsg);
            Debug.Log("Server update message received!");
            break;
            case Commands.CONNECTING_CLIENT:
            ConnectClientMsg ccMsg = JsonUtility.FromJson<ConnectClientMsg>(recMsg);
            Debug.Log("Client connecting update message received!");
            case Commands.DROPPED_CLIENT:
            DroppedClientMsg dcMsg = JsonUtility.FromJson<DroppedClientMsg>(recMsg);
                DestroyPlayers();
                Debug.Log("Dropped CLient update message received!");
            default:
            Debug.Log("Unrecognized message received!");

            break;
        }
    }

    //private void SpawnPlayers()
    //{
    //    if (newPlayers.Count > 0)
    //    {
    //        foreach (string playerID in newPlayers)
    //        {
    //            currentPlayers.Add(playerID, Instantiate(playerGO, new Vector3(0, 0, 0), Quaternion.identity));
    //            currentPlayers[playerID].name = playerID;
    //        }
    //    }
    //}

    //private void DropPlayers(NetworkObjects.NetworkPlayer[] players)
    //{
    //    foreach (NetworkObjects.NetworkPlayer player in players)
    //    {
    //        for (int i = 0; i < newPlayers.Count ; i++)
    //        {
    //            if currentPlayers[playerID].name = 
    //        }
    //    }
    //}

    void SpawnPlayers()
    {
        if (newPlayers.Count > 0)
        {
            foreach (string playerID in newPlayers)
            {
                currentPlayers.Add(playerID, Instantiate(playerGO, new Vector3(0, 0, 0), Quaternion.identity));
                currentPlayers[playerID].name = playerID;
            }
            newPlayers.Clear();
        }
        if (initialSetofPlayers.players.Length > 0)
        {
            Debug.Log(initialSetofPlayers);
            foreach (Player player in initialSetofPlayers.players)
            {
                if (player.id == myAddress)
                    continue;
                currentPlayers.Add(player.id, Instantiate(playerGO, new Vector3(0, 0, 0), Quaternion.identity));
                currentPlayers[player.id].GetComponent<Renderer>().material.color = new Color(player.color.R, player.color.G, player.color.B);
                currentPlayers[player.id].name = player.id;
            }
            initialSetofPlayers.players = new Player[0];
        }
    }

    void UpdatePlayers()
    {
        if (lastestGameState.players.Length > 0)
        {
            foreach (NetworkMan.Player player in lastestGameState.players)
            {
                string playerID = player.id;
                currentPlayers[player.id].GetComponent<Renderer>().material.color = new Color(player.color.R, player.color.G, player.color.B);
                currentPlayers[player.id].transform.position = new Vector3(player.position.x, player.position.y, player.position.z);
            }
            lastestGameState.players = new Player[0];
        }
    }

    void DestroyPlayers()
    {
        if (droppedPlayers.Count > 0)
        {
            foreach (string playerID in droppedPlayers)
            {
                Debug.Log(playerID);
                Debug.Log(currentPlayers[playerID]);
                Destroy(currentPlayers[playerID].gameObject);
                currentPlayers.Remove(playerID);
            }
            droppedPlayers.Clear();
        }
    }

    public void PositionUpdate(GameObject PlayerGo)
    {
        ServerUpdateMsg serverUpdateMsg = new ServerUpdateMsg();
        //serverUpdateMsg.PlayerGo
        serverUpdateMsg  = currentPlayers[myAddress].transform.position.x;
        //playerPos.pPos.y = currentPlayers[myAddress].transform.position.y;
        //playerPos.pPos.z = currentPlayers[myAddress].transform.position.z;
        //
        Byte[] sendBytes = Encoding.ASCII.GetBytes(JsonUtility.ToJson(serverUpdateMsg));
        udp.Send(sendBytes, sendBytes.Length);
    }

    private void UpdatePlayer
    void Disconnect(){
        m_Connection.Disconnect(m_Driver);
        m_Connection = default(NetworkConnection);
    }

    void OnDisconnect(){
        Debug.Log("Client got disconnected from server");
        m_Connection = default(NetworkConnection);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }   
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        cmd = m_Connection.PopEvent(m_Driver, out stream);
        while (cmd != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                OnConnect();
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                OnDisconnect();
            }

            cmd = m_Connection.PopEvent(m_Driver, out stream);
        }
    }
}