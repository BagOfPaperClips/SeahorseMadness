using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public void TitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Death()
    {
        SceneManager.LoadScene("DeathScreen");
    }

    public void FinalDeath()
    {
        SceneManager.LoadScene("FinalDeathScreen");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
