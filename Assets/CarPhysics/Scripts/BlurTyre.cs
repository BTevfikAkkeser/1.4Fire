using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurTyre : MonoBehaviour
{
    //public GameObject FL,FR,RL,RR;
    public Material rimTexture;
    public Texture[] BlurAmount;
    DriftController driftController;
    Rigidbody rb;
    Decelerate decelerate;
    HandBrake handBrake;
    void Start()
    {
        driftController = GetComponent<DriftController>();
        decelerate = FindObjectOfType<Decelerate>();
        handBrake = FindObjectOfType<HandBrake>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (decelerate.dcc<0 || handBrake.hb)
        {
            rimTexture.SetTexture("_MainTex", BlurAmount[0]);

        }
        else
        {
            if (driftController.SpeedThreshold / 2 > rb.velocity.magnitude)
                rimTexture.SetTexture("_MainTex", BlurAmount[0]);
            else if (driftController.SpeedThreshold / 2 < rb.velocity.magnitude && driftController.SpeedThreshold > rb.velocity.magnitude)
                rimTexture.SetTexture("_MainTex", BlurAmount[1]);
            else if (driftController.SpeedThreshold < rb.velocity.magnitude)
                rimTexture.SetTexture("_MainTex", BlurAmount[2]);
        }

    }
}
