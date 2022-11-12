using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    public Text playerName;

    public GameObject nextColorButton;
    public GameObject previusColorButton;
    public Color highlightColor;
    public Image backgroundImage;

    ExitGames.Client.Photon.Hashtable playerProporties = new ExitGames.Client.Photon.Hashtable();
    public Image playerAvatar;
    public Sprite[] avatars;

    Player player;

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        backgroundImage.color = highlightColor;
        nextColorButton.SetActive(true);
        previusColorButton.SetActive(true);
    }

    public void OnClickPreviusColor()
    {
        if ((int)playerProporties["playerAvatar"] == 0)
        {
            playerProporties["playerAvatar"] = avatars.Length - 1;
        } 
        else
        {
            playerProporties["playerAvatar"] = (int)playerProporties["playerAvatar"] - 1;
        }
        
        PhotonNetwork.SetPlayerCustomProperties(playerProporties);
    }

    public void OnClickNextColor()
    {
        if ((int)playerProporties["playerAvatar"] == avatars.Length-1)
        {
            playerProporties["playerAvatar"] = 0;
        }
        else
        {
            playerProporties["playerAvatar"] = (int)playerProporties["playerAvatar"] + 1;
        }

        PhotonNetwork.SetPlayerCustomProperties(playerProporties);
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("playerAvatar"))
        {
            playerAvatar.sprite = avatars[(int)player.CustomProperties["playerAvatar"]];
            playerProporties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
        }
        else
        {
            playerProporties["playerAvatar"] = 0;
        }
    }
    
}
