using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("Dialogue/Node")]
public class DialogueNode : Node
{
    // Typ wejœcia musi byæ taki sam jak typ wyjœcia (PlayerOption)
    [Input] public PlayerOption entry;

    public string actionId;
    public List<LocalizedLine> npcLine;

    // KLUCZOWA ZMIANA: Musi byæ [Output] z dynamicPortList
    [Output(dynamicPortList = true)] public List<PlayerOption> options;

    public bool exitNode = false;

    protected override void Init()
    {
        base.Init();
        if (string.IsNullOrEmpty(name)) name = "New Dialogue";
    }

    // Pozwala xNode na obs³ugê po³¹czeñ miêdzy portami
    public override object GetValue(NodePort port) => null;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (npcLine != null && npcLine.Count > 0 && !string.IsNullOrEmpty(npcLine[0].pl))
        {
            name = npcLine[0].pl;
        }
    }
#endif
}