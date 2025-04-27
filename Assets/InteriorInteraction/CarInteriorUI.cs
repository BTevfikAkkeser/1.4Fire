using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI for interactive car interior elements
/// </summary>
public class CarInteriorUI : MonoBehaviour
{
    [Header("UI References")]
    public Text centerScreenText; // Text displayed in the center of the screen
    
    [Header("Interaction Settings")]
    public float interactionDistance = 2.5f;
    public KeyCode interactionKey = KeyCode.E;
    
    private Camera playerCamera;
    private InteractableElement currentTarget;
    
    void Start()
    {
        playerCamera = Camera.main;
        
        // Make sure we have the center text component
        if (!centerScreenText)
        {
            Debug.LogWarning("Center screen text not assigned in CarInteriorUI!");
        }
    }
    
    void Update()
    {
        // Find what the player is looking at
        CheckForInteractiveObject();
        
        // Handle interaction input
        if (Input.GetKeyDown(interactionKey) && currentTarget != null)
        {
            currentTarget.Interact();
        }
    }
    
    /// <summary>
    /// Cast a ray from camera and check if it hits an interactive object
    /// </summary>
    void CheckForInteractiveObject()
    {
        // Clear previous target highlight if it exists
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
            currentTarget = null;
        }
        
        // Clear the center text
        if (centerScreenText)
        {
            centerScreenText.text = "";
        }
        
        // Cast ray from center of screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // Check if we hit something within interaction distance
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Check if the hit object has an InteractableElement component
            InteractableElement element = hit.collider.GetComponent<InteractableElement>();
            if (element != null && element.canBeInteracted)
            {
                // Set as current target
                currentTarget = element;
                currentTarget.SetHighlighted(true);
                
                // Update UI text
                if (centerScreenText)
                {
                    centerScreenText.text = element.displayName;
                }
            }
        }
    }
}