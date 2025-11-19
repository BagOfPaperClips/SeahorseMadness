using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;
using TMPro;
using UnityEngine.SceneManagement;
//using System.Diagnostics.Tracing;
//using static Unity.Burst.Intrinsics.Arm;
using UnityEngine.Rendering.PostProcessing;
using System.Linq.Expressions;

public class BodySourceViewForTitleNav : MonoBehaviour
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;

    [SerializeField] TextMeshProUGUI DirectionPointing;
    [SerializeField] TextMeshProUGUI Instructions;
    [SerializeField] TextMeshProUGUI Calibration;

    public TextMeshProUGUI Timer;
    [SerializeField] float remainingTime = 5;
    [SerializeField] int seconds;

    public bool posDone = false;

    [SerializeField] int counterL = 0;
    [SerializeField] int counterR = 0;
    [SerializeField] int counterF = 0;
    [SerializeField] int counterB = 0;

    private float pastdir = 0;

    int counter = 0;
    int totalCount = 0;
    int prev = 0;
    int struggleCounter = 0;
    //bool calTime = false;
    bool movementTime = true;

    bool instruction = false; 

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
       
        //JointType.HandLeft,
        //JointType.HandRight,
        //JointType.SpineBase,
        JointType.Head
    };

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
            targetPosition.z = 0;

            if (posDone == true)
            {
                MovementCal(sourceJoint);
            }
            else if(movementTime)
            {
                setupPos(sourceJoint);
            }
            else //if (calTime)
            {
                StruggleMech(sourceJoint);
            }


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

    public void setupPos(Joint joint)
    {
        Instructions.text = "Stay perfect for 5 seconds \nMovement is tracked by head";
        Calibration.text = "";

        //----- POSITIONS  ------//
        float tempx = joint.Position.X * 10;
        float tempy = joint.Position.Y * 10;
        float tempz = joint.Position.Z * 10;
        

        if (tempx <= -1)
        {
            DirectionPointing.text = "Step Right";
            remainingTime = 5;
        }
        else if (tempx >= 1)
        {
            DirectionPointing.text = "Step Left";
            remainingTime = 5;
        }
        else if (tempz <= 14)
        {
            DirectionPointing.text = "Step Backward";
            remainingTime = 5;
        }
        else if (tempz >= 16)
        {
            DirectionPointing.text = "Step Forward";
            remainingTime = 5;
        }
        else
        {
            DirectionPointing.text = "Perfect";
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
            }
        }

        seconds = Mathf.FloorToInt(remainingTime % 60);

        Timer.text = string.Format("{0:00}", seconds);

        //----- GET OUT ------//
        if (remainingTime <= 0)
        {
            Timer.text = "";
            posDone = true;
            prev = 0;
            movementTime = false;
        }
        
    }
    public void MovementCal(Joint joint)
    {
        if (prev == 0)
        {
            Instructions.text = "Test your Leaning \n Lean forward";
        }

        //----- POSITIONS  ------//
        float tempx = joint.Position.X * 10;
        float tempy = joint.Position.Y * 10;
        float tempz = joint.Position.Z * 10;

        //------ LEFT -------//
        if (tempx <= -1)
        {
            if (prev != 1)
            {
                //counterL = 1;
                
                
                if(counter == 3)
                {
                    Instructions.text = "Perfect! Do it again! \nLean Forward";
                    //counter = counter + 1;
                    counter = 0;
                    totalCount = totalCount + 1;
                    
                }
                
                prev = 1;
            }
            DirectionPointing.text = "Left ";

        }
        //------ RIGHT -------//
        else if (tempx >= 1)
        {
            if (prev != -1)
            {
                counterR = counterR + 1;
                
                
                if (counter == 2)
                {
                    Instructions.text = "Lean Left";
                    counter = counter + 1;
                }

                prev = -1;
            }
            DirectionPointing.text = "Right";

        }
        //------ FORWARD  -------//
        else if (tempz <= 14)
        {
            if (prev != 2)
            {
                counterF = counterF + 1;
                

                if (counter == 0)
                {
                    Instructions.text = "Lean Backward";
                    counter = counter + 1;
                }

                prev = 2;
            }
            DirectionPointing.text = "Forward ";

        }
        //------ BACKWARD -------//
        else if (tempz >= 16)
        {
            if (prev != -2)
            {
                counterB = counterB + 1;
                

                if (counter == 1)
                {
                    counter = counter + 1;
                    Instructions.text = "Lean Right";
                    
                }
                
                prev = -2;
            }
            DirectionPointing.text = "Backward ";

        }
        else
        {
            DirectionPointing.text = "Center ";
        }


        //----- GET OUT ------//
        if (totalCount == 2)
        {
            //Instructions.text = "";
            //SceneManager.LoadScene("SampleScene");
            //calTime = true;
            prev = 0;
            posDone = false;

        }

    }
    public void StruggleMech(Joint joint) 
    {
        Debug.Log("InsideStruggle");
        if (instruction ==false)
        {
            Instructions.text = "Struggle Time : Move Right";
        }

        //----- POSITIONS  ------//
        float tempx = joint.Position.X * 10;
        float tempy = joint.Position.Y * 10;
        float tempz = joint.Position.Z * 10;

        if (tempx <= -1)
        {
            Debug.Log("LEFT");
            if (pastdir != 1)
            {
                
                Instructions.text = "Move Right";
                struggleCounter = struggleCounter + 1;
            }
            pastdir = 1;
            DirectionPointing.text = "Left ";

        }
        else if (tempx >= 1)
        {
            Debug.Log("RIGHT");
            if (pastdir != 2)
            {
                Instructions.text = "Move Left";
                struggleCounter = struggleCounter + 1;
            }
            pastdir = 2;
            DirectionPointing.text = "Right";
        }
        else
        {
            instruction = true;
        }

        if (struggleCounter == 6)
        {
            Instructions.text = "Move To Center";
            if(!(tempz >= 16) && !(tempz <= 14) && !(tempx >= 1) && !(tempx <= -1))
            {
                DirectionPointing.text = "Center";
                struggleCounter = struggleCounter + 1;
                if (struggleCounter == 7)
                {
                    SceneManager.LoadScene("SampleScene");
                }
            }
        }
    }
}
