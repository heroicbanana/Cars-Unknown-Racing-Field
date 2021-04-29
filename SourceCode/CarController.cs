using UnityEngine;
using System.Collections;
using UnityEngine.VFX;
using UnityEngine.UI;

 public class CarController : Photon.MonoBehaviour
 {
    public PhotonView photonView;
    public float maxTorque = 50f;
    public float brakeStrength = 300f;
    public float HandBrakeStrength = 300f;
    public float maxSteerAngle = 30f;
    public float Decelaration = 1000f;
    public float ReverseSpeed = 200f;
    public int gear = 0;
    public int currSpeed;
    public Transform centerOfMass;
    public Transform[] tireMeshes = new Transform[4];
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public ParticleSystem ps1;
    public ParticleSystem ps2;
    public bool isSmoke = false;
    public GameObject Cam1;
    public GameObject Cam2;
    public GameObject CineCam;
    //public GameObject ps1;
    //public GameObject ps2;
    public bool isReverse;
    public bool BrakeState;
    public bool HandBrakeState;
    public bool accelerateState;
    public bool DecelarateState;
    public bool idleState;
    public bool isFlat = true;
    public bool isPause = false;
    public Rigidbody m_rigidBody;
    public GameObject NameCanvas;
    public Text Name;
    public float freeze = 1f;
    public GameObject pauseMenu;

    Vector3 currPos = new Vector3();
    Vector3 prevPos = new Vector3();
    Vector3 Direction = new Vector3();
    Vector3 forward = new Vector3();
    Vector3 selfPos;
    Quaternion selfRot;
    Vector3 recPos = new Vector3();
    Vector3 recVel = new Vector3();
    Vector3 recAngVel = new Vector3();
    Quaternion recRot = new Quaternion();

    void Start()
    {
        if(photonView.isMine)
            FreezeAgent();
            
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.centerOfMass = centerOfMass.localPosition;
        if(!photonView.isMine)
        {
            //Debug.Log("removedCamera");
            Name.text = this.gameObject.name;
            CineCam.SetActive(false);   
            Cam2.SetActive(false);
            return;
        }

        pauseMenu = GameObject.Find("PauseMenu");
        pauseMenu.SetActive(false);
        NameCanvas.SetActive(false);
        Cam1 = GameObject.Find("Camera");
        Cam1.SetActive(false);
    }
 
    void Update()
    {
        if(!photonView.isMine)
        {
            if(isSmoke)
            {
                ps1.Play();
                ps2.Play();
            }

            else
            {
                ps1.Stop();
                ps2.Stop();
            }
/*
            transform.position = recPos;
            transform.rotation = recRot;
            m_rigidBody.velocity = recVel;
            m_rigidBody.angularVelocity = recAngVel;
*/

            //smoothNetMovement();
            return;
        }

        if(photonView.isMine)
        {
            UpdateMeshesPositions();
            if(Input.GetKey("w"))
                accelerateState = true;
            else
                accelerateState = false;

            if(Input.GetKey(KeyCode.Space))
                BrakeState = true;
            else
                BrakeState = false;

            if(Input.GetKey("s"))
                HandBrakeState = true;
            else
                HandBrakeState = false;

            if(currSpeed == 0)
                idleState = true;
            else
                idleState = false;

            if(!Input.GetKey("w") && currSpeed != 0)
                DecelarateState = true;
            else
                DecelarateState = false;

            if (Input.GetKeyDown(KeyCode.Escape))
                isPause = !isPause;

            if( BrakeState && !idleState || BrakeState && accelerateState)
            {
                ps1.Play();
                ps2.Play();
                isSmoke = true;
            }

            else
            {
                ps1.Stop();
                ps2.Stop();
                isSmoke = false;
            }

            //photonView.RPC("SyncSmoke", PhotonTargets.Others, isSmoke);
            //photonView.RPC("SyncTransform", PhotonTargets.Others, transform.position, transform.rotation, m_rigidBody.velocity, m_rigidBody.angularVelocity);
        }    

        //photonView.RPC("Sync", PhotonTargets.All, transform.position, transform.rotation);
    }

    void Respawn()
    {
        transform.eulerAngles = new Vector3(0f,0f,0f);
        transform.position = new Vector3(0f,transform.position.y + 2f,0f);
        m_rigidBody.velocity = new Vector3();
        m_rigidBody.angularVelocity = new Vector3(); 
    }
 
    void FixedUpdate()
    {
        if(photonView.isMine)
        {
            if (isPause)
                pauseMenu.SetActive(true);
            else
                pauseMenu.SetActive(false);

            currSpeed = (int)GetComponent<Rigidbody>().velocity.magnitude;

            currSpeed = Mathf.Clamp(currSpeed,0,125);
            maxSteerAngle = (( (float)currSpeed / 125f ) * (3f - 20f)) + 20f;

            if(idleState)
                gear = 0;
            if(accelerateState)
                ChangeGearUp();
         
            if(BrakeState || DecelarateState || HandBrakeState) 
                ChangeGearDown();
            float steer = Input.GetAxis("Horizontal") * freeze;  //1
            float accelerate = Input.GetAxis("Vertical") * freeze;

            float finalAngle = steer * maxSteerAngle; //3
            wheelColliders[0].steerAngle = finalAngle;
            wheelColliders[1].steerAngle = finalAngle;
 
            for(int i = 0; i < 4; i++)
            {
                if(BrakeState)
                {
                    wheelColliders[i].motorTorque = 0f;
                    wheelColliders[i].brakeTorque = brakeStrength;
                    GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity * 0.998f;
                }
            
                else
                {
                    wheelColliders[i].motorTorque = accelerate * maxTorque;  //4
                    wheelColliders[i].brakeTorque = 0f;
                }

                if(accelerate == 0 && !BrakeState)
                {
                    wheelColliders[i].brakeTorque = Decelaration; //5
                }

                if(HandBrakeState && !idleState && !isReverse)
                {
                    //Debug.Log("handbrakes");
                    wheelColliders[i].brakeTorque = HandBrakeStrength;
                }
            }

            Direction = transform.position - prevPos;

            if(Vector3.Dot(transform.forward , Direction) < 0)
            {
                maxTorque = ReverseSpeed;
                if(currSpeed >= 18)
                {
                    //Debug.Log("clamp");
                    GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, 18);
                }
                isReverse = true;
            }

            else
            {
                maxTorque =  600f;
                isReverse = false;
            }    

            prevPos = transform.position;
            forward = transform.forward;
        }
    }

    public void FreezeAgent()
    {
        freeze = 0f;
    }

    public void ThawAgent()
    {
        freeze = 1f;
    }

    void UpdateMeshesPositions()
    {
         for(int i = 0; i < 4; i++)
         {
             Quaternion quat;
             Vector3 pos;
             wheelColliders[i].GetWorldPose(out pos, out quat);
 
             tireMeshes[i].position = pos;
             tireMeshes[i].rotation = quat;
         }
    }

    void ChangeGearUp()
    {
        if(currSpeed < 20 && currSpeed != 0)
            gear = 1;
        else if(currSpeed >= 20 && currSpeed < 40)
            gear = 2;
        else if(currSpeed >= 40 && currSpeed < 60)
            gear = 3;
        else if(currSpeed >= 60 && currSpeed < 80)
            gear = 4;
        else if(currSpeed >=80 && currSpeed < 120)
            gear = 5;
        else if(currSpeed >= 120)
            gear = 6;
    }
 
    void ChangeGearDown()
    {
        /*if(currSpeed < 20 && gear > 1 )
            gear = 1;
        else if(currSpeed < 40 && gear > 2)
            gear = 2;
        else if(currSpeed < 60 && gear > 3)
            gear = 3;
        else if(currSpeed < 80 && gear > 4)
            gear = 4;
        else if(currSpeed < 120 && gear > 5)
            gear = 5;*/
    
        if(currSpeed <= 10 && gear > 1 )
            gear = 1;
        else if(currSpeed < 30 && gear > 2)
            gear = 2;
        else if(currSpeed < 50 && gear > 3)
            gear = 3;
        else if(currSpeed < 70 && gear > 4)
            gear = 4;
        else if(currSpeed < 100 && gear > 5)
            gear = 5;
    }

    //public IEnumerator CarJerk()
    //{
        /*for(int i = 0; i < 4; i++)
        {                
            wheelColliders[i].motorTorque = 0f;
            wheelColliders[i].brakeTorque = brakeStrength;
            //GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity * 0.998f;   
        }

        yield return new WaitForSeconds(1f);

        /*for(int i = 0; i < 4; i++)
        {                
            wheelColliders[i].motorTorque = accelerate * maxTorque;
            wheelColliders[i].brakeTorque = 0f;
            //GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity * 0.998f;   
        }*/
    //}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            //stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
            //stream.SendNext(m_rigidBody.velocity);
            //stream.SendNext(m_rigidBody.angularVelocity);
            stream.SendNext(isSmoke);
            stream.SendNext(this.gameObject.name);
        }
        else
        {
            //transform.position = (Vector3)stream.ReceiveNext();
            //transform.rotation = (Quaternion)stream.ReceiveNext();
            //m_rigidBody.velocity = (Vector3)stream.ReceiveNext();
            //m_rigidBody.angularVelocity = (Vector3)stream.ReceiveNext();
            isSmoke = (bool)stream.ReceiveNext();
            this.gameObject.name = (string)stream.ReceiveNext();
        }
    }

    /*void OnSerializeNetworkView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 setPos = transform.position;
            Quaternion setRot = transform.rotation;
            stream.Serialize(ref setPos);
            stream.Serialize(ref setRot);
        }
        else
        {
            Vector3 newPos = Vector3.zero;
            Quaternion newRot = new Quaternion();
            stream.Serialize(ref newPos);
            stream.Serialize(ref newRot);
            transform.position = newPos;
            transform.rotation = newRot;
        }
    }*/

  /*  [PunRPC]
    void SyncSmoke(bool smoke)
    {
        isSmoke = smoke;
    }

    [PunRPC]
    void SyncTransform(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
    {
        recPos = pos;
        recRot = rot;
        recVel = vel;
        recAngVel = angVel;
    }*/
 }