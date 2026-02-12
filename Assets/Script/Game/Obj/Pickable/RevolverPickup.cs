using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RevolverPickup : PickableObjectBehaviour
{
    public override void OnClickSuccesfully()
    {
        MovementManager.instance.SwitchState(MovementManager.instance.ShootState, true);
    }
}
