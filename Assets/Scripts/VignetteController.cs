using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour
{
    private Volume volume;
    private Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        volume = GetComponent<Volume>();
        if(!volume.profile.TryGet(out vignette))
        {
            Debug.Log("Vignette effect not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(vignette != null)
            {
                vignette.intensity.value = 0.5f;
            }
        }
    }
}
