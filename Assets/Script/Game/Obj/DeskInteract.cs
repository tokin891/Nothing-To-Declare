using System.Collections;
using UnityEngine;

public class DeskInteract : MonoBehaviour, IInteract
{
    [SerializeField] float waitForCandidant = 5;

    public void Interact()
    {
        GameManager.Instance.MovementManager.SwitchState(GameManager.Instance.MovementManager.DeskState, true);
        GameManager.Instance.NpcFlowController.Initialize();
    }
}
