using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "NPC/Create Conscript NPC")]
public class NPCDataConscript : ScriptableObject
{
    public NPCData NPCData;
    public Conscript humanConscript;

    [Header("Interaction")]
    public Sprite armSprite;

    [Header("Dialogue")]
    public DialogueTree dialogueTree;

    [Header("Custom script")]
    public List<NPCBehaviour> npcConscriptCustomScripts;
}