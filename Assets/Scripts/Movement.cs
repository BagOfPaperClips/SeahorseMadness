using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller;

    public Transform cam;
    public float speed = .1f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public BodySourceView BSV;
    public PlayerEating PE;

    #region SoundInfo
    //movement sounds
    public AudioSource moveSound;
    private float maxMove;

    //random environment sounds
    public AudioSource ambiance1;
    public AudioSource ambiance2;
    private bool _ambiance1;
    private bool _ambiance2;

    //creaks
    public AudioSource[] sources;
    private int clipIndex;
    private int lastClipIndex;
    //private AudioSource audio;
    //private bool audioPlaying = false;

    private float tbplaymin;
    private float tbplaymax;
    private float tbplayadd;
    private float tbplay1;
    private float tbplay2;

    //struggle sounds
    public AudioSource[] struggles;
    public bool _struggling=false;
    public AudioSource struggleEnd;
    #endregion

    void Start()
    {
        BSV = GetComponent<BodySourceView>();
        PE = GetComponent<PlayerEating>();
        Debug.Log(BSV);

        //sound
        StartCoroutine(PlaySound());
        ambiance1.Play();
        tbplaymin = 5;
        tbplaymax = 8;
        moveSound.volume = 0;
        maxMove = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (BSV.struggleAmount == false && PE.starved == false)
        {
            float xDirection = Input.GetAxis("Horizontal");
            float zDirection = Input.GetAxis("Vertical");

            if (BSV.isKinect)
            {
                xDirection = BSV.xDir;
                zDirection = BSV.zDir;
            }

            Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //move in the direction you are pressing + the cam.eulerAngles.y which connects to angle of camera
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //Smooth rotations
                transform.rotation = Quaternion.Euler(0f, angle, 0f); //rotation

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir* speed * Time.deltaTime);
                //transform.position += moveDir.normalized * speed; //movement
                moveSound.volume += 0.01f;
                if (moveSound.volume > maxMove) moveSound.volume = maxMove;
            }
            else
            {
                moveSound.volume -= 0.08f;
                if (moveSound.volume < 0f) moveSound.volume = 0f;
            }
        }
        else
        {
            if (_struggling == false)
            {
                StartCoroutine(PlayStruggle());
                _struggling = true;
            }
        }
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("StruggleArea"))
        {
            Debug.Log("INSTRUGGLE");
            Debug.Log(BSV);
            BSV.struggleAmount = true;
            StartCoroutine(PlayStruggle());
        }
        else
        {
            if (BSV.struggleAmount == true)
            {
                struggleEnd.Play();
            }
            BSV.struggleAmount = false;
        }
        if (collision.CompareTag("AudioZone1"))
        {
            if (_ambiance1 == false)
            {
                _ambiance1 = true;
                Debug.Log("Ambiance area 2");
                tbplaymin = 15;
                tbplaymax = 20;
                maxMove = 0.7f;
                StartCoroutine(PlaySound());
                for (int i = 0; i < sources.Length; i++)
                {
                    sources[i].volume -= 0.05f;
                }
            }
        }
        if (collision.CompareTag("AudioZone2"))
        {
            if (_ambiance2 == false)
            {
                ambiance1.Pause();
                ambiance2.Play();
                _ambiance2 = true;
                Debug.Log("Ambiance area 3");
                tbplaymin = 20;
                tbplaymax = 30;
                maxMove = 0.4f;
                StartCoroutine(PlaySound());
                for (int i = 0; i < sources.Length; i++)
                {
                    sources[i].volume -= 0.1f;
                }
            }
        }
        if (collision.CompareTag("FinalDeath"))
        {
            Debug.Log("DEAD");
            Debug.Log(BSV);
            BSV.struggleAmount = true;
            StartCoroutine(PlayStruggle());
            BSV.movevar = 0.01f;
        }
    }

    IEnumerator PlaySound()
    {
        tbplay1 = Random.Range(tbplaymin, tbplaymax);
        Debug.Log(tbplay1);
        yield return new WaitForSeconds(tbplay1);
        lastClipIndex = clipIndex;
        while (clipIndex == lastClipIndex)
        {
            clipIndex = Random.Range(0, sources.Length);
            Debug.Log(clipIndex +" "+ lastClipIndex);
        }
        Debug.Log("Playing clip "+clipIndex);
        sources[clipIndex].Play();
        tbplay2 = Random.Range(tbplaymin, tbplaymax);
        Debug.Log(tbplay2);
        yield return new WaitForSeconds(tbplay2);
        StartCoroutine(PlaySound());
    }

    public IEnumerator PlayStruggle()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        clipIndex = Random.Range(0, struggles.Length);
        Debug.Log("Struggle " + clipIndex);
        struggles[clipIndex].Play();
        yield return new WaitForSeconds(Random.Range(2, 3));
        _struggling = false;
    }
}
