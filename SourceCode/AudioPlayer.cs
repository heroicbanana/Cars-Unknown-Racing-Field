using UnityEngine;
using System.Collections;
public class AudioPlayer : MonoBehaviour
{
    public PhotonView photonView;
    public AudioManager audioManager;
    public bool check;
    public bool isBrake = true;
    public bool isHandBrakes = true;
    bool isInitial = true;
    public int speed;
    public int gear;
    public int oldGear = 0;
    int flag = 0;
    public ParticleSystem ExhaustBurst;
    public bool isBurst = false;
    public CarController car;
    public Quaternion offset;

    void Start()
    {
        if(!photonView.isMine)
            return;
        audioManager.Play("Idle");
        speed = (int)GetComponent<Rigidbody>().velocity.magnitude;
        car = GetComponent<CarController>();
    }

    void FixedUpdate()
    {
        if(!photonView.isMine)
        {
            if(isBurst)
                ExhaustBurst.Play();
            else
                ExhaustBurst.Stop();
            return;
        }

        #region Acceleration
        gear = GetComponent<CarController>().gear;
        int OriginalSpeed = (int)GetComponent<Rigidbody>().velocity.magnitude;
        if(!car.idleState && check == true )
        {
            audioManager.Play("start");
            check = false;
        }

        else if(!car.idleState && check == false)
        {
            audioManager.ChangePitch("start",true,OriginalSpeed);
        }

        else if(car.idleState || GetComponent<Rigidbody>().velocity.z < 0)
        {
            audioManager.Stop("start");
            check = true;
        }

        #endregion

        #region GearSound
        if(gear > oldGear)
        {
            audioManager.ChangeOffset(OriginalSpeed);
            if(gear <= 4)
            {
                audioManager.Play("gear");
                ExhaustBurst.Play();
                isBurst = true;
            }
            //oldGear = gear;
        }

        else if(gear < oldGear)
        {
            audioManager.ChangeOffsetDown(OriginalSpeed);
            /*if(gear <= 2)
            {
                audioManager.Play("gear");
                ExhaustBurst.Play();
            }*/
            //oldGear = gear;
        }

        else if(gear == oldGear)
        {
            ExhaustBurst.Stop();
            isBurst = false;
        }

        oldGear = gear;
        //photonView.RPC("SyncExhaust", PhotonTargets.Others, isBurst);

        #endregion

        #region  Brakes
        if(car.BrakeState && isBrake)
        {
            audioManager.Play("Brakes");
            isBrake = false;
        }

        else if(!car.BrakeState)
        {
            audioManager.Stop("Brakes");
            isBrake = true;
        }

        if(car.idleState)
        {
            audioManager.Stop("Brakes");
            isBrake = true;
        }
        #endregion

         #region  HandBrakes
        if(Input.GetKey("s") && isHandBrakes)
        {
            audioManager.Play("HandBrakes");
            isHandBrakes = false;
        }

        else if(!Input.GetKey("s"))
        {
            audioManager.Stop("HandBrakes");
            isHandBrakes = true;
        }

        if(OriginalSpeed == 0)
        {
            audioManager.Stop("HandBrakes");
            isHandBrakes = true;
        }

        if(GetComponent<CarController>().isReverse)
        {
            audioManager.Stop("HandBrakes");
            isHandBrakes = true;
        }
        #endregion
        
        #region initial
        if(car.BrakeState && car.accelerateState && car.idleState && isInitial)
        {
           audioManager.Play("initial");
           isInitial = false;
        }

        else if(!(car.BrakeState && car.accelerateState && car.idleState))
        {
            //audioManager.Stop("initial");
            audioManager.ChangePitchDown("initial");
            float _pitch = audioManager.getPitch("initial");
            if(_pitch <= 0)
            {
                audioManager.Stop("initial");
            }
            isInitial = true;
        }

        if(isInitial == false)
        {
           audioManager.ChangePitch("initial",OriginalSpeed);
        }
        #endregion
    
        //if(gear > oldGear)
        {
            //int _speed = GetComponent<CarController>().currSpeed;
            //GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, _speed);
            //gameObject.transform.rotation += offset;
            //StartCoroutine(car.CarJerk());
            //oldGear = gear;
        }
    }

    void LateUpdate()
    {
        //oldGear = gear;
    }

    /*IEnumerator CarJerk()
    {
        int _speed = GetComponent<CarController>().currSpeed;
        GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, _speed);

        yield return new WaitForSeconds(1);
        //Debug.Log("waitover");
    }8*/

    /*[PunRPC]
    void SyncExhaust(bool burst)
    {
        isBurst = burst;
    }*/

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(isBurst);
        }
        else
        {
            isBurst = (bool)stream.ReceiveNext();        
        }
    }
}