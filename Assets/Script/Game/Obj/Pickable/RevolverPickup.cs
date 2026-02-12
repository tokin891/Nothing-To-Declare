using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RevolverPickup : PickableObjectBehaviour
{
    public override void OnClickSuccesfully()
    {
        GameManager.Instance.MovementManager.SwitchState(GameManager.Instance.MovementManager.ShootState, true);
    }
}
