using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class SetRaceTrack : MonoBehaviour
{
    //public PhotonView photonView;
    public CinemachineSmoothPath racePath;
    public GameObject[] Agents;
    public List<CarAgent> carAgents = new List<CarAgent>();
    public GameObject checkPointPrefab;
    public List<GameObject> checkpoints;

    void Start()
    {
        checkpoints = new List<GameObject>();
        int NoCheckpoints = (int)racePath.MaxUnit(CinemachinePathBase.PositionUnits.PathUnits);
    
        for(int i = 0;i < NoCheckpoints;i++)
        {
            GameObject checkpoint = Instantiate<GameObject>(checkPointPrefab);

            if(i == NoCheckpoints - 1)
                checkpoint.tag = "finalCheckpoint";

            checkpoint.transform.SetParent(racePath.transform);
            checkpoint.transform.localPosition = racePath.m_Waypoints[i].position;
            checkpoint.transform.rotation = racePath.EvaluateOrientationAtUnit(i,CinemachinePathBase.PositionUnits.PathUnits);

            checkpoints.Add(checkpoint);
        }
    }

    public void ResetCarAgent(CarAgent agent, Vector3 offset)
    {
        int prevCheckpointIndex = agent.NextCheckpointIndex - 1;
        if(prevCheckpointIndex == -1)
            prevCheckpointIndex = 0;

        float Startposition = racePath.FromPathNativeUnits(prevCheckpointIndex , CinemachinePathBase.PositionUnits.PathUnits);

        Vector3 basePosition = racePath.EvaluatePosition(Startposition);
        Quaternion orientation = racePath.EvaluateOrientation(Startposition);
        Vector3 offsetPosition = Vector3.right * (carAgents.IndexOf(agent) - carAgents.Count/2f);
        //Debug.Log(offsetPosition);
        agent.transform.position = basePosition + orientation * offset;//offsetPosition;
        agent.transform.rotation = orientation;
    }

    void Update()
    {
        /*if(!photonView.isMine)
        {
            return;
        }*/

        Agents = GameObject.FindGameObjectsWithTag("Player");
        carAgents = new List<CarAgent>();
        foreach(GameObject gobj in Agents)
        {
            if(carAgents.Count < 4)
                carAgents.Add(gobj.GetComponent<CarAgent>());
        }

        /*foreach(CarAgent car in carAgents)
        {
            Debug.Log(car);
        }*/

        //foreach(CarAgent temp in carAgents)
        //    photonView.RPC("SyncAgents", PhotonTargets.Others, temp);
    }

    /*[PunRPC]
    void SyncAgents(CarAgent agent)
    {
        carAgents.Add(agent);
    }*/
    
}
