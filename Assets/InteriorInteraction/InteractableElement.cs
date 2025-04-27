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
    public UnityEvent interactionEvent;
    public string interactionMessage = "Press E to interact";
    public bool canBeInteracted = true;
    
    [Header("Visual Feedback")]
    public Material standardMaterial;
    public Material highlightMaterial;
    public GameObject highlightObject;
    
    [Header("Audio Feedback")]
    public AudioClip interactionSound;
    public float soundVolume = 1.0f;
    
    private bool isHighlighted = false;
    private bool isActivated = false;
    
    /// <summary>
    /// Called when player interacts with this element
    /// </summary>
    public virtual void Interact()
    {
        if (!canBeInteracted)
            return;
            
        if (interactionEvent != null)
        {
            interactionEvent.Invoke();
        }
        
        isActivated = !isActivated;
        
        // Play interaction sound if available
        if (interactionSound)
        {
            AudioSource.PlayClipAtPoint(interactionSound, transform.position, soundVolume);
        }
        
        Debug.Log($"Interacted with {displayName}. Activated: {isActivated}");
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
    }

    public string GetDisplayName()
    {
        return displayName;
    }
}