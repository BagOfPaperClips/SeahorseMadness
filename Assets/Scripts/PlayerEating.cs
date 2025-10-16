using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class PlayerEating : MonoBehaviour
{
    //post processing game object
    public GameObject PPGO;
    public float intensity = 0;

    PostProcessVolume _volume;
    Vignette _vignette;
    Bloom _bloom;

    //sound
    public AudioSource eatSound;
    public AudioSource full;

    //endscreen countdown (hidden to players)
    private bool _isEating = false;
    public float timeRemaining = 4;
    public bool timerIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        _volume = PPGO.GetComponent<PostProcessVolume>();

        _volume.profile.TryGetSettings<Vignette>(out _vignette);
        _volume.profile.TryGetSettings<Bloom>(out _bloom);

        if (!_vignette)
        {
            print("error, vignette empty");
        }
        else
        {
            _vignette.enabled.Override(true);
        }

        if (!_bloom)
        {
            print("error, bloom empty");
        }
        else
        {
            _bloom.enabled.Override(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEating == false)
        {
            intensity += 0.0001f;
        }
        else
        {
            intensity -= 0.005f;
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
        _vignette.intensity.Override(intensity);
        _bloom.intensity.Override(intensity*20);

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
                //go to endscreen
            }
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    intensity = 0f;
        //    _vignette.intensity.Override(0f);
        //}
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Food"))
        {
            Debug.Log("Food collected: ");
            //intensity = 0f;
            //_vignette.intensity.Override(0f);
            eatSound.Play();
            timerIsRunning = false;
            timeRemaining = 5;
            _isEating = true;
        }
    }
}
