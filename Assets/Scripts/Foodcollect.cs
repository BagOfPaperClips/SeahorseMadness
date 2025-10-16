using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foodcollect : MonoBehaviour
{
    SphereCollider _collider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            _collider=gameObject.GetComponent<SphereCollider>();
            _collider.enabled=false;
            Destroy(gameObject,0.8f);
        }
    }
}
