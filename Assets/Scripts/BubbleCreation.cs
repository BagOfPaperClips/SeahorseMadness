using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleCreation : MonoBehaviour
{
    public List<ParticleSystem> parts = new List<ParticleSystem>();
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem[] systemsOnThisObject = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in systemsOnThisObject)
        {
            parts.Add(ps);
        }
    }

    // Update is called once per frame
    void Update()
    {
        playParticles();
    }


    IEnumerator playParticles()
    {
        foreach (ParticleSystem ps in parts)
        {
            ps.Play();
        }
        Debug.Log("Delay Before");
        yield return new WaitForSeconds(2f);

        Debug.Log("Delay Finsihed");

        foreach (ParticleSystem ps in parts)
        {
            ps.Stop();
        }
    }
}
