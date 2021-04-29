using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPositions : MonoBehaviour
{
    //public PhotonView photonView;
    public List<CarAgent> positions;

    public void AddPosition(CarAgent agent)
    {
        positions.Add(agent);
    }

    /*void Update()
    {
        foreach(CarAgent temp in positions)
            photonView.RPC("SyncPositions", PhotonTargets.All, temp);
    }

    [PunRPC]
    void SyncPositions(CarAgent agent)
    {
        positions.Add(agent);
    }*/
}
