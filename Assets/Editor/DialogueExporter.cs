using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueTree))]
public class DialogueExporter : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DialogueTree tree = (DialogueTree)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Export to Excel (CSV)"))
        {
            ExportToCSV(tree);
        }

        if (GUILayout.Button("Import from Excel (CSV)"))
        {
            ImportFromCSV(tree);
        }
    }

    private void ExportToCSV(DialogueTree tree)
    {
        string path = EditorUtility.SaveFilePanel("Save Dialogue Export", "", tree.name + "_Export.csv", "csv");
        if (string.IsNullOrEmpty(path)) return;

        StringBuilder sb = new StringBuilder();
        // Nag³ówki kolumn
        sb.AppendLine("Node ID;Type;Text PL;Text EN;Action ID");

        foreach (var node in tree.nodes)
        {
            if (node is DialogueNode dNode)
            {
                // Export kwestii NPC
                if (dNode.npcLine != null)
                {
                    foreach (var line in dNode.npcLine)
                    {
                        sb.AppendLine($"{dNode.name};NPC;{line.pl};{line.en};{dNode.actionId}");
                    }
                }

                // Export opcji gracza
                if (dNode.options != null)
                {
                    foreach (var option in dNode.options)
                    {
                        sb.AppendLine($"{dNode.name};Player;{option.playerLine.pl};{option.playerLine.en};-");
                    }
                }
            }
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();
        Debug.Log("Dialogi wyeksportowane do: " + path);
    }

    private void ImportFromCSV(DialogueTree tree)
    {
        string path = EditorUtility.OpenFilePanel("Select Dialogue CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);

        // Zaczynamy od 1, ¿eby pomin¹æ nag³ówki
        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(';'); // Upewnij siê, ¿e separator jest taki sam jak przy eksporcie
            if (data.Length < 4) continue;

            string nodeName = data[0];
            string type = data[1];
            string newTextPl = data[2];
            string newTextEn = data[3];

            // Szukamy wêz³a w grafie o tej nazwie
            DialogueNode node = tree.nodes.Find(x => x.name == nodeName) as DialogueNode;

            if (node != null)
            {
                Undo.RecordObject(node, "Import CSV Data"); // Pozwala na Cofnij (Ctrl+Z)

                if (type == "NPC")
                {
                    // W tym prostym przyk³adzie podmieniamy pierwsz¹ liniê NPC
                    if (node.npcLine != null && node.npcLine.Count > 0)
                    {
                        node.npcLine[0].pl = newTextPl;
                        node.npcLine[0].en = newTextEn;
                    }
                }
                else if (type == "Player")
                {
                    // Szukamy opcji gracza, która pasuje do tekstu (lub indeksu)
                    foreach (var option in node.options)
                    {
                        // Jeœli tekst siê zgadza lub jest to jedyna opcja
                        option.playerLine.pl = newTextPl;
                        option.playerLine.en = newTextEn;
                    }
                }
                EditorUtility.SetDirty(node);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Import zakoñczony pomyœlnie!");
    }

    // ... tutaj zostaje poprzednia metoda ExportToCSV ...
}