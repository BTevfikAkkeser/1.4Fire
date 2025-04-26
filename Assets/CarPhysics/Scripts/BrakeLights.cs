using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeLights : MonoBehaviour
{
    [Tooltip("The Gameobject that has the Tail light material")]
    public GameObject TailLightObject;
    public Material Toff, Ton, ToffSecondary, TonSecondary;
    public int indexMaterial = 0;
    public int indexMaterialSecondary = 0;

    Renderer mrender;
    Material[] m;

    Decelerate mobilDccInp;
    HandBrake mobilHandBrake;
    void Start()
    {
        mrender = TailLightObject.GetComponent<Renderer>();
        m = mrender.sharedMaterials;
        mobilDccInp = FindObjectOfType<Decelerate>();
        mobilHandBrake = FindObjectOfType<HandBrake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mobilDccInp.dcc == 1 || Input.GetKey(KeyCode.Space) || mobilHandBrake)
        {
            m[indexMaterial] = Ton;
            if(TonSecondary!=null)
            m[indexMaterialSecondary] = TonSecondary;

            mrender.sharedMaterials = m;
        }
        else
        {
            m[indexMaterial] = Toff;
            if(ToffSecondary!=null)
            m[indexMaterialSecondary] = ToffSecondary;

            mrender.sharedMaterials = m;

        }

    }
}
