using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMasher : MonoBehaviour
{
    public float mashDelay = .5f;
    public GameObject text;

    float mash;
    bool pressed;
    bool started;
    // Start is called before the first frame update
    void Start()
    {
        mash = mashDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            started = true;
            Debug.Log("Start");
        }
        if (started)
        {
            mash -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && !pressed)
            {
                pressed = true;
                mash = mashDelay;

            }
            else if(Input.GetKeyUp(KeyCode.Space))
            {
                pressed = false;
            }
            if(mash <= 0)
            {
                Debug.Log("You died");
                Destroy(this);
            }
        }
    }
}
