using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCar : MonoBehaviour
{
    public GameObject car;
    public float stickiness = 5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,car.transform.position,stickiness * Time.deltaTime);
        transform.LookAt(car.transform);
    }
}
