using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;
using TMPro;
using UnityEngine.SceneManagement;
using System.Diagnostics.Tracing;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class BodySourceViewForTitleNav : MonoBehaviour
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;

    [SerializeField] TextMeshProUGUI DirectionPointing;
    [SerializeField] TextMeshProUGUI Instructions;
    public TextMeshProUGUI Timer;
    [SerializeField] float remainingTime = 5;
    [SerializeField] int seconds;

    public bool posDone = false;

    [SerializeField] int counterL = 0;
    [SerializeField] int counterR = 0;
    [SerializeField] int counterF = 0;
    [SerializeField] int counterB = 0;
    int prev = 0;

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
       
        //JointType.HandLeft,
        //JointType.HandRight,
        //JointType.SpineBase,
        JointType.Head,
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
            else
            {
                setupPos(sourceJoint);
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
        //----- POSITIONS  ------//
        float tempx = joint.Position.X * 10;
        float tempy = joint.Position.Y * 10;
        float tempz = joint.Position.Z * 10;
        

        if (tempx <= -2)
        {
            DirectionPointing.text = "Too left";
            remainingTime = 5;
        }
        else if (tempx >= 2)
        {
            DirectionPointing.text = "Too Right";
            remainingTime = 5;
        }
        else if (tempz <= 13)
        {
            DirectionPointing.text = "Too forward";
            remainingTime = 5;
        }
        else if (tempz >= 18)
        {
            DirectionPointing.text = "Too backward";
            remainingTime = 5;
        }
        else
        {
            DirectionPointing.text = "You found center";
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
        }
        
    }
    public void MovementCal(Joint joint)
    {
        Instructions.text = "Try to move around \n every direction 2 different times to continue";

        //----- POSITIONS  ------//
        float tempx = joint.Position.X * 10;
        float tempy = joint.Position.Y * 10;
        float tempz = joint.Position.Z * 10;


        if (tempx <= -2)
        {
            if (prev != 1)
            {
                counterL = counterL + 1;
                DirectionPointing.text = "Left " + counterL;
                prev = 1;
            }
            
        }
        else if (tempx >= 2)
        {
            if (prev != -1)
            {
                counterR = counterR + 1;
                DirectionPointing.text = "Right " + counterR;
                prev = -1;
            }
            
        }
        else if (tempz <= 13)
        {
            if (prev != 2)
            {
                counterF = counterF + 1;
                DirectionPointing.text = "Forward " + counterF;
                prev = 2;
            }
            
        }
        else if (tempz >= 18)
        {
            if (prev != -2)
            {
                counterB = counterB + 1;
                DirectionPointing.text = "Backward " + counterB;
                prev = -2;
            }
            
        }
        else
        {
            DirectionPointing.text = "Center ";
        }


        //----- GET OUT ------//
        if (counterL>=2 && counterR >= 2 && counterF >= 2 && counterB >= 2)
        {
            SceneManager.LoadScene("SampleScene");
        }

    }
}
