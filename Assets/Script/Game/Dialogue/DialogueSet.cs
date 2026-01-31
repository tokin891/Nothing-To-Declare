using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueSet")]
public class DialogueSet : ScriptableObject
{
    public List<LocalizedLine> lines;

    public string GetLine(string key)
    {
        var line = lines.Find(l => l.key == key);
        return line != null ? line.Get() : $"#{key}";
    }
}
