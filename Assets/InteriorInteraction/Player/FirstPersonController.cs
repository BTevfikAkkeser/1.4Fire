using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPOOutline;
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
    
    [Header("Debug Settings")]
    public bool showDebugRay = true;
    public Color debugRayColor = Color.yellow;
    public Color debugHitColor = Color.green;
    
    private Camera playerCamera;
    private float rotationX = 0;  // Vertical rotation (looking up/down)
    private float rotationY = 0;  // Horizontal rotation (looking left/right)
    private float targetFOV;
    private Outlinable currentTarget;
    
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
            InteractWithObject();
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
            // Disable the outline effect
            currentTarget.GetComponent<Outline>().enabled = false;
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
        
        // Check if ray hits something within interaction distance
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            // Check if hit object has Outlinable component
            Outlinable outlinable = hit.collider.GetComponent<Outlinable>();
            if (outlinable != null)
            {
                // Set as current target and highlight it
                currentTarget = outlinable;
                // Enable the outline effect
                currentTarget.GetComponent<Outline>().enabled = true;
                
                // Update UI
                if (centerScreenText)
                {
                    centerScreenText.text = "Press E to Interact";  // Customize prompt here
                }
                
                if (crosshair)
                {
                    crosshair.color = interactableCrosshairColor;
                }
            }
        }

        // Debug visualization
        if (showDebugRay)
        {
            Color rayColor = currentTarget != null ? debugHitColor : debugRayColor;
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, rayColor);
        }
    }

    void InteractWithObject()
    {
        if (currentTarget != null)
        {
            // Perform interaction
            Debug.Log("Interacting with " + currentTarget.gameObject.name);
            // Here you can add specific interaction logic like opening doors, triggering events, etc.
            // For example, if the object has a specific interaction method, you could call it:
            // currentTarget.Interact(); if it has such a method
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebugRay || playerCamera == null) return;

        // Draw ray in scene view
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Gizmos.color = currentTarget != null ? debugHitColor : debugRayColor;
        Gizmos.DrawRay(ray.origin, ray.direction * interactionDistance);

        // Draw interaction sphere at hit point if there's a target
        if (currentTarget != null)
        {
            Gizmos.DrawWireSphere(currentTarget.transform.position, 0.1f);
        }
    }
}
