using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalStats : MonoBehaviour
{
    public FinalPositions fp;
    public GameObject area;
    public string[] temp;
    public Text[] txt;
    public photonHandler pH;


    void Start()
    {
        fp = GameObject.Find("FinalPositions").GetComponent<FinalPositions>();
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach(CarAgent agent in fp.positions)
        {
            temp[i] = agent.gameObject.name;
            i++;
        }

        for(int j = 0;j < i;j++)
        {
            txt[j].text = temp[j];
        }

        if(pH == null)
        {
            pH = GameObject.Find("PhotonStatic").GetComponent<photonHandler>();
        }
    }

    /*public void GoBack()
    {
        for(int i = 0;i < area.GetComponent<SetRaceTrack>().Agents.Length;i++)
        {
            Destroy(area.GetComponent<SetRaceTrack>().Agents[i]);
        }

        area.GetComponent<SetRaceTrack>().Agents = new GameObject[4];
        area.GetComponent<SetRaceTrack>().carAgents = new List<CarAgent>();
        fp.positions = new List<CarAgent>();
        PhotonNetwork.Disconnect();
        Application.LoadLevel(0);
    }*/

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        //PhotonNetwork.LeaveRoom();
        //Destroy(pH.gameObject);
        PhotonNetwork.LeaveRoom();
    }

    void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu2");
    }

    //void OnLeftRoom()
    //{
      //  PhotonNetwork.LoadLevel(0);
    //}
}
