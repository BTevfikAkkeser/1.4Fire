using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlChange : MonoBehaviour
{
    public List<Sprite> Options;
    public int currentSelection = 0;
    public GameObject PositionAcceleration, PositionDeceleration, PositionHandbrake;
    [Space]
    public GameObject LeftButton;
    public GameObject RightButton;
    public GameObject SteeringWheel;

    public void Pressed()
    {
        currentSelection++;
        currentSelection = currentSelection % Options.Count;
        GetComponent<Image>().sprite = Options[currentSelection];

        
        if (currentSelection == 0) // Keyboard
        {
            // UI'de inputlara ihtiyaç yok, hepsini kapat
            LeftButton.SetActive(false);
            RightButton.SetActive(false);
            SteeringWheel.SetActive(false);
            PositionAcceleration.SetActive(false);
            PositionDeceleration.SetActive(false);
            PositionHandbrake.SetActive(false);
        }
    }
}
