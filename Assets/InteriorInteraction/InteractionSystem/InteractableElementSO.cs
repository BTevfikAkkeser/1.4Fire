using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Araba içinde etkileşimli elemanlara eklenen bileşen
/// ScriptableObject tabanlı etkileşim sistemi için tasarlanmıştır
/// </summary>
public class InteractableElementSO : MonoBehaviour
{
    [Header("Interaction Configuration")]
    public CarInteractionEventSO interactionEvent;
    public bool canBeInteracted = true;
    
    [Header("Visual Components")]
    public Renderer mainRenderer;
    public Material standardMaterial;
    public Material highlightMaterial;
    public GameObject highlightObject;
    
    [Header("Outline Settings")]
    public bool useOutliner = true;
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 0.05f;
    
    [Header("Events")]
    public UnityEvent<bool> onStateChanged;
    public UnityEvent onInteract;
    public UnityEvent<float> onValueChanged;
    
    // Eleman durumu
    [HideInInspector] public bool isActivated = false;
    [HideInInspector] public bool isHighlighted = false;
    [HideInInspector] public float currentValue = 0f;
    
    // Transformasyon değişkenleri
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine transformCoroutine;
    private InteractiveElementOutliner outliner;
    private AudioSource audioSource;
    
    void Awake()
    {
        // Başlangıç değerlerini kaydet
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        
        // AudioSource kontrolü
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && interactionEvent != null && interactionEvent.interactionSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Start()
    {
        // ScriptableObject'ten ilk değerleri al
        if (interactionEvent != null)
        {
            isActivated = interactionEvent.defaultState;
            currentValue = interactionEvent.defaultValue;
            
            // Eğer tanımlanmamışsa outline rengi ve kalınlığını ScriptableObject'ten al
            if (outlineColor == Color.yellow && interactionEvent.highlightColor != Color.yellow)
            {
                outlineColor = interactionEvent.highlightColor;
            }
        }
        
        // Renderer referansını al
        if (mainRenderer == null)
        {
            mainRenderer = GetComponent<Renderer>();
        }
        
        // Outliner ekle
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
            
            // Outline'ı başlangıçta gizle
            outliner.SetOutlineVisibility(false);
        }
    }
    
    /// <summary>
    /// Elemana etkileşim
    /// </summary>
    public virtual void Interact()
    {
        if (!canBeInteracted || interactionEvent == null)
            return;
            
        // Toggle durumunu güncelle
        if (interactionEvent.hasToggleState)
        {
            isActivated = !isActivated;
            onStateChanged.Invoke(isActivated);
        }
        
        // Etkileşim sesi
        if (interactionEvent.interactionSound != null)
        {
            AudioSource.PlayClipAtPoint(interactionEvent.interactionSound, transform.position, interactionEvent.soundVolume);
        }
        
        // ScriptableObject'in etkileşim metodunu çağır
        interactionEvent.OnInteract(gameObject, isActivated);
        
        // Yerel etkileşim olayını tetikle
        onInteract.Invoke();
        
        // Transformasyon animasyonunu başlat
        HandleTransformation();
    }
    
    /// <summary>
    /// Transformasyon tipine göre gerekli dönüşümü uygula
    /// </summary>
    private void HandleTransformation()
    {
        // Önceki animasyonu durdur
        if (transformCoroutine != null)
        {
            StopCoroutine(transformCoroutine);
        }
        
        // Transformasyon tipine göre işlem yap
        if (interactionEvent != null)
        {
            switch (interactionEvent.transformationType)
            {
                case CarInteractionEventSO.TransformationType.Rotate:
                    transformCoroutine = StartCoroutine(RotateElement());
                    break;
                    
                case CarInteractionEventSO.TransformationType.Slide:
                    transformCoroutine = StartCoroutine(SlideElement());
                    break;
                    
                case CarInteractionEventSO.TransformationType.Toggle:
                    transformCoroutine = StartCoroutine(ToggleElement());
                    break;
                    
                case CarInteractionEventSO.TransformationType.VolumeControl:
                    // Ses değerini kademeli olarak artır
                    SetNextVolumeStep();
                    break;
            }
        }
    }
    
    /// <summary>
    /// Elemanı döndürme animasyonu
    /// </summary>
    private IEnumerator RotateElement()
    {
        float startTime = Time.time;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles + interactionEvent.rotationAxis * interactionEvent.rotationAmount);
        
        float duration = 0.5f; // Saniye
        
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        
        transform.localRotation = targetRotation;
    }
    
    /// <summary>
    /// Elemanı kaydırma animasyonu
    /// </summary>
    private IEnumerator SlideElement()
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = isActivated ? 
                              originalPosition + interactionEvent.slideDirection.normalized * interactionEvent.slideDistance : 
                              originalPosition;
        
        float duration = 0.5f; // Saniye
        
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        transform.localPosition = targetPosition;
    }
    
    /// <summary>
    /// Elemanı açma/kapama animasyonu
    /// </summary>
    private IEnumerator ToggleElement()
    {
        float startTime = Time.time;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = isActivated ? 
                                  Quaternion.Euler(originalRotation.eulerAngles + interactionEvent.toggleRotation) : 
                                  originalRotation;
        
        float duration = 0.5f; // Saniye
        
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        
        transform.localRotation = targetRotation;
    }
    
    /// <summary>
    /// Ses seviyesini kademeli olarak artırma
    /// </summary>
    private void SetNextVolumeStep()
    {
        // 0.2 birim artır ve 1.0'ı aşınca sıfırla
        currentValue += 0.2f;
        if (currentValue > interactionEvent.maxValue)
        {
            currentValue = interactionEvent.minValue;
        }
        
        // Değer değişim olayını tetikle
        onValueChanged.Invoke(currentValue);
    }
    
    /// <summary>
    /// Ses seviyesini doğrudan ayarlama
    /// </summary>
    public void SetValue(float value)
    {
        if (interactionEvent == null)
            return;
            
        currentValue = Mathf.Clamp(value, interactionEvent.minValue, interactionEvent.maxValue);
        onValueChanged.Invoke(currentValue);
    }
    
    /// <summary>
    /// Elemanı vurgulama
    /// </summary>
    public virtual void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;
        
        // Vurgulama objesini göster/gizle
        if (highlightObject)
        {
            highlightObject.SetActive(highlighted);
        }
        
        // Materyali değiştir
        if (mainRenderer && standardMaterial && highlightMaterial)
        {
            mainRenderer.material = highlighted ? highlightMaterial : standardMaterial;
        }
        
        // Outline'ı göster/gizle
        if (outliner)
        {
            outliner.SetOutlineVisibility(highlighted);
        }
    }
    
    /// <summary>
    /// Görüntülenecek adı al
    /// </summary>
    public string GetDisplayName()
    {
        return interactionEvent != null ? interactionEvent.displayName : gameObject.name;
    }
    
    /// <summary>
    /// Etkileşim mesajını al
    /// </summary>
    public string GetInteractionMessage()
    {
        return interactionEvent != null ? interactionEvent.interactionMessage : "Press E to interact";
    }
}