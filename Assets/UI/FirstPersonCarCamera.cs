using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// First person camera controller for sitting in a car with limited horizontal movement
/// </summary>
public class FirstPersonCarCamera : MonoBehaviour
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
    
    [Header("Optional Animation Settings")]
    public bool enableHeadBobbing = false;
    public float bobbingAmount = 0.05f;
    public float bobbingSpeed = 0.5f;
    
    private Camera playerCamera;
    private float rotationX = 0;  // Vertical rotation (looking up/down)
    private float rotationY = 0;  // Horizontal rotation (looking left/right)
    private float targetFOV;
    private Vector3 initialPosition;
    private float timer = 0;
    
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        targetFOV = defaultFOV;
        initialPosition = transform.localPosition;
        
        // Store initial rotation
        rotationY = transform.localEulerAngles.y;
    }
    
    void Update()
    {
        HandleLook();
        HandleZoom();
        
        if (enableHeadBobbing)
        {
            ApplyHeadBobbing();
        }
        
        // Allow cursor unlock with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    /// <summary>
    /// Handle mouse look controls with limits
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
            transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
        
        // Relock cursor on click
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
    /// Apply head bobbing effect for realism
    /// </summary>
    void ApplyHeadBobbing()
    {
        timer += Time.deltaTime * bobbingSpeed;
        
        // Apply gentle up/down motion
        Vector3 bobPosition = initialPosition;
        bobPosition.y += Mathf.Sin(timer) * bobbingAmount;
        transform.localPosition = bobPosition;
    }
}