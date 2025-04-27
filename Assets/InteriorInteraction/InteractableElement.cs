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
    public GameObject highlightElement;
    
    [Header("Audio Feedback")]
    public AudioClip interactionSound;
    public float soundVolume = 1.0f;
    
    //[Header("Transformation Settings")]
    public enum TransformationType
    {
        None,
        Rotate,
        Slide,
        Toggle,
        VolumeControl
    }
    public TransformationType transformationType = TransformationType.None;
    public Vector3 rotationAxis = Vector3.up; // Rotasyon ekseni
    public float rotationSpeed = 45f; // Derece/saniye
    public Vector3 slideDirection = Vector3.forward; // Kayma yönü
    public float slideDistance = 0.1f; // Metre cinsinden kayma mesafesi
    public Vector3 toggleRotation = new Vector3(0, 0, -45); // Toggle döndürme açısı
    public float transformationDuration = 0.5f; // Dönüşüm süresi (saniye)
    public float minValue = 0f; // Minimum değer (ses için)
    public float maxValue = 1f; // Maximum değer (ses için)
    public float currentValue = 0f; // Mevcut değer (0-1 arası)
    
    [Header("Outliner Settings")]
    public bool useOutliner = true;
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 0.05f;
    
    // Events
    public UnityEvent<float> onValueChanged; // Değer değiştiğinde
    public UnityEvent<bool> onToggled; // Açılıp kapandığında
    
    // State variables
    private bool isHighlighted = false;
    private bool isActivated = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine transformCoroutine;
    private InteractiveElementOutliner outliner;
    private AudioSource audioSource;
    
    void Awake()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        
        // Ses kontrolü için AudioSource bileşeni
        audioSource = GetComponent<AudioSource>();
    }
    
    void Start()
    {
        // Outliner ekle (eğer kullanılacaksa)
        if (useOutliner)
        {
            outliner = GetComponent<InteractiveElementOutliner>();
            if (outliner == null)
            {
                outliner = gameObject.AddComponent<InteractiveElementOutliner>();
                outliner.outlineColor = outlineColor;
                outliner.outlineWidth = outlineWidth;
                outliner.alwaysVisible = false;
                outliner.showOnHighlight = true;
            }
        }
        
        // Başlangıç durumunu ayarla
        if (transformationType == TransformationType.VolumeControl && audioSource != null)
        {
            audioSource.volume = currentValue;
        }
    }
    
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
                onToggled.Invoke(isActivated);
                break;
                
            case TransformationType.VolumeControl:
                // Volume kontrol için interaksiyon mantığını değiştir
                // Her etkileşimde sesi bir adım artır, maksimuma ulaşınca sıfırla
                SetVolumeStep();
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
        Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles + rotationAxis * 90f);
        
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
    /// Set volume in steps
    /// </summary>
    private void SetVolumeStep()
    {
        // Her tıklamada sesi %20 artır, %100'e ulaşınca sıfırla
        currentValue += 0.2f;
        if (currentValue > 1.0f)
        {
            currentValue = 0f;
        }
        
        // Ses değerini ayarla
        if (audioSource != null)
        {
            audioSource.volume = currentValue;
        }
        
        // Event'i tetikle
        onValueChanged.Invoke(currentValue);
        
        Debug.Log($"Volume set to {currentValue * 100}%");
    }
    
    /// <summary>
    /// Set the volume directly to a specific value (0-1)
    /// </summary>
    public void SetVolume(float value)
    {
        currentValue = Mathf.Clamp01(value);
        if (audioSource != null)
        {
            audioSource.volume = currentValue;
        }
        onValueChanged.Invoke(currentValue);
    }
    
    /// <summary>
    /// Called when player highlights this element
    /// </summary>
    public virtual void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;
        
        // Enable/disable highlight object if available
        if (highlightElement)
        {
            highlightElement.SetActive(highlighted);
        }
        
        // Change material if available
        Renderer renderer = GetComponent<Renderer>();
        if (renderer && standardMaterial && highlightMaterial)
        {
            renderer.material = highlighted ? highlightMaterial : standardMaterial;
        }
        
        // Outline kontrolü
        if (outliner != null)
        {
            outliner.SetOutlineVisibility(highlighted);
        }
    }

    public string GetDisplayName()
    {
        return displayName;
    }
    
    /// <summary>
    /// Set the outline color
    /// </summary>
    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        if (outliner != null)
        {
            outliner.SetOutlineColor(color);
        }
    }
    
    /// <summary>
    /// Set the outline width
    /// </summary>
    public void SetOutlineWidth(float width)
    {
        outlineWidth = width;
        if (outliner != null)
        {
            outliner.SetOutlineWidth(width);
        }
    }
}