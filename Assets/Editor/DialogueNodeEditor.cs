//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using XNode;
//using XNodeEditor;

//[CustomNodeEditor(typeof(DialogueNode))]
//public class DialogueNodeEditor : NodeEditor
//{
//    public override void OnBodyGUI()
//    {
//        DialogueNode node = target as DialogueNode;
//        if (node == null) return;

//        serializedObject.Update();

//        // 1. Wejœcie (Zawsze u góry)
//        NodeEditorGUILayout.PortField(new GUIContent("Wejœcie (Entry)"), node.GetPort("entry"));
//        EditorGUILayout.Space();

//        // 2. Sekcja NPC
//        EditorGUILayout.LabelField("KWESTIE NPC", EditorStyles.boldLabel);
//        EditorGUILayout.BeginVertical("helpbox");
//        SerializedProperty npcLinesProp = serializedObject.FindProperty("npcLine");
//        if (npcLinesProp != null)
//        {
//            for (int i = 0; i < npcLinesProp.arraySize; i++)
//            {
//                SerializedProperty lineProp = npcLinesProp.GetArrayElementAtIndex(i);

//                // Pobieranie podgl¹du tekstu
//                string preview = "Pusta kwestia...";
//                if (node.npcLine != null && i < node.npcLine.Count && node.npcLine[i] != null)
//                    preview = !string.IsNullOrEmpty(node.npcLine[i].pl) ? node.npcLine[i].pl : preview;

//                EditorGUILayout.BeginHorizontal();
//                EditorGUILayout.PropertyField(lineProp, new GUIContent($"{i}: {preview}"), true);
//                if (GUILayout.Button("X", GUILayout.Width(20)))
//                {
//                    npcLinesProp.DeleteArrayElementAtIndex(i);
//                    serializedObject.ApplyModifiedProperties();
//                    return;
//                }
//                EditorGUILayout.EndHorizontal();
//            }
//        }
//        if (GUILayout.Button("Dodaj kwestiê NPC"))
//        {
//            if (node.npcLine == null) node.npcLine = new List<LocalizedLine>();
//            node.npcLine.Add(new LocalizedLine());
//        }
//        EditorGUILayout.EndVertical();

//        EditorGUILayout.Space();
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("actionId"), new GUIContent("ID Akcji"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("exitNode"), new GUIContent("Koniec Dialogu?"));

//        // 3. Sekcja Opcji Gracza - PRZEBUDOWANY UK£AD
//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("OPCJE GRACZA", EditorStyles.boldLabel);
//        EditorGUILayout.BeginVertical("box");

//        SerializedProperty optionsProp = serializedObject.FindProperty("options");
//        if (optionsProp != null)
//        {
//            for (int i = 0; i < optionsProp.arraySize; i++)
//            {
//                SerializedProperty optionProp = optionsProp.GetArrayElementAtIndex(i);

//                // Pobieranie labelName
//                string labelName = "Opcja " + i;
//                SerializedProperty playerLineProp = optionProp.FindPropertyRelative("playerLine");
//                if (playerLineProp != null)
//                {
//                    SerializedProperty plProp = playerLineProp.FindPropertyRelative("pl");
//                    if (plProp != null && !string.IsNullOrEmpty(plProp.stringValue))
//                        labelName = "Opcja: " + plProp.stringValue;
//                }

//                EditorGUILayout.BeginHorizontal();

//                // PRZYCISK X NA POCZ¥TKU - Nie zas³ania ju¿ niczego
//                if (GUILayout.Button("X", GUILayout.Width(20)))
//                {
//                    optionsProp.DeleteArrayElementAtIndex(i);
//                    serializedObject.ApplyModifiedProperties();
//                    return;
//                }

//                // PORT WYJŒCIOWY (Kó³eczko)
//                NodePort dynamicPort = node.GetOutputPort("options " + i);
//                if (dynamicPort != null)
//                {
//                    NodeEditorGUILayout.PortField(GUIContent.none, dynamicPort, GUILayout.Width(20));
//                }

//                // TREŒÆ OPCJI (Input)
//                EditorGUILayout.PropertyField(optionProp, new GUIContent(labelName), true);

//                EditorGUILayout.EndHorizontal();
//                GUILayout.Space(2);
//            }
//        }

//        if (GUILayout.Button("Dodaj now¹ opcjê gracza"))
//        {
//            if (node.options == null) node.options = new List<PlayerOption>();
//            node.options.Add(new PlayerOption());
//            EditorUtility.SetDirty(node);
//        }
//        EditorGUILayout.EndVertical();

//        serializedObject.ApplyModifiedProperties();
//    }
//}