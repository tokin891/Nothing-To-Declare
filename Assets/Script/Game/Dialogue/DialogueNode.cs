using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("Dialogue/Node")]
public class DialogueNode : Node
{
    [Input] public bool entry;

    public string actionId;
    public List<LocalizedLine> npcLine;

    [Output(dynamicPortList = true)] public List<PlayerOption> options;

    public bool exitNode = false;

    // Ta funkcja sprawi, ¿e okienko bêdzie mia³o nazwê z nodeId
    protected override void Init()
    {
        base.Init();
        if (string.IsNullOrEmpty(name)) name = "New Dialogue";
    }

    public override object GetValue(NodePort port) => null;

    // W pliku DialogueNode.cs dodaj:
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (npcLine != null && npcLine.Count > 0 && !string.IsNullOrEmpty(npcLine[0].pl))
        {
            name = npcLine[0].pl; // Zmienia nazwê bloczka w grafie na tekst NPC
        }
    }
#endif
}