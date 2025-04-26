using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackFire : MonoBehaviour
{
    public DriftController driftController;
    void Start()
    {
        driftController = transform.root.GetComponent<DriftController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(driftController.backfire)
        GetComponent<ParticleSystem>().Play();
        else
        GetComponent<ParticleSystem>().Stop();
        
    }
}
