using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

using Windows.Kinect;
using Joint = Windows.Kinect.Joint;
using Unity.VisualScripting;

public class BodySourceView : MonoBehaviour
{
    public CharacterController controller;

    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;

    public bool struggleAmount;
    
    //movement
    public float sspeed = .1f;
    public float tspeed = .1f;

    public bool isKinect = false;

    public float xDir = 0;
    public float zDir = 0;

    //movement sounds
    public UnityEngine.AudioSource moveSound;
    private bool _moving;

    //struggle & sounds
    public float movevar = 3f;
    public UnityEngine.AudioSource struggleButton;
    //struggle coroutine from buttonmasher use
    public ButtonMasher buttonMasher;

    public Transform cam;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    private float pastdir = 0;

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
        //JointType.HandLeft, ///CHANGED
        //JointType.HandRight, ///CHANGED

        JointType.SpineBase,
        JointType.Head,
    };

    void Start()
    {
        buttonMasher = GetComponent<ButtonMasher>();
    }
    
    void Update()
    {
        #region Get Kinect data
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
            return;

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
                continue;

            if (body.IsTracked)
                trackedIds.Add(body.TrackingId);
        }
        #endregion

        #region Delete Kinect bodies
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                // Destroy body object
                Destroy(mBodies[trackingId]);

                // Remove from list
                mBodies.Remove(trackingId);
            }
        }
        #endregion

        #region Create Kinect bodies
        foreach (var body in data)
        {
            // If no body, skip
            if (body == null)
                continue;

            if (body.IsTracked)
            {
                // If body isn't tracked, create body
                if (!mBodies.ContainsKey(body.TrackingId))
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);

                // Update positions
                UpdateBodyObject(body, mBodies[body.TrackingId]);

                isKinect = true;
                break;
            }
        }
        #endregion
    }

    private GameObject CreateBodyObject(ulong id)
    {
        // Create body parent
        GameObject body = new GameObject("Body:" + id);

        // Create joints
        foreach (JointType joint in _joints)
        {
            // Create Object
            GameObject newJoint = Instantiate(mJointObject);
            newJoint.name = joint.ToString();

            // Parent to body
            newJoint.transform.parent = body.transform;
        }

        return body;
    }

    private void UpdateBodyObject(Body body, GameObject bodyObject)
    {
        // Update joints
        foreach (JointType _joint in _joints)
        {
            // Get new target position
            Joint sourceJoint = body.Joints[_joint];

            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);

            if (struggleAmount)
            {
                Struggle(sourceJoint);
            }
            else
            {
                Movement(sourceJoint);
            }
                
            targetPosition.z = 0;

            // Get joint, set new position
            Transform jointObject = bodyObject.transform.Find(_joint.ToString());
            jointObject.position = targetPosition;
        }
    }

    public Vector3 GetVector3FromJoint(Joint joint)
    {
        //Debug.Log(new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10));
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }


    public void Movement(Joint joint)
    {
        Debug.Log("Inside");
        float tempx = joint.Position.X * 10;
        float tempy = joint.Position.Y * 10;
        float tempz = joint.Position.Z * 10;

        xDir = 0;
        zDir = 0;

        //-----------------------------------\\
        //REGULAR CONTROLS

        if (tempx <= -1)
        {
            Debug.Log("LEFT");
            xDir = -1;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, xDir*90, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation= Quaternion.Euler(0f,angle,0f);
            //move.x -= sspeed *Time.deltaTime;
            if (_moving == false)
            {
                moveSound.Play();
                _moving = true;
            }
        }
        else if(tempx >= 1)
        {
            Debug.Log("RIGHT");
            xDir = 1;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, xDir * 90, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //move.x += sspeed * Time.deltaTime;
            if (_moving == false)
            {
                moveSound.Play();
                _moving = true;
            }
        }
        else if(tempz <= 14)
        {
            Debug.Log("FORWARD");
            zDir = 1;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, zDir * 0, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //move.z += sspeed * Time.deltaTime;
            if (_moving == false)
            {
                moveSound.Play();
                _moving = true;
            }
        }
        else if(tempz >= 16)
        {
            Debug.Log("BACKWARD");
            zDir = -1;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, zDir * 180, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //move.z -= sspeed * Time.deltaTime;
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



            //-----------------------------------\\
            //TANK CONTROLS
            /*
            float value = 0;
            if (tempx <= -2)
            {
                Debug.Log("LEFT");
                xDir = -1;
                seahorse.transform.Rotate(0.0f, xDir * tspeed, 0.0f);
                value = seahorse.transform.rotation.y;
                //move.x -= sspeed *Time.deltaTime;
            }
            else if (tempx >= 2)
            {
                Debug.Log("RIGHT");
                xDir = 1;
                seahorse.transform.Rotate(0.0f, xDir * tspeed,0.0f);
                value = seahorse.transform.rotation.y;
                //move.x += sspeed * Time.deltaTime;
            }
            if (tempz <= 13)
            {
                Debug.Log("FORWARD");
                zDir = 1;
                seahorse.transform.Rotate(0.0f, 0.0f, 0.0f);
                value = seahorse.transform.rotation.y;
                //move.z += sspeed * Time.deltaTime;
            }
            else if (tempz >= 18)
            {
                Debug.Log("BACKWARD");
                zDir = -1;
                seahorse.transform.Rotate(0.0f, 0.0f, 0.0f);
                value = seahorse.transform.rotation.y;
                //move.z -= sspeed * Time.deltaTime;
            */
            //this.transform.position = move;

            //Regular Controls

            Vector3 v = new Vector3(xDir, 0.0f, zDir);

        //Tank Controls
        //Vector3 v = new Vector3(0f, 0f, value);
        
        //controller.Move(v * sspeed * Time.deltaTime);

    }

    public void Struggle(Joint joint)
    {
        float tempx = joint.Position.X * 10;
        float tempy = joint.Position.Y * 10;
        float tempz = joint.Position.Z * 10;

        float strugglespeed = 100f;

        Debug.Log("STRUGGLE");

        if (tempx <= -1)
        {
            Debug.Log("LEFT");
            if (pastdir != 1 && buttonMasher._struggling == false)
            {
                StartCoroutine(buttonMasher.PlayStruggleButton());
                transform.position += new Vector3(0, 0, movevar);
            }
            pastdir = 1;

        }
        else if (tempx >= 1)
        {
            Debug.Log("RIGHT");
            if (pastdir != 2 && buttonMasher._struggling==false)
            {
                StartCoroutine(buttonMasher.PlayStruggleButton());
                transform.position += new Vector3(0, 0, movevar);
            }
            pastdir = 2;
            
        }
        /*
        else if (tempz <= 13)
        {
            Debug.Log("FORWARD");
            if (pastdir != 3)
            {
                Debug.Log("forwardMoving");
                transform.position += new Vector3(0, 0, movevar);
            }
            pastdir = 3;
            
        }
        else if (tempz >= 18)
        {
            Debug.Log("BACKWARD");
            if (pastdir != 4)
            {
                transform.position += new Vector3(0, 0, movevar);
            }
            pastdir = 4;
            
        }
        */

        Debug.Log(pastdir);
    }
}
