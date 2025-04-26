using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SteeringWheel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
 Image Wheel; //the thing you're trying to rotate
 Vector2 dir;
 float dist;
 float angle;
 public float result;

public bool isRotating = false;
 
public void OnPointerDown(PointerEventData eventData){
    isRotating = true;
}
 
public void OnPointerUp(PointerEventData eventData){
    isRotating = false;
}

 void Start()
 {
     Wheel = GetComponent<Image>();
 }
 Touch t;
     void Update(){
         if (isRotating) {
             if (Input.touchCount > 0)
         {
             for (int i = 0; i < Input.touchCount; i++) 
   { 
     if (Input.touches[i].phase == TouchPhase.Moved) 
     {  
       t = Input.touches[i]; 
     } 
   }
             
                dir  =    (t.position - (Vector2)Wheel.transform.position); 

             dist =    Mathf.Sqrt (dir.x * dir.x + dir.y * dir.y); 

             if (dist < 700 && dist > 10) {
                 angle =    Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg; 
                if(angle<100&&angle>-100)
                Wheel.transform.rotation = Quaternion.AngleAxis (angle, Vector3.back);
                
             }
             
         }
         }
         else
         {
            Wheel.transform.rotation = Quaternion.Lerp(Wheel.transform.rotation, Quaternion.identity,Time.deltaTime*10);
         }
         result = Wheel.transform.rotation.eulerAngles.z;
                result = (result<180)? -result : 360 - result;
     }

}
