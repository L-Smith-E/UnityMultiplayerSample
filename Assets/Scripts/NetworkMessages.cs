using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkMessages
{
    public enum Commands{
        PLAYER_UPDATE,
        SERVER_UPDATE,
        HANDSHAKE,
        PLAYER_INPUT,
        CONNECTING_CLIENT,
        DROPPED_CLIENT,
        HEARTBEAT,
        PLAYERID_CMD,
    }

    [System.Serializable]
    public class NetworkHeader{
        public Commands cmd;
    }

    [System.Serializable]
    public class HandshakeMsg:NetworkHeader{
        public NetworkObjects.NetworkPlayer player;
        public HandshakeMsg(){      // Constructor
            cmd = Commands.HANDSHAKE;
            player = new NetworkObjects.NetworkPlayer();
        }
    }
    
    [System.Serializable]
    public class PlayerUpdateMsg:NetworkHeader{
        public NetworkObjects.NetworkPlayer player;
        public PlayerUpdateMsg(List<NetworkObjects.NetworkPlayer> listOfPlayers){      // Constructor
            cmd = Commands.PLAYER_UPDATE;
            player = new NetworkObjects.NetworkPlayer[listOfPlayers.Count];
        }
    };

    public class PlayerInputMsg:NetworkHeader{
        public Input myInput;
        public PlayerInputMsg(){
            cmd = Commands.PLAYER_INPUT;
            myInput = new Input();
        }
    }
   
    //player connecting
    [System.Serializable]
    public class  ServerUpdateMsg:NetworkHeader{
        public List<NetworkObjects.NetworkPlayer> player_c;
        public ServerUpdateMsg(){      // Constructor
            cmd = Commands.SERVER_UPDATE;
            player_c = new NetworkObjects.NetworkPlayer();
        }
    }

    [System.Serializable]
    public class ConnectingClient : NetworkHeader
    {
        public NetworkObjects.NetworkPlayer player;
        public ConnectingClient()
        {
            cmd = Commands.CONNECTING_CLIENT;
            player = new 
        }
    }
    
    [System.Serializable]
    public class DroppedClientMsg : NetworkHeader
    {
        public NetworkObjects.NetworkPlayer[] players;
        public DroppedClientMsg(List<NetworkObjects.NetworkPlayer> listOfPlayers)
        {
            players = new NetworkObjects.NetworkPlayer[listOfPlayers.Count];
            cmd = Commands.DROPPED_CLIENT;
        }
    }
} 

namespace NetworkObjects
{
    [System.Serializable]
    public class NetworkObject{
        public string id;
    }
    [System.Serializable]
    public class NetworkPlayer : NetworkObject{
        public Color cubeColor;
        public Vector3 cubPos;

        public NetworkPlayer(){
            cubeColor = new Color();
        }
    }
}
