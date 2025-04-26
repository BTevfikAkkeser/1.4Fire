using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float right;

    private bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        right = 0;
    }

    void Update()
    {
        // Mobilde butona basılma kontrolü
        if (isPressed)
        {
            right += Time.deltaTime * 5f;
            right = Mathf.Clamp01(right);
        }

        // Klavye kontrolü (D tuşuna basılınca right = 1)
        if (Input.GetKey(KeyCode.D))
        {
            right = 1;
        }
        else if (!isPressed)
        {
            right = 0;
        }
    }
}
