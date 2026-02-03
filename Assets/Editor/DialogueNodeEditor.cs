using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(DialogueNode))]
public class DialogueNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        DialogueNode node = target as DialogueNode;
        serializedObject.Update();

        // 1. Port Wejœciowy
        NodeEditorGUILayout.PortField(new GUIContent("Wejœcie (Entry)"), node.GetPort("entry"));
        EditorGUILayout.Space();

        // 2. Sekcja NPC
        EditorGUILayout.LabelField("KWESTIE NPC", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("helpbox");

        SerializedProperty npcLinesProp = serializedObject.FindProperty("npcLine");

        // SPRAWDZENIE: Czy property w ogóle istnieje w klasie?
        if (npcLinesProp != null)
        {
            for (int i = 0; i < npcLinesProp.arraySize; i++)
            {
                SerializedProperty lineProp = npcLinesProp.GetArrayElementAtIndex(i);

                // Bezpieczne pobieranie podgl¹du tekstu
                string previewText = "Pusta kwestia...";
                if (node.npcLine != null && i < node.npcLine.Count && node.npcLine[i] != null)
                {
                    if (!string.IsNullOrEmpty(node.npcLine[i].pl))
                        previewText = node.npcLine[i].pl;
                }

                EditorGUILayout.BeginHorizontal();
                // U¿ywamy lineProp zamiast bezpoœredniego obiektu, by unikn¹æ nulli
                EditorGUILayout.PropertyField(lineProp, new GUIContent($"{i}: {previewText}"), true);

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    npcLinesProp.DeleteArrayElementAtIndex(i); // U¿ywamy SerializedProperty do usuwania
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUILayout.Button("Dodaj kwestiê NPC"))
        {
            if (node.npcLine == null) node.npcLine = new List<LocalizedLine>(); // Inicjalizacja jeœli null
            node.npcLine.Add(new LocalizedLine());
            EditorUtility.SetDirty(node); // Powiadomienie Unity o zmianie
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("actionId"), new GUIContent("ID Akcji"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("exitNode"), new GUIContent("Koniec Dialogu?"));

        // 3. Sekcja Opcji Gracza (ju¿ dopracowana)
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("OPCJE GRACZA", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        for (int i = 0; i < node.options.Count; i++)
        {
            SerializedProperty optionProp = serializedObject.FindProperty("options").GetArrayElementAtIndex(i);

            string labelName = (node.options[i].playerLine != null && !string.IsNullOrEmpty(node.options[i].playerLine.pl))
                ? "Opcja: " + node.options[i].playerLine.pl
                : "Opcja " + i;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(optionProp, new GUIContent(labelName), true);

            NodePort dynamicPort = node.GetOutputPort("options " + i);
            if (dynamicPort != null)
            {
                NodeEditorGUILayout.PortField(GUIContent.none, dynamicPort, GUILayout.Width(20));
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                node.options.RemoveAt(i);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Dodaj now¹ opcjê gracza"))
        {
            node.options.Add(new PlayerOption());
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}