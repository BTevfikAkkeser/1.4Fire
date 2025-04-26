using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTransform : MonoBehaviour
{
    // Start is called before the first frame update
   public GameObject RL;
    public GameObject FL;
    public GameObject RR;
 
    public GameObject FR;
    [Range(-10f,10f)][Tooltip("For Positive and Negative Camber")]
    public float camberValue = 5;
    [Range(-10f,10f)][Tooltip("Positive Values - Toe Out, Negative Values - Toe In")]
    public float ToeValue = 2;

    float variablex;
    float variablez;


    // Update is called once per frame
    void FixedUpdate()
    {

        variablex = -camberValue*Mathf.Cos(Mathf.Deg2Rad*(transform.rotation.eulerAngles.y-90));
        variablez = -camberValue*Mathf.Sin(Mathf.Deg2Rad*(transform.rotation.eulerAngles.y-90));
        RR.transform.rotation = Quaternion.Euler(-variablex,RR.transform.rotation.y+transform.rotation.y-GetComponent<DriftController>().m_WheelColliders[2].steerAngle,variablez);
        FR.transform.rotation = Quaternion.Euler(-variablex,FR.transform.rotation.y+ToeValue,variablez);

        FL.transform.rotation = Quaternion.Euler(180+variablex,FL.transform.rotation.y-ToeValue,variablez);
        RL.transform.rotation = Quaternion.Euler(180+variablex,FL.transform.rotation.y+transform.rotation.y-GetComponent<DriftController>().m_WheelColliders[3].steerAngle,variablez);

        
        
    }
}
