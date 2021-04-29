using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class photonHandler : MonoBehaviour
{
    public RoomController roomController;
    public int numPlayers;
    public GameObject mainPlayer;
    public GameObject menuCanvas;
    public GameObject waitCanvas;
    public GameObject startButton;
    public TMP_InputField nameInput;
    public GameObject joinRoom;
    public GameObject createRoom;
    public bool start = false;
    public bool check = true;
    public int spawnId;
    public PhotonView photonView;
    public string code;

    void Awake()
    {
        Debug.Log("start");
        DontDestroyOnLoad(this.transform);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void Start()
    {
        //roomController = GameObject.Find("PhotonManager").GetComponent<RoomController>();
        createRoom = GameObject.Find("CREATEROOM");
        joinRoom = GameObject.Find("JOINROOM");
        //startButton = GameObject.Find("startAnyway");
        //nameInput = GameObject.Find("InputField").GetComponent<InputField>();

        createRoom.SetActive(false);
        joinRoom.SetActive(false);
        //startButton.SetActive(false);
    }

    public void CreateNewRoom()
    {
        //Debug.Log(nameInput.text);
        if(nameInput.text != "" || nameInput.text != null)
            PhotonNetwork.CreateRoom(/*roomController.createRoom.text*/GenerateRandomNo(), new RoomOptions() {MaxPlayers = 4}, null);
    }

    public void JoinOrCreateRoom()
    {
        if(nameInput.text != "" || nameInput.text != null)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;

            PhotonNetwork.JoinOrCreateRoom(roomController.JoinRoom.text, roomOptions, TypedLobby.Default);
        }
    }

    public void Play()
    {
        createRoom.SetActive(true);
        joinRoom.SetActive(true);
    }

    public string GenerateRandomNo()
    {
        int _min = 1000;
        int _max = 9999;
        int no = Random.Range(_min, _max);
        code = no.ToString();
        return no.ToString();
    }

    public void GameScene()
    {
        menuCanvas.SetActive(false);
        waitCanvas.SetActive(true);
        GameObject.Find("code").GetComponent<TMP_Text>().text += (" " + code);
        
        if(PhotonNetwork.isMasterClient)
        {
            startButton.SetActive(true);
        }
    }

    public void startButtonClicked()
    {
        //start = true;
        photonView.RPC("StartGame", PhotonTargets.All, null);
        PhotonNetwork.room.open = false;
        //PhotonNetwork.LoadLevel("SewerMap");
    }

    [PunRPC]
    public void StartGame()
    {
        //Scene scene;
        numPlayers = PhotonNetwork.room.PlayerCount;
        //Debug.Log(scene.buildIndex);
        //Debug.Log("brfore load");
        PhotonNetwork.LoadLevel("SewerMap");
        //Debug.Log("after load");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LoadLevel("MainMenu");
    }

    void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu2");
    }

    void OnJoinedRoom()
    {
        GameScene();
        spawnId = PhotonNetwork.room.PlayerCount;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("loaded");
        if(scene.name == "SewerMap")
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        //Debug.Log("spawned");
        GameObject player = PhotonNetwork.Instantiate(mainPlayer.name, mainPlayer.transform.position, mainPlayer.transform.rotation, 0);
        Debug.Log(nameInput.text);
        player.name = nameInput.text;
    }

    void OnDisconnectedFromPhoton()
    {
        Debug.Log("disconnected");
    }

    private void Update()
    {
        if (startButton == null)
        {
            startButton = GameObject.Find("startAnyway");
            startButton.SetActive(false);
        }

        if (nameInput == null)
            nameInput = GameObject.Find("NAME").GetComponent<TMP_InputField>();
            //nameInput = GameObject.Find("InputField").GetComponent<InputField>();

        if (roomController == null)
            roomController = GameObject.Find("PhotonManager").GetComponent<RoomController>();

        if (menuCanvas == null)
        {
            menuCanvas = GameObject.Find("MAINMENU");
            menuCanvas.SetActive(false);
        }

        if (waitCanvas == null)
        {
            waitCanvas = GameObject.Find("Waiting");
            waitCanvas.SetActive(false);
        }
    }

    /*[PunRPC]
    void SyncGameStart(bool _start)
    {
        start = _start;
    }*/
}
