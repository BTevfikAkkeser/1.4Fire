using UnityEngine;
using System;

[CreateAssetMenu(fileName = "VehicleEvent", menuName = "Driving/Vehicle Event")]
public class VehicleEvent : ScriptableObject
{
    [Header("Olay Ayarları")]
    public string eventName;
    public string description;
    public float duration = 5f;
    public bool isHighPriority = false;
    
    [Header("Görsel ve Ses Efektleri")]
    public bool hasVisualEffect = false;
    public bool hasSoundEffect = false;
    public AudioClip soundEffect;
    public GameObject visualEffectPrefab;
    
    [Header("Oyun Etkileri")]
    public float drivingDifficultyIncrease = 0.2f;
    public bool requiresPlayerAction = false;
    public string playerActionDescription;
    
    [Header("İstatistikler")]
    public int timesTriggered = 0;
    public float averagePlayerReactionTime = 0f;
    
    // Bu olayı tetiklemek için çağrılan yöntem
    public virtual void TriggerEvent(DriftController vehicle)
    {
        Debug.Log($"Olay tetiklendi: {eventName}");
        timesTriggered++;
        
        // Alt sınıflar bu yöntemi override ederek özel davranışlar ekleyebilir
    }
    
    // Olayı sonlandırmak için çağrılan yöntem
    public virtual void EndEvent(DriftController vehicle)
    {
        Debug.Log($"Olay sonlandı: {eventName}");
    }
}