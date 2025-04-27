using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple car interior interaction system with outline highlighting, name display and
/// three interaction types
/// </summary>
public class CarInteriorInteraction : MonoBehaviour
{
    [Header("UI References")]
    public Text centerText; // Single text for displaying model name
    
    [Header("Interaction Settings")]
    public float interactionDistance = 2.5f;
    public KeyCode interactionKey = KeyCode.E;
    public LayerMask interactionLayer = -1;
    
    private Camera playerCamera;
    private InteractableElement currentTarget;
    
    void Start()
    {
        playerCamera = Camera.main;
        
        // Make sure we have the center text component
        if (!centerText)
        {
            Debug.LogWarning("Center text not assigned in CarInteriorInteraction!");
        }
        
        // Lock cursor for first-person look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        
        // Allow to escape cursor lock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
        if (centerText)
        {
            centerText.text = "";
        }
        
        // Cast ray from center of screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // Check if we hit something within interaction distance
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            // Check if the hit object has an InteractableElement component
            InteractableElement element = hit.collider.GetComponent<InteractableElement>();
            if (element != null && element.canBeInteracted)
            {
                // Set as current target
                currentTarget = element;
                currentTarget.SetHighlighted(true);
                
                // Update UI text
                if (centerText)
                {
                    centerText.text = element.displayName;
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (playerCamera == null) return;
        
        // Draw the interaction ray
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        // Change color based on whether something is targeted
        Gizmos.color = currentTarget != null ? Color.green : Color.red;
        
        // Draw the ray
        Gizmos.DrawRay(ray.origin, ray.direction * interactionDistance);
    }
}