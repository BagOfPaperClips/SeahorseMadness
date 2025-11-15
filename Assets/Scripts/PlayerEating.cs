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
    //how long they have at full vignette
    private float timeRemaining = 10f;
    public bool timerIsRunning = false;
    //final death
    private bool isDead=false;

    //scripts
    public SceneTransitioner sceneTransitioner;
    public Movement movement;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        //volume = PPGO.GetComponent<Volume>();

        UnityEngine.Rendering.VolumeProfile volumeProfile = PPGO.GetComponent<UnityEngine.Rendering.Volume>()?.profile;

        volumeProfile.TryGet<Vignette>(out vignette);
        volumeProfile.TryGet<Bloom>(out bloom);
        sceneTransitioner = GameObject.Find("LevelLoader").GetComponent<SceneTransitioner>();
        movement = GetComponent<Movement>();

        if (!vignette)
        {
            print("error, vignette empty");
        }

        if (!bloom)
        {
            print("error, bloom empty");
        }

        if (!sceneTransitioner)
        {
            print("error, sceneTransitioner empty");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _elapsed += Time.deltaTime;
        if (_isEating == false)
        {
            intensity = 0.03f * _elapsed;
        }
        else if (isDead)
        {
            intensity = 0.1f * _elapsed;
        }
        else
        {
            intensity -= (0.02f * _elapsed);
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
            if (timerIsRunning == false) StartCoroutine(movement.PlayStruggle());
            timerIsRunning = true;
            //Debug.Log("Timer is running");
        }
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                if (movement._struggling == false)
                {
                    movement._struggling = true;
                    StartCoroutine(movement.PlayStruggle());
                }
                //Debug.Log(timeRemaining);
                //switch to red at specific times
                if (timeRemaining > 5f && timeRemaining < 6f || timeRemaining > 3f && timeRemaining < 4f || timeRemaining > 1f && timeRemaining < 2f)
                {
                    vignette.color.Override(new Color(250, 0, 0));
                }
                else
                {
                    vignette.color.Override(new Color(0, 0, 0));
                }
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out! You died of starvation");
                timeRemaining = 0;
                timerIsRunning = false;
                if (isDead)
                {
                    sceneTransitioner.FinalDeath();
                }
                else
                {
                    sceneTransitioner.Death();
                }
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
            _elapsed = 0f;
            //intensity = 0f;
            //vignette.intensity.Override(0f);
            eatSound.Play();
            timerIsRunning = false;
            timeRemaining = 10f;
            _isEating = true;
            movement._struggling = false;
            vignette.color.Override(new Color(0, 0, 0));
        }
        if (collision.CompareTag("FinalDeath"))
        {
            Debug.Log("Final Death");
            timeRemaining = 0f;
            isDead =true;
        }
    }
}
