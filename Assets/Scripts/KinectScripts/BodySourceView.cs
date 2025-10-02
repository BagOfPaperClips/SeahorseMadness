using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodySourceView : MonoBehaviour
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;

    public float sspeed = .1f;
    public Vector3 move;

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
        //JointType.HandLeft, ///CHANGED
        //JointType.HandRight, ///CHANGED

        JointType.SpineBase,
        JointType.Head,
    };
    private void Start()
    {
        move = this.transform.position;
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
            if(_joint == JointType.Head)
                Debug.Log("HEAD");

            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            Movement(sourceJoint);
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

        //Debug.Log("X" + tempx + " Y" + tempy + " Z" + tempz);


        if (tempx <= -2)
        {
            Debug.Log("LEFT");
            move.x += sspeed;
        }
        else if(tempx >= 2)
        {
            Debug.Log("RIGHT");
            move.x -= sspeed;
        }
        if(tempz <= 13)
        {
            Debug.Log("FORWARD");
            move.y += sspeed;
        }
        else if(tempz >= 18)
        {
            Debug.Log("BACKWARD");
            move.y -= sspeed;
        }

        this.transform.position = move;

    }
}
