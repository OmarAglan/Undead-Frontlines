using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class LightFlicker : MonoBehaviour
{
    public bool isFlickring = false;
    public float timeDelay;
    

    void Update()
    {
        if (isFlickring == false)
        {
            StartCoroutine(FlickringLight());
        
        }
    }

    IEnumerator FlickringLight()
    {
        isFlickring = true;
        this.gameObject.GetComponent<Light>().enabled = false;
        timeDelay = Random.Range(0.05f, 0.02f);
        yield return new WaitForSeconds(timeDelay);
        this.gameObject.GetComponent<Light>().enabled = true;
        timeDelay = Random.Range(1f, 0.08f);
        yield return new WaitForSeconds(timeDelay);
        isFlickring = false;
    }
}
