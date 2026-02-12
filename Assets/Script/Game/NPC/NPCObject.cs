using Unity.VisualScripting;
using UnityEngine;

public class NPCObject : MonoBehaviour
{
    public MeshRenderer renderNPC;
    public NPCData NPCData;

    public Vector3 PositionAwakeOfRenderMesh { private set; get; }
    public Transform TransformOfRenderMesh => renderNPC.transform;

    public void Initialize()
    {
        TransformOfRenderMesh.localPosition = Vector3.zero;
        TransformOfRenderMesh.localScale = Vector3.one;

        for (int i = 0; i < NPCData.npcCustomScript.Count; i++)
        {
            if (NPCData.npcCustomScript[i].actionID == "Start")
                NPCData.npcCustomScript[i].Execute(this);
        }

        renderNPC.material = NPCData.normalMaterial;
    }

    private void Awake()
    {
        PositionAwakeOfRenderMesh = renderNPC.transform.localPosition;
    }

    public void ActionNPC(string actionID)
    {
        //if (actionNPC == null)
        //{
        //    if (gameObject.TryGetComponent<IActionNPC>(out IActionNPC npca))
        //    {
        //        actionNPC = npca;
        //    }
        //    else
        //    {
        //        Debug.Log("Theres no action");
        //        return;
        //    }
        //}

        //if (actionNPC == null)
        //{
        //    return;
        //}

        //actionNPC.InputAction(actionID);

        for (int i = 0; i < NPCData.npcCustomScript.Count; i++)
        {
            if (NPCData.npcCustomScript[i].actionID == actionID)
                NPCData.npcCustomScript[i].Execute(this);
        }
    }
}
