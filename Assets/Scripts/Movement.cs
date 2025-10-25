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

    //movement sounds
    public AudioSource moveSound;
    private bool _moving;

    //random environment sounds
    public AudioSource ambiance1;
    public AudioSource ambiance2;
    private bool _ambiance2;

    //creaks
    public AudioSource[] sources;
    private int clipIndex;
    private int lastClipIndex;
    private AudioSource audio;
    private bool audioPlaying = false;

    private float tbplaymin;
    private float tbplaymax;
    private float tbplayadd;
    private float tbplay1;
    private float tbplay2;

    //struggle sounds
    public AudioSource[] struggles;
    private bool _struggling=false;
    public AudioSource struggleEnd;

    void Start()
    {
        BSV = GetComponent<BodySourceView>();
        Debug.Log(BSV);

        //sound
        StartCoroutine(PlaySound());
        ambiance1.Play();
        tbplaymin = 8;
        tbplaymax = 15;
    }

    // Update is called once per frame
    void Update()
    {
        if (BSV.struggleAmount == false)
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
                if (_moving == false)
                {
                    moveSound.Play();
                    _moving = true;
                }
            }
            else
            {
                moveSound.Pause();
                _moving = false;
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
            if (BSV.struggleAmount == true) struggleEnd.Play();
            BSV.struggleAmount = false;


        }
        if (collision.CompareTag("AudioZone1"))
        {
            if (_ambiance2 == false)
            {
                ambiance1.Pause();
                ambiance2.Play();
                _ambiance2= true;
                Debug.Log("Ambiance area 2");
                tbplaymin = 10;
                tbplaymax = 30;
                StartCoroutine(PlaySound());
            }
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

    IEnumerator PlayStruggle()
    {
        yield return new WaitForSeconds(Random.Range(1, 4));
        clipIndex = Random.Range(0, struggles.Length);
        Debug.Log("Struggle " + clipIndex);
        struggles[clipIndex].Play();
        yield return new WaitForSeconds(Random.Range(1, 4));
        _struggling = false;
    }
}
