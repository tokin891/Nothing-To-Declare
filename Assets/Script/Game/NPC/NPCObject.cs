using Unity.VisualScripting;
using UnityEngine;

public class NPCObject : MonoBehaviour
{
    public NPCData NPCDatas;

    private IActionNPC actionNPC;

    public void ActionNPC(string actionID)
    {
        if (actionNPC == null)
        {
            if (gameObject.TryGetComponent<IActionNPC>(out IActionNPC npca))
            {
                actionNPC = npca;
            }
            else
            {
                Debug.Log("Theres no action");
                return;
            }
        }

        if (actionNPC == null)
        {
            return;
        }

        actionNPC.InputAction(actionID);
    }
}
