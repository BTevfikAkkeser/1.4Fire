using UnityEngine;

[CreateAssetMenu(fileName = "NewWinEvent", menuName = "Car Events/Win Event")]
public class WinEventSO : CarInteractionEventSO
{
    public virtual void TriggerEvent(Transform eventOrigin)
    {
        CarVFXHandler vfxHandler = eventOrigin.GetComponent<CarVFXHandler>();
        if (vfxHandler != null)
        {
            vfxHandler.HandleWin();
        }
    }
}
