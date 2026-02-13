using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RevolverPickup : PickableObjectBehaviour
{
    public override void OnClickSuccesfully()
    {
        if (!GameManager.Instance.IsNpcInOffice)
        {
            Debug.Log("Theres no npc to shoot");
            return;
        }

        GameManager.Instance.MovementManager.SwitchState(GameManager.Instance.MovementManager.ShootState, true);
    }
}
