using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventPlanItem
{
    public string eventName;
    public bool isFailChain;
}

public class CarEventManager : MonoBehaviour
{
    [Header("Event Setup")]
    [SerializeField] private List<CarInteractionEventSO> eventSequence;
    
    [Header("Event Origin")]
    [SerializeField] private Transform eventOrigin;
    
    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = true;

    [Header("Runtime Event Plan (Debug Only)")]
    [SerializeField] private List<EventPlanItem> eventPlan = new List<EventPlanItem>();

    private int currentEventIndex = 0;
    private bool waitingForInteraction = false;
    private bool isFailChainActive = false;
    private CarInteractionEventSO currentInteractionEvent = null;
    private List<CarInteractionEventSO> currentFailChain = null;
    private int currentFailChainIndex = 0;

    private void Start()
    {
        // Set eventOrigin to this object if not specified
        if (eventOrigin == null)
            eventOrigin = this.transform;
            
        BuildEventPlan();
        StartCoroutine(PlayEvents());
    }

    private void BuildEventPlan()
    {
        eventPlan.Clear();
        foreach (var ev in eventSequence)
        {
            eventPlan.Add(new EventPlanItem
            {
                eventName = ev.eventName + (ev.useRandomChance ? " (RNG)" : ""),
                isFailChain = false
            });
        }
    }

    private IEnumerator PlayEvents()
    {
        while (currentEventIndex < eventSequence.Count)
        {
            CarInteractionEventSO currentEvent = eventSequence[currentEventIndex];

            yield return new WaitForSeconds(currentEvent.delayAfterPrevious);

            if (currentEvent.useRandomChance)
            {
                float roll = Random.value;
                if (debugMode) Debug.Log($"RNG Roll for {currentEvent.eventName}: {roll}");

                if (roll > currentEvent.chanceToTrigger)
                {
                    if (debugMode) Debug.Log($"Event {currentEvent.eventName} FAILED RNG check, switching to Fail Chain.");

                    if (currentEvent.failEvents != null && currentEvent.failEvents.Count > 0)
                    {
                        InsertFailChainIntoPlan(currentEventIndex + 1, currentEvent.failEvents);
                        yield return StartCoroutine(PlayFailChain(currentEvent.failEvents));
                        
                        // Continue with the next event after finishing the fail chain
                        currentEventIndex++;
                        continue;
                    }
                    else
                    {
                        currentEventIndex++;
                        continue;
                    }
                }
            }

            if (currentEvent.waitForInteraction)
            {
                waitingForInteraction = true;
                currentInteractionEvent = currentEvent;
                ShowInteractionPrompt(currentEvent.interactionPrompt);

                while (waitingForInteraction)
                {
                    yield return null;
                }
            }
            else
            {
                TriggerEvent(currentEvent);
            }

            currentEventIndex++;
        }

        if (debugMode) Debug.Log("Event Sequence Finished.");
    }

    private IEnumerator PlayFailChain(List<CarInteractionEventSO> failEvents)
    {
        if (debugMode) Debug.Log("Playing Fail Event Chain!");

        isFailChainActive = true;
        currentFailChain = failEvents;
        currentFailChainIndex = 0;

        foreach (var failEvent in failEvents)
        {
            yield return new WaitForSeconds(failEvent.delayAfterPrevious);

            if (failEvent.waitForInteraction)
            {
                waitingForInteraction = true;
                currentInteractionEvent = failEvent;
                ShowInteractionPrompt(failEvent.interactionPrompt);

                while (waitingForInteraction)
                {
                    yield return null;
                }
            }
            else
            {
                TriggerEvent(failEvent);
            }
            
            currentFailChainIndex++;
        }

        if (debugMode) Debug.Log("Fail Event Chain Finished.");
        isFailChainActive = false;
        currentFailChain = null;
    }

    private void InsertFailChainIntoPlan(int insertIndex, List<CarInteractionEventSO> failEvents)
    {
        // Create a temporary list for debug visualization
        List<EventPlanItem> updatedPlan = new List<EventPlanItem>();
        
        // Add events before the insertion point
        for (int i = 0; i < insertIndex; i++)
        {
            updatedPlan.Add(eventPlan[i]);
        }
        
        // Add fail chain events
        foreach (var failEv in failEvents)
        {
            updatedPlan.Add(new EventPlanItem
            {
                eventName = failEv.eventName + " (Fail)",
                isFailChain = true
            });
        }
        
        // Add events after the insertion point
        for (int i = insertIndex; i < eventPlan.Count; i++)
        {
            updatedPlan.Add(eventPlan[i]);
        }
        
        // Replace the event plan
        eventPlan = updatedPlan;
    }

    private void TriggerEvent(CarInteractionEventSO carEvent)
    {
        if (debugMode) Debug.Log($"Event Triggered: {carEvent.eventName}");
        
        // Call the event's TriggerEvent method with the event origin
        if (carEvent != null)
        {
            carEvent.TriggerEvent(eventOrigin);
        }
    }

    public void OnPlayerInteracted()
    {
        if (waitingForInteraction && currentInteractionEvent != null)
        {
            waitingForInteraction = false;
            TriggerEvent(currentInteractionEvent);
            currentInteractionEvent = null;
        }
    }

    private void ShowInteractionPrompt(string prompt)
    {
        if (debugMode) Debug.Log($"Prompt: {prompt}");
        // Buraya gerçek bir UI prompt gösterebilirsin.
    }
}