using UnityEngine;

[CreateAssetMenu(fileName = "NewCrashEvent", menuName = "Car Events/Crash Event")]
public class CrashEventSO : CarInteractionEventSO
{
    public virtual void TriggerEvent(Transform eventOrigin)
    {
        CarVFXHandler vfxHandler = eventOrigin.GetComponent<CarVFXHandler>();
        if (vfxHandler != null)
        {
            vfxHandler.HandleCrash();
        }
    }
}
