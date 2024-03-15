using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomInput;

    public void QuickPlay()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.JoinOrCreateRoom("a", roomOptions, TypedLobby.Default);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomInput.text);
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}
