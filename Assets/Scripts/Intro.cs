using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class Intro : MonoBehaviour
{

    public float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene("UI");
    }
}
