using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for all interactive elements in the car interior
/// </summary>
public class InteractableElement : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string displayName = "Interact";
    public bool canBeInteracted = true;
    
    [Header("Visual Feedback")]
    public Material standardMaterial;
    public Material highlightMaterial;
    public GameObject highlightObject;
    
    [Header("Audio Feedback")]
    public AudioClip interactionSound;
    public float soundVolume = 1.0f;
    
    public enum TransformationType
    {
        None,
        Rotate,  // Rotating elements like steering wheel, knobs
        Slide,   // Sliding elements like levers, windows
        Toggle   // Toggle elements like buttons, switches
    }
    public TransformationType transformationType = TransformationType.None;
    public Vector3 rotationAxis = Vector3.up;         // For Rotate type
    public float rotationAmount = 45f;                // Degrees for rotation
    public Vector3 slideDirection = Vector3.forward;  // For Slide type
    public float slideDistance = 0.1f;                // Distance for sliding
    public Vector3 toggleRotation = new Vector3(0, 0, -45); // For Toggle type
    public float transformationDuration = 0.5f;       // Animation duration
    
    [Header("Outliner Settings")]
    public bool useOutliner = true;
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 0.05f;
    
    // Events
    public UnityEvent onInteract;
    
    // State variables
    private bool isHighlighted = false;
    private bool isActivated = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine transformCoroutine;
    private InteractiveElementOutliner outliner;
    
    void Awake()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }
    
    void Start()
    {
        // Add outliner if needed
        if (useOutliner)
        {
            outliner = GetComponent<InteractiveElementOutliner>();
            if (outliner == null)
            {
                outliner = gameObject.AddComponent<InteractiveElementOutliner>();
                outliner.outlineColor = outlineColor;
                outliner.outlineWidth = outlineWidth;
                outliner.SetOutlineVisibility(false);
            }
        }
    }
    
    /// <summary>
    /// Called when player interacts with this element
    /// </summary>
    public virtual void Interact()
    {
        if (!canBeInteracted)
            return;
            
        if (onInteract != null)
        {
            onInteract.Invoke();
        }
        
        isActivated = !isActivated;
        
        // Play interaction sound if available
        if (interactionSound)
        {
            AudioSource.PlayClipAtPoint(interactionSound, transform.position, soundVolume);
        }
        
        // Handle transformation based on type
        HandleTransformation();
        
        Debug.Log($"Interacted with {displayName}. Activated: {isActivated}");
    }
    
    /// <summary>
    /// Apply transformation based on type
    /// </summary>
    private void HandleTransformation()
    {
        // Cancel any ongoing transformation
        if (transformCoroutine != null)
        {
            StopCoroutine(transformCoroutine);
        }
        
        switch (transformationType)
        {
            case TransformationType.Rotate:
                transformCoroutine = StartCoroutine(RotateElement());
                break;
                
            case TransformationType.Slide:
                transformCoroutine = StartCoroutine(SlideElement());
                break;
                
            case TransformationType.Toggle:
                transformCoroutine = StartCoroutine(ToggleElement());
                break;
        }
    }
    
    /// <summary>
    /// Rotate the element around its axis
    /// </summary>
    private IEnumerator RotateElement()
    {
        float startTime = Time.time;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles + rotationAxis * rotationAmount);
        
        while (Time.time < startTime + transformationDuration)
        {
            float t = (Time.time - startTime) / transformationDuration;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        
        transform.localRotation = targetRotation;
    }
    
    /// <summary>
    /// Slide the element in a direction
    /// </summary>
    private IEnumerator SlideElement()
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = isActivated ? 
                               originalPosition + slideDirection.normalized * slideDistance : 
                               originalPosition;
        
        while (Time.time < startTime + transformationDuration)
        {
            float t = (Time.time - startTime) / transformationDuration;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        transform.localPosition = targetPosition;
    }
    
    /// <summary>
    /// Toggle the element (rotate to a specific angle and back)
    /// </summary>
    private IEnumerator ToggleElement()
    {
        float startTime = Time.time;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = isActivated ? 
                                  Quaternion.Euler(originalRotation.eulerAngles + toggleRotation) : 
                                  originalRotation;
        
        while (Time.time < startTime + transformationDuration)
        {
            float t = (Time.time - startTime) / transformationDuration;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        
        transform.localRotation = targetRotation;
    }
    
    /// <summary>
    /// Called when player highlights this element
    /// </summary>
    public virtual void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;
        
        // Enable/disable highlight object if available
        if (highlightObject)
        {
            highlightObject.SetActive(highlighted);
        }
        
        // Change material if available
        Renderer renderer = GetComponent<Renderer>();
        if (renderer && standardMaterial && highlightMaterial)
        {
            renderer.material = highlighted ? highlightMaterial : standardMaterial;
        }
        
        // Control outliner visibility
        if (outliner != null)
        {
            outliner.SetOutlineVisibility(highlighted);
        }
    }
}