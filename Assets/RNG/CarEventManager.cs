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

    [Header("Runtime Event Plan (Debug Only)")]
    [SerializeField] private List<EventPlanItem> eventPlan = new List<EventPlanItem>();

    private int currentEventIndex = 0;
    private bool waitingForInteraction = false;
    private bool isFailChainActive = false;

    private void Start()
    {
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
                Debug.Log($"RNG Roll for {currentEvent.eventName}: {roll}");

                if (roll > currentEvent.chanceToTrigger)
                {
                    Debug.Log($"Event {currentEvent.eventName} FAILED RNG check, switching to Fail Chain.");

                    if (currentEvent.failEvents != null && currentEvent.failEvents.Count > 0)
                    {
                        InsertFailChainIntoPlan(currentEvent.failEvents);
                        StartCoroutine(PlayFailEvents(currentEvent.failEvents));
                        yield break;
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

        Debug.Log("Event Sequence Finished.");
    }

    private IEnumerator PlayFailEvents(List<CarInteractionEventSO> failEvents)
    {
        Debug.Log("Playing Fail Event Chain!");

        isFailChainActive = true;

        foreach (var failEvent in failEvents)
        {
            yield return new WaitForSeconds(failEvent.delayAfterPrevious);

            if (failEvent.waitForInteraction)
            {
                waitingForInteraction = true;
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
        }

        Debug.Log("Fail Event Chain Finished.");
    }

    private void InsertFailChainIntoPlan(List<CarInteractionEventSO> failEvents)
    {
        foreach (var failEv in failEvents)
        {
            eventPlan.Add(new EventPlanItem
            {
                eventName = failEv.eventName + " (Fail)",
                isFailChain = true
            });
        }
    }

    private void TriggerEvent(CarInteractionEventSO carEvent)
    {
        Debug.Log($"Event Triggered: {carEvent.eventName}");
    }

    public void OnPlayerInteracted()
    {
        if (waitingForInteraction)
        {
            waitingForInteraction = false;

            if (!isFailChainActive)
            {
                CarInteractionEventSO currentEvent = eventSequence[currentEventIndex];
                if (currentEvent != null)
                    TriggerEvent(currentEvent);
            }
        }
    }

    private void ShowInteractionPrompt(string prompt)
    {
        Debug.Log($"Prompt: {prompt}");
        // Buraya gerçek bir UI prompt gösterebilirsin.
    }
}
