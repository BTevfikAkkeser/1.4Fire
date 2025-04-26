using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeftButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float left;

    private bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        left = 0;
    }

    void Update()
    {
        // Mobilde butona basılma kontrolü
        if (isPressed)
        {
            left += Time.deltaTime * 5f;
            left = Mathf.Clamp01(left);
        }

        // Klavye kontrolü (D tuşuna basılınca right = 1)
        if (Input.GetKey(KeyCode.A))
        {
            left = 1;
        }
        else if (!isPressed)
        {
            left = 0;
        }
    }
}
