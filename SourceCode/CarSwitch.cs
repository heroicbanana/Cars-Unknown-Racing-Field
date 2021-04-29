using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSwitch : MonoBehaviour
{
    public GameObject car;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("e"))
        {
            car.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
