using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandBrake : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool hb;
 
public void OnPointerDown(PointerEventData eventData){
    hb = true;
}
 
public void OnPointerUp(PointerEventData eventData){
    hb = false;
}
}
