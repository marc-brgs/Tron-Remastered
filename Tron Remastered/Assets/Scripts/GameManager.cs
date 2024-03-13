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

    public float spawnMinX = -20f;
    public float spawnMaxX = 20f;
    public float spawnMinY = -20f;
    public float spawnMaxY = 20f;

    private PhotonView photonView;


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
        Random.InitState(42); // Sync random seed

        photonView = GetComponent<PhotonView>();
        SpawnPlayers();
        SetCameraFocus(playerView);
    }

    void Update()
    {
        UpdateSpeed();
    }

    private void SpawnPlayers()
    {
        if (PhotonNetwork.InRoom) // Multi
        {
            Vector3 randomPosition = new Vector3(Random.Range(spawnMinX, spawnMaxX), 0f, Random.Range(spawnMinY, spawnMaxY));
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
            //player.GetComponent<PlayerMovement>().gameManager = this;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                playerView = player;
            }
        }
        else // Solo
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            //player.GetComponent<PlayerMovement>().gameManager = this;
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
        int deadPlayerViewID = deadPlayer.GetComponent<PhotonView>().ViewID;
        photonView.RPC("EndGame", RpcTarget.All, deadPlayerViewID);
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
