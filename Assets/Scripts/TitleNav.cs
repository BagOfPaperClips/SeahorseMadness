using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.EventSystems;
using UnityEngine.EventSystems;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public class TitleNav : MonoBehaviour
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;
    int prev = 0;

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
       
        //JointType.HandLeft,
        //JointType.HandRight,
        //JointType.SpineBase,
        JointType.Head,
    };

    public GameObject playButton, credits, exit;
    void Update()
    {

        //clear the selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(playButton);

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
    public void MovementVal(Joint joint)
    {
        float tempx = joint.Position.X * 10;
        float tempy = joint.Position.Y * 10;
        float tempz = joint.Position.Z * 10;

        if (tempx <= -2)
        {
            if (prev != 1)
            {

                prev = 1;
            }

        }
        else if (tempx >= 2)
        {
            if (prev != -1)
            {

                prev = -1;
            }

        }
        else if (tempz <= 13)
        {
            if (prev != 2)
            {

                prev = 2;
            }

        }
        else if (tempz >= 18)
        {
            if (prev != -2)
            {

                prev = -2;
            }

        }


    }
}
