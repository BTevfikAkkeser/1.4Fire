using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// First person controller for interacting with car interior
/// </summary>
public class FirstPersonController : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 2.0f;
    public float lookUpLimit = 60.0f;    // Max looking up angle
    public float lookDownLimit = 60.0f;   // Max looking down angle
    public float lookLeftLimit = 80.0f;   // Max looking left angle
    public float lookRightLimit = 80.0f;  // Max looking right angle
    
    [Header("Zoom Settings")]
    public float defaultFOV = 60.0f;
    public float zoomedFOV = 30.0f;
    public float zoomSpeed = 10.0f;
    public KeyCode zoomKey = KeyCode.Mouse1; // Right mouse button
    
    [Header("Interaction Settings")]
    public float interactionDistance = 2.5f;
    public KeyCode interactionKey = KeyCode.E;
    public LayerMask interactionLayer = ~0; // Everything by default
    
    [Header("UI References")]
    public Text centerScreenText;
    public Image crosshair;
    public Color normalCrosshairColor = Color.white;
    public Color interactableCrosshairColor = Color.yellow;
    
    private Camera playerCamera;
    private float rotationX = 0;  // Vertical rotation (looking up/down)
    private float rotationY = 0;  // Horizontal rotation (looking left/right)
    private float targetFOV;
    private InteractableElement currentTarget;
    
    void Start()
    {
        // Get camera reference
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        targetFOV = defaultFOV;
        
        // Store initial horizontal rotation
        rotationY = playerCamera.transform.localEulerAngles.y;
    }
    
    void Update()
    {
        HandleLook();
        HandleZoom();
        CheckForInteractiveObject();
        
        // Handle interaction input
        if (Input.GetKeyDown(interactionKey) && currentTarget != null)
        {
            currentTarget.Interact();
        }
        
        // Allow cursor unlock with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Relock cursor on click
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    /// <summary>
    /// Handle mouse look controls with horizontal and vertical limits
    /// </summary>
    void HandleLook()
    {
        // Only move camera if cursor is locked
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            
            // Vertical rotation (looking up/down)
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -lookDownLimit, lookUpLimit);
            
            // Horizontal rotation (looking left/right)
            rotationY += mouseX;
            rotationY = Mathf.Clamp(rotationY, -lookLeftLimit, lookRightLimit);
            
            // Apply camera rotation with limits
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
    }
    
    /// <summary>
    /// Handle camera zoom controls
    /// </summary>
    void HandleZoom()
    {
        // Check zoom input
        if (Input.GetKey(zoomKey))
        {
            targetFOV = zoomedFOV;
        }
        else
        {
            targetFOV = defaultFOV;
        }
        
        // Smoothly adjust FOV
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
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
        
        // Update crosshair to normal color
        if (crosshair)
        {
            crosshair.color = normalCrosshairColor;
        }
        
        // Cast ray from center of screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // Check if we hit something within interaction distance
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            // Check if the hit object has an InteractableObject component
            InteractableElement interactable = hit.collider.GetComponent<InteractableElement>();
            if (interactable != null && interactable.interactionEvent != null)
            {
                // Set as current target
                currentTarget = interactable;
                currentTarget.SetHighlighted(true);
                
                // Update UI text with the name from the interaction event
                if (centerScreenText)
                {
                    centerScreenText.text = interactable.GetDisplayName();
                }
                
                // Update crosshair to interactive color
                if (crosshair)
                {
                    crosshair.color = interactableCrosshairColor;
                }
            }
        }
    }
}