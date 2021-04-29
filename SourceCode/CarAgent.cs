using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAgent : MonoBehaviour
{
    public photonHandler pHandler;
    public int spawnId;
    public int NextCheckpointIndex;
    public GameObject stats;
    public GameObject temp;
    public FinalPositions fp;
    public PhotonView photonView;
    public SetRaceTrack area;
    public Rigidbody rigidbody;
    public GameObject b1;
    public GameObject b2;
    public GameObject b3;
    public GameObject b4;
    public bool once = true;
    public bool onceAgain = true;
    Vector3 offset;
    int nxtCheckpoint;

    void Start()
    {
        pHandler = GameObject.Find("PhotonStatic").GetComponent<photonHandler>();
        spawnId = pHandler.spawnId;
        if(spawnId == 1)
            offset = new Vector3(1f, 0f, 1f);
        else if(spawnId == 2)
            offset = new Vector3(5f, 0f, 1f);
        else if(spawnId == 3)
            offset = new Vector3(10f, 0f, 1f);
        else if(spawnId == 4)
            offset = new Vector3(15f, 0f, 1f);

        if(!photonView.isMine)
            return;
        stats = GameObject.FindGameObjectWithTag("stats");
        temp = GameObject.Find("Canvas");
        temp.SetActive(false);
        if(photonView.isMine)
            stats.SetActive(false);
        fp = GameObject.Find("FinalPositions").GetComponent<FinalPositions>();
        area = GameObject.Find("SewerMap").GetComponent<SetRaceTrack>();
        AgentReset();
        b1 = GameObject.Find("b3");
        b2 = GameObject.Find("b4");
        b3 = GameObject.Find("b5");
        b4 = GameObject.Find("b6");
        b3.SetActive(false);
        b4.SetActive(false);
    }

    public void AgentReset()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        area.ResetCarAgent(this, offset);
    }

    private void GotCheckpoint()
    {
        NextCheckpointIndex = (NextCheckpointIndex + 1) % area.checkpoints.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("checkpoint") &&
            other.gameObject == area.checkpoints[NextCheckpointIndex])
        {
            GotCheckpoint();
        }

        else if(other.transform.CompareTag("finalCheckpoint") && once && other.gameObject == area.checkpoints[NextCheckpointIndex])
        {
            fp.AddPosition(this);
            if(photonView.isMine)
            {
                stats.SetActive(true);
                //GetComponent<CarController>().enabled = false;
                //GetComponent<CarController>().Cam1.SetActive(true);
                //this.gameObject.SetActive(false);
                GetComponent<CarController>().FreezeAgent();
            }
            once = false;
        }
    }

    void Update()
    {
        if(!photonView.isMine)
        {
            SetReceivedValues();
            //stats = GameObject.FindGameObjectWithTag("stats");
            fp = GameObject.Find("FinalPositions").GetComponent<FinalPositions>();
            area = GameObject.Find("SewerMap").GetComponent<SetRaceTrack>();
            return;
        }

        if(NextCheckpointIndex > 5)
        {
            b1.SetActive(false);
            b2.SetActive(false);
            b3.SetActive(true);
            b4.SetActive(true);
        }

        if(Input.GetKey("r"))
            AgentReset();

        if(pHandler.numPlayers == area.carAgents.Count && onceAgain)
        {
            temp.SetActive(true);
            StartCoroutine(temp.GetComponent<countDown>().StartCountDown(this.gameObject));
            //Debug.Log("afterCountDown");
            //GetComponent<CarController>().ThawAgent();
            onceAgain = false;
        }
    }

    void SetReceivedValues()
    {
        NextCheckpointIndex = nxtCheckpoint;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(NextCheckpointIndex);
        }
        else
        {
            nxtCheckpoint = (int)stream.ReceiveNext();
        }
    }
}
