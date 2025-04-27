using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCarInteractionEvent", menuName = "RNG/Car Interaction Event")]
public class CarInteractionEventSO : ScriptableObject
{
    [Header("Basic Info")]
    public string eventName;
    public float delayAfterPrevious = 1f;

    [Header("Interaction Settings")]
    public bool waitForInteraction;
    public string interactionPrompt;

    [Header("Random Chance Settings")]
    public bool useRandomChance;
    [Range(0f, 1f)] public float chanceToTrigger = 1f;

    [Header("Fail Event Chain")]
    public List<CarInteractionEventSO> failEvents;
}
