using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCarInteractionEvent", menuName = "RNG/Car Interaction Event")]
public class CarInteractionEventSO : ScriptableObject
{
    [Header("Basic Info")]
    public string eventName;
    public AudioClip soundClip;    // Ses klibi
    public ParticleSystem vfx;     // VFX (Visual Effects)

    [Header("Target Settings")]
    public GameObject targetObject;  // Reference to the target object (car, etc.)

    public float delayAfterPrevious = 0f;
    
    /// <summary>
    /// Trigger this event with the specified event origin transform
    /// </summary>
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

       if (targetObject != null)
        {
            DriftController driftController = targetObject.GetComponent<DriftController>();
            if (driftController != null)
            {
                // Instead of calling ActivateHandbrake, directly set isBreaking
                driftController.isBreaking = true;
                
                // Find any MonoBehaviour to start a coroutine
                MonoBehaviour mb = Object.FindObjectOfType<MonoBehaviour>();
                if (mb != null)
                {
                    mb.StartCoroutine(ReleaseHandbrake(driftController, 3.0f));
                }
            }
        }

        // Add this method to your class
        System.Collections.IEnumerator ReleaseHandbrake(DriftController controller, float duration)
        {
            yield return new WaitForSeconds(duration);
            controller.isBreaking = false;
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