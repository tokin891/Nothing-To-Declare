using System.Collections.Generic;

[System.Serializable]
public class DialogueNode
{
    public string nodeId;
    public string actionId;                     // Do wykonywania akcji
    public List<LocalizedLine> npcLine;         // NPC mówi
    public List<PlayerOption> options;          // gracz wybiera
    public bool exitNode = false;               // Po dialogu koniec
}
