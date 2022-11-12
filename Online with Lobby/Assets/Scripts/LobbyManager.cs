using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;


    public RoomButton roomButtonPrefab;
    List<RoomButton> roomButtonsList = new List<RoomButton>();
    public Transform contentObject;


    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public GameObject startGame;


    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }


    public void onClickCreate()
    {
        if(roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 8, BroadcastPropsChangeToAll = true });
        }
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text =  "Room: " + PhotonNetwork.CurrentRoom.Name;
        
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= nextUpdateTime)
        {
            updateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }

        

    }

    void updateRoomList(List<RoomInfo> list)
    {
        foreach(RoomButton item in roomButtonsList)
        {
            Destroy(item.gameObject);
        }
       
        
        roomButtonsList.Clear();
        

        foreach(RoomInfo room in list)
        {
            if (room.RemovedFromList)
                continue;
            RoomButton newRoom = Instantiate(roomButtonPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomButtonsList.Add(newRoom);
        }
           
    }
    
    public void JoinRoom(string roomName, RoomButton roomButton)
    {
        PhotonNetwork.JoinRoom(roomName);
        

    }

    

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }



    //Inside the room panel listing players
    void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if(PhotonNetwork.CurrentRoom == null)
            return;
        
        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);

            if(player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }

            playerItemsList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }


    //start game button
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            startGame.SetActive(true);
        } else
        {
            startGame.SetActive(false);
        }
    }

    public void OnClickStartGameButton()
    {
        PhotonNetwork.LoadLevel("Game");
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }
}
