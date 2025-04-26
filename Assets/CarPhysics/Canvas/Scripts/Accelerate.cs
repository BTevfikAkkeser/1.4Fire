using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Accelerate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float acc;

    private bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        acc = 1;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        acc = 0;
    }

    void Update()
    {
        // Klavyeden "W" tuşu veya mobil buton kontrolü
        if (Input.GetKey(KeyCode.W))
        {
            acc = 1;
        }
        else if (!isPressed) // Mobilde butona basılmamışsa
        {
            acc = 0;
        }
    }
}
