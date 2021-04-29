using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomController : MonoBehaviour
{
    //public InputField createRoom;
    public TMP_InputField JoinRoom;
    public photonHandler pHandler;

    void Awake()
    {
        DontDestroyOnLoad(this.transform);
        //SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void Update()
    {
        //if (createRoom == null)
          //  createRoom = GameObject.Find("CreateRoomInput").GetComponent<InputField>();

        if (JoinRoom == null)
            JoinRoom = GameObject.Find("joinRoomInputField").GetComponent<TMP_InputField>();
    }

    public void onClickCreateRoom()
    {
        //if(createRoom.text.Length > 0)
        //{
            pHandler.CreateNewRoom();
        //}
    }

    public void onClickJoinRoom()
    {
        pHandler.JoinOrCreateRoom();
    }
}
