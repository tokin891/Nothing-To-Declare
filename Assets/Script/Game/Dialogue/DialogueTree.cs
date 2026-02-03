using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "Dialogue/DialogueTree")]
public class DialogueTree : NodeGraph
{
    public NPCData NPCData;
    public List<DialogueNode> nodes;
}
