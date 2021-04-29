using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class countDown : MonoBehaviour
{
    public Text countDownText;

    public IEnumerator StartCountDown(GameObject car)
    {
        countDownText.text = "3";
        yield return new WaitForSeconds(1f);
        countDownText.text = string.Empty;
        yield return new WaitForSeconds(0.5f);
        
        countDownText.text = "2";
        yield return new WaitForSeconds(1f);
        countDownText.text = string.Empty;
        yield return new WaitForSeconds(0.5f);

        countDownText.text = "1";
        yield return new WaitForSeconds(1f);
        countDownText.text = string.Empty;
        yield return new WaitForSeconds(0.5f);

        countDownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countDownText.text = string.Empty;
        //Debug.Log("countDownFinished");

        CarController cc = car.GetComponent<CarController>();//.ThawAgent();
        coupeController CC = car.GetComponent<coupeController>();

        if (cc != null)
            cc.ThawAgent();
        else if (CC != null)
            CC.ThawAgent();
    }
}
