using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class RoomButton : MonoBehaviour
{
    public Text roomName;

    LobbyManager manager;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void OnClickRoomButton()
    {
        manager.JoinRoom(roomName.text, this);

    }

  

}
