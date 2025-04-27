using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Araba içi etkileşimler için temel ScriptableObject sınıfı
/// </summary>
[CreateAssetMenu(fileName = "New Car Interaction", menuName = "Car Interior/Interaction Event")]
public class CarInteractionEventSO : ScriptableObject
{
    [Header("Element Settings")]
    public string displayName = "Interactive Element";
    public string interactionMessage = "Press E to interact";

    [Header("Visual Feedback")]
    public Color highlightColor = Color.yellow;
    public Sprite displayIcon;
    
    [Header("Audio Feedback")]
    public AudioClip interactionSound;
    public float soundVolume = 1.0f;
    
    [Header("State Settings")]
    public bool hasToggleState = true;
    public bool defaultState = false;

    [Header("VFX Settings")]
    public bool useVFX = false;
    public GameObject onStateVFXPrefab;
    public GameObject offStateVFXPrefab;
    public Vector3 vfxOffset = Vector3.zero;
    public float vfxDuration = 2.0f;

    //[Header("Transform Settings")]
    public enum TransformationType
    {
        None,
        Rotate,
        Slide,
        Toggle,
        VolumeControl
    }
    public TransformationType transformationType = TransformationType.None;
    public Vector3 rotationAxis = Vector3.up;     // Rotate için
    public float rotationAmount = 90f;            // Rotate için (derece)
    public Vector3 slideDirection = Vector3.forward; // Slide için
    public float slideDistance = 0.1f;            // Slide için
    public Vector3 toggleRotation = new Vector3(0, 0, -45); // Toggle için
    
    [Header("Value Settings")]
    public float minValue = 0f;    // VolumeControl için
    public float maxValue = 1f;    // VolumeControl için
    public float defaultValue = 0f; // VolumeControl için
    
    [Header("Custom Parameters")]
    public List<InteractionParameter> parameters = new List<InteractionParameter>();
    
    /// <summary>
    /// Etkileşim gerçekleştiğinde çağrılır
    /// </summary>
    public virtual void OnInteract(GameObject target, bool currentState)
    {
        // Temel sınıfta uygulama yok, türetilmiş sınıflarda uygulanacak
        Debug.Log($"Interacted with {displayName}. Current state: {currentState}");
    }
}

/// <summary>
/// Etkileşim parametresi
/// </summary>
[System.Serializable]
public class InteractionParameter
{
    public enum ParameterType { Float, Integer, Boolean, String }
    
    public string paramName = "Parameter";
    public ParameterType type = ParameterType.Float;
    
    // Parametre değer seçenekleri
    public float floatValue = 0f;
    public int intValue = 0;
    public bool boolValue = false;
    public string stringValue = "";
    
    // Türüne göre değeri döndür
    public object GetValue()
    {
        switch (type)
        {
            case ParameterType.Float:
                return floatValue;
            case ParameterType.Integer:
                return intValue;
            case ParameterType.Boolean:
                return boolValue;
            case ParameterType.String:
                return stringValue;
            default:
                return null;
        }
    }
}