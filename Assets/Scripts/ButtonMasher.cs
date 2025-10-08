using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMasher : MonoBehaviour
{
    public float mashDelay = .5f;
    //public GameObject text;

    float mash;
    bool pressed;
    bool started;
    public BodySourceView BSV;

    bool countdownActive = false;
    float countdownTimer = 4;
    float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        BSV = GetComponent<BodySourceView>();
        mash = mashDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (BSV.struggleAmount)
        {


            if (Input.GetKeyUp(KeyCode.Space))
            {
                started = true;
                Debug.Log("Start");
            }
            if (started)
            {
                elapsedTime += Time.deltaTime;

                mash -= Time.deltaTime;

                if (countdownTimer > 1)
                {
                    countdownTimer -= Time.deltaTime;
                }

                if (Input.GetKeyDown(KeyCode.Space) && !pressed)
                {
                    pressed = true;
                    mash = mashDelay;

                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    pressed = false;
                }
                if (mash <= 0)
                {

                    //FAIL STATE

                    Debug.Log("You died");

                }
                if (countdownTimer < 1)
                {
                    Debug.Log("GOTOUT");
                    BSV.struggleAmount = false;
                    started = false;
                    mash = mashDelay;
                    countdownTimer = 4;
                    elapsedTime = 0;
                }
            }
        }

    }
}
