using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Hand : MonoBehaviour
{
    public Transform mHandMesh;
    
    private void Update()
    {
        mHandMesh.position = Vector3.Lerp(mHandMesh.position, transform.position, Time.deltaTime * 15.0f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Start"))
        {
            Debug.Log("Play Game");

            
            SceneManager.LoadScene("SampleScene");
    
        }

        if (collision.CompareTag("Credits"))
        {
            Debug.Log("CreditPage");


            SceneManager.LoadScene("Credits");

        }

        if (collision.CompareTag("quit"))
        {
            Debug.Log("QuitePage");


            Application.Quit();
        }


        if (collision.CompareTag("BACK"))
        {
            Debug.Log("TitlePage");


            SceneManager.LoadScene("TitleScreen");
        }

    }
}
