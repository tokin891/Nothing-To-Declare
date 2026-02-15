using System.Collections;
using UnityEngine;

public class DeskInteract : MonoBehaviour, IInteract
{
    public void Interact()
    {
        GameManager.Instance.MovementManager.SwitchState(GameManager.Instance.MovementManager.DeskState, true);
        GameManager.Instance.NpcFlowController.Initialize();
    }
}
