using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotateSpeed;
        
    void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime , Space.Self);
    }
}
