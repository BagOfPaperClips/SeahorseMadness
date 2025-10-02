using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class PlayerEating : MonoBehaviour
{

    public GameObject PPGO;
    public float intensity = 0;

    PostProcessVolume _volume;
    Vignette _vignette;

    // Start is called before the first frame update
    void Start()
    {
        _volume = PPGO.GetComponent<PostProcessVolume>();

        _volume.profile.TryGetSettings<Vignette>(out _vignette);

        if (!_vignette)
        {
            print("error, vignette empty");
        }
        else
        {
            _vignette.enabled.Override(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        intensity += 0.0005f;
        if (intensity > 1f) intensity = 1f;
        _vignette.intensity.Override(intensity);

        if (Input.GetMouseButtonDown(0))
        {
            intensity = 0f;
            _vignette.intensity.Override(0f);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Food"))
        {
            Debug.Log("Food collected: ");
            intensity = 0f;
            _vignette.intensity.Override(0f);
        }
    }
}
