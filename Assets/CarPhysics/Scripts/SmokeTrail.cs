using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrail : MonoBehaviour
{
    DriftController dc;
    Rigidbody rb;
    ParticleSystem ps;
    public Material smokeMaterial;
    [SerializeField][Range(0,10)]
    private float smokeInfrequency;
    [SerializeField][Range(0,1f)]
    private float smokeSensitivity;
    float time;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        dc = transform.root.GetComponent<DriftController>();
        rb = transform.root.GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;
        ParticleSystem.EmissionModule rod = GetComponent<ParticleSystem>().emission;
        if(Mathf.Abs(Mathf.Abs(dc.m_WheelColliders[0].steerAngle)-Mathf.Abs(dc.m_WheelColliders[2].steerAngle))>smokeInfrequency&&rb.velocity.magnitude>(dc.SpeedThreshold+dc.TractionThreshold/5))
        {
        rod.rateOverDistance = 25;
        time +=Time.deltaTime/500;
        time = Mathf.Clamp(time,0,smokeSensitivity);
        smokeMaterial.SetColor("_Color",new Color(1,1,1,time));

            ps.Play();
        }
        else
        {
            time -=Time.deltaTime/50;
        time = Mathf.Clamp(time,0,smokeSensitivity);
        smokeMaterial.SetColor("_Color",new Color(1,1,1,time));
                rod.rateOverDistance = 0;
        }
        
    }
}
