using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCarInteractionEvent", menuName = "RNG/Car Interaction Event")]
public class CarInteractionEventSO : ScriptableObject
{
    [Header("Basic Info")]
    public string eventName;
    public AudioClip soundClip;    // Ses klibi
    public ParticleSystem vfx;     // VFX (Visual Effects)

    private DriftController driftController;

    public void TriggerEvent(Transform eventOrigin)
    {
        // Eğer event'te ses varsa, ses kaynağını oynat
        if (soundClip != null)
        {
            AudioSource audioSource = eventOrigin.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(soundClip);  // Ses oynatılır
            }
        }

        // Eğer VFX varsa, VFX tetiklenir
        if (vfx != null)
        {
            ParticleSystem newVFX = Instantiate(vfx, eventOrigin.position, Quaternion.identity);
            newVFX.Play();
        }

        // Eğer el freni eventi ise, DriftController üzerinden el freni aktif edilir
        if (targetObject != null)
        {
            driftController = targetObject.GetComponent<DriftController>();
            if (driftController != null)
            {
               // driftController.m_MaxHandbrakeTorque();  // El freni etkinleştirilir
            }
        }
    }

    [Header("Interaction Settings")]
    public bool waitForInteraction;
    public string interactionPrompt;

    [Header("Random Chance Settings")]
    public bool useRandomChance;
    [Range(0f, 1f)] public float chanceToTrigger = 1f;

    [Header("Fail Event Chain")]
    public List<CarInteractionEventSO> failEvents;
}
