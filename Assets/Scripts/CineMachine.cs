using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class CineMachine : MonoBehaviour
{
    public CinemachineFreeLook virtualCamera;
    Movement m;
    BodySourceView BSV;


    private void Start()
    {
        m = GetComponent<Movement>();
        BSV = GetComponent<BodySourceView>();
    }
    void Update()
    {
        Vector3 pos = m.transform.position;
       if(BSV.struggleAmount){
            Debug.Log("CHANGECAM");
            virtualCamera.ForceCameraPosition(new Vector3(-100,-1000,-1000), Quaternion.identity);
            virtualCamera.m_YAxis.Value = 0;
        }
        else
        {
            virtualCamera.ForceCameraPosition(pos, Quaternion.identity);
        }
    }
}
