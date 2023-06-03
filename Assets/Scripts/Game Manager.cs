using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
public class GameManager : MonoBehaviourPunCallbacks
{
    int debCount = 0;

    private bool m_IsCurrentPlayerSpawned = false;

    public bool gameEnded = false;

    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerMovement[] players;
    private int playersInGame;
    private List<int> pickedSpawnIndex;
    public GameObject imageTarget;
    //instance
    public static GameManager instance;
    private void Awake()
    {
    }
    private void Start()
    {
        instance = this;
        Debug.LogWarning("Game Manager Start");

        spawnPoints = new Transform[2];

        playerPrefabLocation = "Robot";
        pickedSpawnIndex = new List<int>();
        players = new PlayerMovement[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
    private void Update()
    {
        if(debCount++ > 1000)
        {
            Debug.LogWarning(ARPlacementDetection.s_StartGame);
            debCount = 0;
        }
        if(!ARPlacementDetection.s_StartGame)
        {
            return;
        }
        Debug.LogWarning("Ready!");
        Debug.LogWarning("Plane position: " + GameObject.FindGameObjectWithTag("Plane").transform.position);


        // foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Player"))
        // {
        //    gameObj.transform.SetParent(imageTarget.transform);
        // }
        // for (int i = 1; i < imageTarget.transform.childCount; i++)
        // {
        //     imageTarget.transform.GetChild(i).gameObject.SetActive(ARPlacementDetection.s_StartGame);
        // }
    }
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        Debug.LogWarning("Players in Game = " + playersInGame);
        // if (playersInGame == PhotonNetwork.PlayerList.Length)
        if(ARPlacementDetection.s_StartGame && !m_IsCurrentPlayerSpawned)
        {
            Debug.LogWarning("Plane Initial position: " + GameObject.FindGameObjectWithTag("Plane").transform.position);

            // spawnPoints[0] = GameObject.FindGameObjectWithTag("Plane").transform;
            // spawnPoints[0].position = new Vector3(spawnPoints[0].position.x, spawnPoints[0].position.y+1.01f, spawnPoints[0].position.z-1);
            // spawnPoints[1] = GameObject.FindGameObjectWithTag("Plane").transform;
            // spawnPoints[1].position = new Vector3(spawnPoints[1].position.x, spawnPoints[1].position.y+1.01f, spawnPoints[1].position.z+1);
            SpawnPlayer();
        }
    }
    void SpawnPlayer()
    {
        Transform spawnPosition;
        Vector3 instantiatePosition;
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("On master client");
            spawnPosition = GameObject.FindGameObjectWithTag("Plane").transform;
            instantiatePosition = new Vector3(spawnPosition.position.x, spawnPosition.position.y + 1.01f, spawnPosition.position.z - 1.0f);
        }
        else
        {
            Debug.LogWarning("Not on master client"); 
            spawnPosition = GameObject.FindGameObjectWithTag("Plane").transform;
            instantiatePosition = new Vector3(spawnPosition.position.x, spawnPosition.position.y + 1.01f, spawnPosition.position.z + 1.0f);
        }
        instantiatePosition = new Vector3(instantiatePosition.x - 13.20967f + 0.03f, instantiatePosition.y + 17.31141f + 0.18f, instantiatePosition.z - 10.39281f - 2f);
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, instantiatePosition, Quaternion.identity);
        m_IsCurrentPlayerSpawned = true;
        Debug.LogWarning("Player Instatiated at " + instantiatePosition);
        //intialize the player
        PhotonView playerPhotonView = playerObject.transform.GetChild(0).gameObject.GetComponent<PhotonView>();
        playerPhotonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
    public PlayerMovement GetPlayer(int playerID)
    {
        return players.First(x => x.id == playerID);
    }
    public PlayerMovement GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }
}