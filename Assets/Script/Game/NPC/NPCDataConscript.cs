using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "NPC/Create Conscript NPC", fileName ="NPC_Name Conscript")]
public class NPCDataConscript : ScriptableObject
{
    public NPCData NPCData;
    public Conscript humanConscript;

    [Header("Interaction")]
    public Sprite armSprite;
    public Sprite eyeSprite;

    [Header("Dialogue")]
    public DialogueTree dialogueTree;

    [Header("Custom script")]
    public List<NPCBehaviour> npcConscriptCustomScripts;
}