using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 3f;

    public void TitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void StartGame()
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }

    public void Death()
    {
        StartCoroutine(LoadLevel("DeathScreen"));
    }

    public void FinalDeath()
    {
        SceneManager.LoadScene("FinalDeathScreen");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void Tutorial()
    {
        StartCoroutine(LoadLevel("TutorialScene"));
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator LoadLevel(string levelIndex)
    {
        //Play animation
        transition.SetTrigger("Start");

        //wait
        yield return new WaitForSeconds(transitionTime);

        //loadscene
        SceneManager.LoadScene(levelIndex);
    }
}
