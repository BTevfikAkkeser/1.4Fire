// VehicleEventManager.cs - Olayları yöneten sınıf
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VehicleEventManager : MonoBehaviour
{
    [Header("Referanslar")]
    public DriftController driftController;
    
    [Header("Olay Havuzu")]
    public List<VehicleEvent> lowPriorityEvents = new List<VehicleEvent>();
    public List<VehicleEvent> highPriorityEvents = new List<VehicleEvent>();
    
    [Header("RNG Ayarları")]
    public float minTimeBetweenEvents = 20f;
    public float maxTimeBetweenEvents = 60f;
    public float highPriorityChance = 0.3f;
    
    private VehicleEvent currentEvent;
    private bool isEventActive = false;
    private float difficultyMultiplier = 1f;
    
    void Start()
    {
        if (driftController == null)
        {
            driftController = GetComponent<DriftController>();
        }
        
        StartCoroutine(EventLoop());
    }
    
    IEnumerator EventLoop()
    {
        while (true)
        {
            // Aktif bir olay yoksa, bir sonraki olayı tetiklemek için bekle
            if (!isEventActive)
            {
                float waitTime = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents) / difficultyMultiplier;
                yield return new WaitForSeconds(waitTime);
                
                TriggerRandomEvent();
            }
            
            yield return null;
        }
    }
    
    void TriggerRandomEvent()
    {
        if (isEventActive || (lowPriorityEvents.Count == 0 && highPriorityEvents.Count == 0))
            return;
            
        List<VehicleEvent> eventPool;
        
        // Yüksek öncelikli olay mı tetiklenecek?
        if (Random.value < highPriorityChance && highPriorityEvents.Count > 0)
        {
            eventPool = highPriorityEvents;
        }
        else
        {
            eventPool = lowPriorityEvents.Count > 0 ? lowPriorityEvents : highPriorityEvents;
        }
        
        if (eventPool.Count == 0)
            return;
            
        // Rastgele bir olay seç
        int eventIndex = Random.Range(0, eventPool.Count);
        currentEvent = eventPool[eventIndex];
        
        // Olayı tetikle
        isEventActive = true;
        currentEvent.TriggerEvent(driftController);
        
        // Olayın süresini başlat
        StartCoroutine(EventDurationRoutine(currentEvent.duration));
    }
    
    IEnumerator EventDurationRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        if (currentEvent != null)
        {
            currentEvent.EndEvent(driftController);
            isEventActive = false;
            currentEvent = null;
        }
    }
    
    // Zorluk seviyesini güncellemek için kullanılabilir
    public void UpdateDifficulty(float newMultiplier)
    {
        difficultyMultiplier = Mathf.Clamp(newMultiplier, 0.5f, 3f);
    }
    
    // Manuel olarak bir olayı tetiklemek için
    public void TriggerSpecificEvent(VehicleEvent eventToTrigger)
    {
        if (isEventActive || eventToTrigger == null)
            return;
            
        currentEvent = eventToTrigger;
        isEventActive = true;
        currentEvent.TriggerEvent(driftController);
        StartCoroutine(EventDurationRoutine(currentEvent.duration));
    }
}