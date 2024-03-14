using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Camera camera;
    public GameObject playerView;
    public TMP_Text speed;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject waitingPanel;

    public Material[] playerMaterials;
    public Vector3[] spawnPositions;

    private PhotonView photonView;
    private bool gameStarted = false;
    private bool gameEnded = false;
    
    public static GameManager instance = null;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }


    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(!gameStarted && PhotonNetwork.PlayerList.Length == 2) {
            StartGame();
        }
        
        if(gameStarted)
        {
            UpdateSpeed();
        }
    }

    public void StartGame()
    {
        SpawnPlayer();
        SetCameraFocus(playerView);
        gameStarted = true;
        waitingPanel.SetActive(false);
        playerView.GetComponent<PlayerMovement>().SetPlayerMaterial();
    }
    
    private void SpawnPlayer()
    {
        if (PhotonNetwork.InRoom) // Multi
        {
            int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPositions.Length;
            Vector3 spawnPosition = spawnPositions[index];

            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
            if (player.GetComponent<PhotonView>().IsMine)
            {
                playerView = player;
            }
        }
        else // Solo
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            player.GetComponent<PlayerMovement>().SetRandomColor();
            player.GetComponent<PlayerMovement>().solo = true;
            
            playerView = player;
        }
    }
    
    private void SetCameraFocus(GameObject target)
    {
        camera.GetComponent<CameraDrift>().target = target.transform;
        camera.GetComponent<CameraDrift>().enabled = true;
    }

    private void UpdateSpeed()
    {
        float speedMultiplier = 2f;
        speed.text = (playerView.GetComponent<PlayerMovement>().forwardSpeed * speedMultiplier).ToString("F1");
    }
    
    public void InitiateEndGame(GameObject deadPlayer)
    {
        if(!gameEnded)
        {
            int deadPlayerViewID = deadPlayer.GetComponent<PhotonView>().ViewID;
            photonView.RPC("EndGame", RpcTarget.All, deadPlayerViewID);
        }
    }

    [PunRPC]
    private void EndGame(int deadPlayerViewID)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonView.Find(deadPlayerViewID).OwnerActorNr) // Mort
        {
            camera.GetComponent<CameraDrift>().absoluteOffsetY = true;
            camera.GetComponent<CameraDrift>().speed = .5f;
            camera.GetComponent<CameraDrift>().offset = new Vector3(0f, 100f, 0f);
            gameOverPanel.SetActive(true);
        }
        else // Win
        {
            camera.GetComponent<CameraDrift>().absoluteOffsetY = true;
            camera.GetComponent<CameraDrift>().speed = .5f;
            camera.GetComponent<CameraDrift>().offset = new Vector3(0f, 100f, 0f);
            winPanel.SetActive(true);
        }
        gameEnded = true;
    }

    public void OnMenuClick()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1) MigrateMaster();
            else
            {
                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                PhotonNetwork.LeaveRoom();
            }
        }
        PhotonNetwork.LoadLevel("Lobby");
    }

    private void MigrateMaster()
    {
        var dict = PhotonNetwork.CurrentRoom.Players;
        if (PhotonNetwork.SetMasterClient(dict[dict.Count - 1]))
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
