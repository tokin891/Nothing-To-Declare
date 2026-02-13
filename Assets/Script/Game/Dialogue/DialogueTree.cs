using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "Dialogue/DialogueTree", fileName ="DT NPC_NAME")]
public class DialogueTree : NodeGraph
{
    public NPCData NPCData;
    public List<DialogueNode> nodes;
}
