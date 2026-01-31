using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueTree")]
public class DialogueTree : ScriptableObject
{
    public NPCData NPCData;
    public List<DialogueNode> nodes;

    public DialogueNode GetNode(string id)
    {
        return nodes.Find(n => n.nodeId == id);
    }
}
