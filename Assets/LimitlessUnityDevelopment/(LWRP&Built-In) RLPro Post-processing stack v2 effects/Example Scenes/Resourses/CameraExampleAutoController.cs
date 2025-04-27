using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraExampleAutoController : MonoBehaviour
{
    // Your Post-processing volume with VHS Rewind Effect;
    public PostProcessVolume volume;

    // Temp Glitch effect.
    private RLProVHSRewind m_effect;
    private RLProAnalogTVNoise m_effect1;
    private RLProBleed m_effect2;
    private RLProTVEffect m_effect3;

    private Vector3 tr;
    private float speed = 0.01f;
    private float Rspeed = 0.1f;
    private float spotA = 2.7f;
    private float spotB = -0.5f;
    private bool enableEffect = true;

    // Start is called before the first frame update
    void Start()
    {
        //Null check
        if (volume == null)
            return;

        // Get refference to glitch effect
        volume.profile.TryGetSettings(out m_effect);
        volume.profile.TryGetSettings(out m_effect1);
        volume.profile.TryGetSettings(out m_effect2);
        volume.profile.TryGetSettings(out m_effect3);

        //Null check
        if (m_effect is null)
        {
            Debug.Log("Add VHSTapeRewind effect to your Volume component to make CameraExampleAutoController work");
            return;
        }
        if (m_effect1 is null)
        {
            Debug.Log("Add AnalogTVNoise effect to your Volume component to make CameraExampleAutoController work");
            return;
        }
        if (m_effect2 is null)
        {
            Debug.Log("Add Bleed effect to your Volume component to make CameraExampleAutoController work");
            return;
        }
        if (m_effect3 is null)
        {
            Debug.Log("Add TVEffect effect to your Volume component to make CameraExampleAutoController work");
            return;
        }

        //Activate effect
        m_effect.active = true;
        m_effect1.active = true;
        m_effect2.active = true;
        m_effect3.active = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.z > spotA)
		{

            speed = -speed;

            enableEffect = !enableEffect;

        }
        else if(transform.position.z < spotB)
		{
            speed = -speed;
            enableEffect = !enableEffect;

        }

        if (transform.rotation.eulerAngles.z > 5)
            Rspeed = -Rspeed;
        else if (transform.rotation.eulerAngles.z < -5)
            Rspeed = -Rspeed;
        Quaternion r = transform.rotation;

        r.eulerAngles = r.eulerAngles+new Vector3(Rspeed, Rspeed*2, Rspeed);



        tr = new Vector3(0, 0, speed);
        transform.SetPositionAndRotation(transform.position + tr, r);


        //Null check
        if (volume == null)
            return;
        if (m_effect is null)
            return;

		// Randomly change glitch value

		if (enableEffect)
		{
            m_effect.fade.value = 1;
            m_effect1.active = true;
            m_effect2.active = true;
            m_effect3.active = true;

		}

		else
		{
            m_effect.fade.value = 0;
            m_effect1.active = false;
            m_effect2.active = false;
            m_effect3.active = false;
        }

    }
}
