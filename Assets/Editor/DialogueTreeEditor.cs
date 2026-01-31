using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(DialogueTree))]
public class DialogueTreeEditor : Editor
{
    private DialogueTree tree;
    private string search = "";
    private bool showNodes = true;
    private bool showGraph = true;
    private string selectedNodeId = null;

    private Dictionary<DialogueNode, bool> foldoutNpc = new();
    private Dictionary<DialogueNode, bool> foldoutOptions = new();

    private void OnEnable()
    {
        tree = (DialogueTree)target;

        if (tree.nodes == null)
            tree.nodes = new List<DialogueNode>();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dialogue Tree Editor", EditorStyles.boldLabel);

        DrawSearchBar();
        DrawGraphView();
        DrawNodeList();
        DrawButtons();

        if (GUI.changed)
            EditorUtility.SetDirty(tree);
    }

    // -------------------------
    // SORT HELPERS
    // -------------------------

    private int ExtractNumber(string id)
    {
        if (string.IsNullOrEmpty(id))
            return 0;

        var digits = new string(id.Where(char.IsDigit).ToArray());
        return string.IsNullOrEmpty(digits) ? 0 : int.Parse(digits);
    }

    private string GetPrefix(string id)
    {
        if (string.IsNullOrEmpty(id))
            return "";

        int underscore = id.IndexOf('_');
        return underscore > 0 ? id[..underscore] : id;
    }

    // -------------------------
    // SEARCH BAR
    // -------------------------

    private void DrawSearchBar()
    {
        EditorGUILayout.Space();
        search = EditorGUILayout.TextField("Search Node ID:", search);
        EditorGUILayout.Space();
    }

    // -------------------------
    // GRAPH VIEW (ASCII TREE)
    // -------------------------

    private void DrawGraphView()
    {
        showGraph = EditorGUILayout.Foldout(showGraph, "Dialogue Flow (Tree)");

        if (!showGraph)
            return;

        if (tree.nodes.Count == 0)
        {
            EditorGUILayout.HelpBox("No nodes in this DialogueTree.", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginVertical("box");

        DialogueNode startNode = tree.nodes[0];
        HashSet<string> visited = new HashSet<string>();

        DrawTreeNode(startNode, visited, "", true);

        EditorGUILayout.EndVertical();
    }

    private void DrawTreeNode(DialogueNode node, HashSet<string> visited, string prefix, bool isLast)
    {
        if (node == null)
            return;

        string branch = isLast ? "└── " : "├── ";

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        GUIStyle link = new GUIStyle(EditorStyles.linkLabel);
        link.fontStyle = FontStyle.Bold;

        if (GUILayout.Button(prefix + branch + node.nodeId, link))
            selectedNodeId = node.nodeId;

        GUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(node.actionId))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            EditorGUILayout.LabelField(prefix + "   [Action: " + node.actionId + "]");
            GUILayout.EndHorizontal();
        }

        if (visited.Contains(node.nodeId))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            EditorGUILayout.LabelField(prefix + "   ↺ (loop)");
            GUILayout.EndHorizontal();
            return;
        }

        visited.Add(node.nodeId);

        if (node.exitNode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            EditorGUILayout.LabelField(prefix + "   [EXIT]");
            GUILayout.EndHorizontal();
            return;
        }

        for (int i = 0; i < node.options.Count; i++)
        {
            var opt = node.options[i];
            var next = tree.nodes.Find(n => n.nodeId == opt.nextNodeId);

            bool lastChild = (i == node.options.Count - 1);
            string newPrefix = prefix + (isLast ? "    " : "│   ");

            DrawTreeNode(next, visited, newPrefix, lastChild);
        }
    }

    // -------------------------
    // NODE LIST
    // -------------------------

    private void DrawNodeList()
    {
        showNodes = EditorGUILayout.Foldout(showNodes, $"Nodes ({tree.nodes.Count})");

        if (!showNodes)
            return;

        EditorGUILayout.Space();

        foreach (var node in tree.nodes
            .Where(n => string.IsNullOrEmpty(search) || (n.nodeId != null && n.nodeId.Contains(search)))
            .ToList())
        {
            if (node.npcLine == null)
                node.npcLine = new List<LocalizedLine>();
            if (node.options == null)
                node.options = new List<PlayerOption>();

            Color prevBg = GUI.backgroundColor;
            if (selectedNodeId == node.nodeId)
                GUI.backgroundColor = new Color(0.8f, 0.9f, 1f);

            EditorGUILayout.BeginVertical("box");

            // Node ID + Exit toggle
            EditorGUILayout.BeginHorizontal();
            node.nodeId = EditorGUILayout.TextField("Node ID", node.nodeId);
            node.exitNode = EditorGUILayout.Toggle("Exit Node", node.exitNode);
            EditorGUILayout.EndHorizontal();

            // Action ID
            node.actionId = EditorGUILayout.TextField("Action ID", node.actionId);

            EditorGUILayout.Space();

            // NEXT NODE STATUS
            foreach (var opt in node.options)
            {
                bool exists = tree.nodes.Any(n => n.nodeId == opt.nextNodeId);

                GUI.color = exists ? Color.green : Color.red;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Next: {opt.nextNodeId} {(exists ? "(OK)" : "(MISSING)")}");
                if (exists && GUILayout.Button("Go", GUILayout.Width(40)))
                    selectedNodeId = opt.nextNodeId;
                EditorGUILayout.EndHorizontal();
            }

            GUI.color = Color.white;

            EditorGUILayout.Space();

            // NPC LINES
            if (!foldoutNpc.ContainsKey(node))
                foldoutNpc[node] = false;

            foldoutNpc[node] = EditorGUILayout.Foldout(foldoutNpc[node], "NPC Lines");

            if (foldoutNpc[node])
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < node.npcLine.Count; i++)
                {
                    var line = node.npcLine[i];

                    EditorGUILayout.BeginVertical("box");

                    line.key = EditorGUILayout.TextField("Key", line.key);
                    line.pl = EditorGUILayout.TextField("PL", line.pl);
                    line.en = EditorGUILayout.TextField("EN", line.en);
                    line.de = EditorGUILayout.TextField("DE", line.de);

                    if (GUILayout.Button("Remove NPC Line"))
                    {
                        node.npcLine.RemoveAt(i);
                        break;
                    }

                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("Add NPC Line"))
                    node.npcLine.Add(new LocalizedLine());

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // PLAYER OPTIONS
            if (!foldoutOptions.ContainsKey(node))
                foldoutOptions[node] = false;

            foldoutOptions[node] = EditorGUILayout.Foldout(foldoutOptions[node], "Player Options");

            if (foldoutOptions[node])
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < node.options.Count; i++)
                {
                    var opt = node.options[i];

                    if (opt.playerLine == null)
                        opt.playerLine = new LocalizedLine();

                    EditorGUILayout.BeginVertical("box");

                    opt.playerLine.key = EditorGUILayout.TextField("Key", opt.playerLine.key);
                    opt.playerLine.pl = EditorGUILayout.TextField("PL", opt.playerLine.pl);
                    opt.playerLine.en = EditorGUILayout.TextField("EN", opt.playerLine.en);
                    opt.playerLine.de = EditorGUILayout.TextField("DE", opt.playerLine.de);

                    opt.typeInteraction = (PlayerInteractionType)EditorGUILayout.EnumPopup("Interaction Type", opt.typeInteraction);
                    opt.nextNodeId = EditorGUILayout.TextField("Next Node ID", opt.nextNodeId);

                    if (GUILayout.Button("Remove Option"))
                    {
                        node.options.RemoveAt(i);
                        break;
                    }

                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("Add Player Option"))
                    node.options.Add(new PlayerOption()
                    {
                        playerLine = new LocalizedLine(),
                        nextNodeId = "",
                        typeInteraction = PlayerInteractionType.None
                    });

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // REMOVE NODE
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("REMOVE NODE"))
            {
                tree.nodes.Remove(node);
                GUI.backgroundColor = prevBg;
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
                break;
            }
            GUI.backgroundColor = prevBg;

            // DUPLICATE NODE
            if (GUILayout.Button("Duplicate Node"))
            {
                var clone = new DialogueNode()
                {
                    nodeId = (node.nodeId ?? "node") + "_copy",
                    actionId = node.actionId,
                    exitNode = node.exitNode,
                    npcLine = node.npcLine.Select(l => new LocalizedLine()
                    {
                        key = l.key,
                        pl = l.pl,
                        en = l.en,
                        de = l.de
                    }).ToList(),
                    options = node.options.Select(o => new PlayerOption()
                    {
                        playerLine = new LocalizedLine()
                        {
                            key = o.playerLine.key,
                            pl = o.playerLine.pl,
                            en = o.playerLine.en,
                            de = o.playerLine.de
                        },
                        nextNodeId = o.nextNodeId,
                        typeInteraction = o.typeInteraction
                    }).ToList()
                };

                tree.nodes.Add(clone);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            GUI.backgroundColor = prevBg;
        }
    }

    // -------------------------
    // BOTTOM BUTTONS
    // -------------------------

    private void DrawButtons()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button("Sort Nodes by ID"))
        {
            tree.nodes = tree.nodes
                .OrderBy(n => GetPrefix(n.nodeId))
                .ThenBy(n => ExtractNumber(n.nodeId))
                .ToList();
        }

        if (GUILayout.Button("Add New Node"))
        {
            int nextId = tree.nodes.Count + 1;

            tree.nodes.Add(new DialogueNode()
            {
                nodeId = "node_" + nextId,
                npcLine = new List<LocalizedLine>(),
                options = new List<PlayerOption>()
            });
        }
    }
}
