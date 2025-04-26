using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidTrail : MonoBehaviour
{
    DriftController dc;
    Rigidbody rb;
    ParticleSystem ps;
    [SerializeField]
    [Range(0, 2)]
    private float skidSensitivity;
    WheelHit hit;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        dc = transform.root.GetComponent<DriftController>();
        rb = transform.root.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;
        ParticleSystem.EmissionModule rod = GetComponent<ParticleSystem>().emission;
        if (dc.isBreaking || dc.speedoffset > 1 || Mathf.Abs(hit.sidewaysSlip) > 0)
        {
            if (dc.m_WheelColliders[2].isGrounded && dc.m_WheelColliders[3].isGrounded)
            {
                rod.rateOverDistance = 25;
                if(rb.velocity.magnitude > dc.SpeedThreshold && Mathf.Abs(dc.horizontalInput) > 0.1f)
                {
                    settings.startColor = new Color(1, 1, 1, (dc.horizontalInput-0.1f) * skidSensitivity);
                }
                else if (dc.speedoffset > 1)
                {
                    settings.startColor = new Color(1, 1, 1, dc.speedoffset / dc.TractionThreshold * skidSensitivity);
                }
                else if (dc.frictionVal > 0)
                {
                    if (dc.m_WheelColliders[2].GetGroundHit(out hit))
                    {
                        if (Mathf.Abs(hit.sidewaysSlip) > 0)
                        {
                            settings.startColor = new Color(1, 1, 1, Mathf.Abs(hit.sidewaysSlip) * skidSensitivity);
                        }
                    }
                }
            }
            else
            {
                rod.rateOverDistance = 0;
            }
        }
        else
        {
            rod.rateOverDistance = 0;
        }
    }
}
