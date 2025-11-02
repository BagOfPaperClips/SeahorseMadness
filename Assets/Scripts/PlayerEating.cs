using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//ausing UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class PlayerEating : MonoBehaviour
{
    //post processing game object
    public GameObject PPGO;
    public float intensity = 0;

    UnityEngine.Rendering.Universal.Vignette vignette;
    UnityEngine.Rendering.Universal.Bloom bloom;

    //sound
    public AudioSource eatSound;
    public AudioSource full;

    //endscreen countdown (hidden to players)
    private bool _isEating = false;
    private float _elapsed = 0f;
    public float timeRemaining = 5;
    public bool timerIsRunning = false;

    // Start is called before the first frame update
    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }


    void Start()
    {
        //volume = PPGO.GetComponent<Volume>();

        UnityEngine.Rendering.VolumeProfile volumeProfile = PPGO.GetComponent<UnityEngine.Rendering.Volume>()?.profile;

        volumeProfile.TryGet<Vignette>(out vignette);
        volumeProfile.TryGet<Bloom>(out bloom);

        if (!vignette)
        {
            print("error, vignette empty");
        }

        if (!bloom)
        {
            print("error, bloom empty");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _elapsed += Time.deltaTime;
        if (_isEating == false)
        {
            intensity = 0.05f * _elapsed;
        }
        else
        {
            intensity = 1 - (0.35f * _elapsed);
        }
        //max vignette sizes
        if (intensity > 1f) intensity = 1f;
        if (intensity < 0f)
        {
            _isEating = false;
            full.Play();
            intensity = 0f;
        }
        //change vignette and bloom
        vignette.intensity.Override(intensity);
        bloom.intensity.Override(intensity*20);

        if (intensity == 1f) {
            timerIsRunning = true;
            Debug.Log("Timer is running");
        }
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out! You died of starvation");
                timeRemaining = 0;
                timerIsRunning = false;
                SceneManager.LoadScene("DeathScreen");
            }
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    intensity = 0f;
        //    vignette.intensity.Override(0f);
        //}
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Food"))
        {
            Debug.Log("Food collected: ");
            //intensity = 0f;
            //vignette.intensity.Override(0f);
            eatSound.Play();
            timerIsRunning = false;
            timeRemaining = 5;
            _isEating = true;
        }
    }
}
