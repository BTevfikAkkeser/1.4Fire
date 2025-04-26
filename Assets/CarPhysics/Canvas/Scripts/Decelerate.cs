using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Decelerate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float dcc;
    public KeyCode decelerateKey = KeyCode.S; // Default key, can be changed in inspector
    
    void Update()
    {
        // Check for keyboard input
        if (Input.GetKeyDown(decelerateKey))
        {
            dcc = -1;
        }
        else if (Input.GetKeyUp(decelerateKey))
        {
            dcc = 0;
        }
    }

    // Existing pointer functionality
    public void OnPointerDown(PointerEventData eventData)
    {
        dcc = -1;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dcc = 0;
    }
}