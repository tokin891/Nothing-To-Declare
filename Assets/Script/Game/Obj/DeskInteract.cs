using System.Collections;
using UnityEngine;

public class DeskInteract : MonoBehaviour, IInteract
{
    [SerializeField] float waitForCandidant = 5;

    public void Interact()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        MovementManager.instance.SwitchState(MovementManager.instance.DeskState, true);
        yield return new WaitForSeconds(waitForCandidant);
        NPCManager.Instance.Initialize();
    }
}
