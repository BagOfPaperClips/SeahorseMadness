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
    public AudioSource ambiance;
    public AudioSource[] sources;
    private int clipIndex;
    private int lastClipIndex;
    private AudioSource audio;
    private bool audioPlaying = false;

    public float tbplaymin;
    public float tbplaymax;

    void Start()
    {
        BSV = GetComponent<BodySourceView>();
        Debug.Log(BSV);

        //sound
        StartCoroutine(PlaySound());
        ambiance.Play();
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

            Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //move in the direction you are pressing + the cam.eulerAngles.y which connects to angle of camera
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //Smooth rotations
                transform.rotation = Quaternion.Euler(0f, angle, 0f); //rotation

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir * speed * Time.deltaTime);
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
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("StruggleArea"))
        {
            Debug.Log("INSTRUGGLE");
            Debug.Log(BSV);
            BSV.struggleAmount = true;
            

        }
        else
        {
            BSV.struggleAmount = false;
        }
        
    }

    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(Random.Range(tbplaymin, tbplaymax));
        lastClipIndex = clipIndex;
        while (clipIndex == lastClipIndex)
        {
            clipIndex = Random.Range(0, sources.Length);
            Debug.Log(clipIndex +" "+ lastClipIndex);
        }
        Debug.Log("Playing clip "+clipIndex);
        sources[clipIndex].Play();
        yield return new WaitForSeconds(Random.Range(tbplaymin, tbplaymax));
        StartCoroutine(PlaySound());
    }
}
