using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class photonConnect : MonoBehaviour
{
    public string versionname = "0.1";
    public GameObject MainMenu;
    public GameObject Retry;

    void Awake()
    {
        DontDestroyOnLoad(this.transform);
        //SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void Update()
    {
        if(MainMenu == null)
            MainMenu = GameObject.Find("MAINMENU");
    
        if(Retry == null)
        {
            Retry = GameObject.Find("Retry");
            Retry.SetActive(false);
        }
    }

    void Start()
    {
        connectToPhoton();
    }

    public void connectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings(versionname);
        Debug.Log("connecting to photon.....");
    }

    void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected to master");
    }

    void OnJoinedLobby()
    {
        Retry.SetActive(false);
        MainMenu.SetActive(true);
        Debug.Log("joined lobby");
    }

    void OnDisconnectedFromPhoton()
    {
        MainMenu.SetActive(false);
        Retry.SetActive(true);
        Debug.Log("disconnected");
    }

    public void Re_Connect()
    {
        connectToPhoton();
    }
}
