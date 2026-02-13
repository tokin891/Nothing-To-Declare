using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class NPCObject : MonoBehaviour
{
    public MeshRenderer renderNPC;
    [HideInInspector]
    public NPCData NpcData;

    public Vector3 PositionAwakeOfRenderMesh { private set; get; }
    public Transform TransformOfRenderMesh => renderNPC.transform;

    private Dictionary<string, List<NPCBehaviour>> actionDictionary = new Dictionary<string, List<NPCBehaviour>>();

    public void Initialize()
    {
        TransformOfRenderMesh.localPosition = Vector3.zero;
        TransformOfRenderMesh.localScale = Vector3.one;
        PositionAwakeOfRenderMesh = TransformOfRenderMesh.localPosition;

        for (int i = 0; i < NpcData.npcCustomScript.Count; i++)
        {
            if (NpcData.npcCustomScript[i].actionID == "Start")
                NpcData.npcCustomScript[i].Execute(this);
        }

        BuildPreviewDictionary(GameManager.Instance.GetCurrentNpcData.GetConscriptData(GameManager.Instance.CurrentConscript));
        renderNPC.material = NpcData.normalMaterial;
    }

    private void BuildPreviewDictionary(NPCDataConscript conscriptData)
    {
        actionDictionary.Clear();

        if (NpcData.npcCustomScript != null)
            AddListToDictionary(NpcData.npcCustomScript);

        if (conscriptData != null && conscriptData.npcConscriptCustomScripts != null)
            AddListToDictionary(conscriptData.npcConscriptCustomScripts);
    }

    private void AddListToDictionary(List<NPCBehaviour> list)
    {
        foreach (var behaviour in list)
        {
            if (behaviour == null) continue;

            if (!actionDictionary.ContainsKey(behaviour.actionID))
            {
                actionDictionary[behaviour.actionID] = new List<NPCBehaviour>();
            }
            actionDictionary[behaviour.actionID].Add(behaviour);
        }
    }

    public void ActionNPC(string actionID)
    {
        if (actionDictionary.TryGetValue(actionID, out List<NPCBehaviour> behaviours))
        {
            foreach (var b in behaviours)
            {
                b.Execute(this);
            }
        }
    }
}
