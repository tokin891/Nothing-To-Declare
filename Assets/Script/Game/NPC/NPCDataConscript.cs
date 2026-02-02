using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Create Conscript NPC")]
public class NPCDataConscript : ScriptableObject
{
    public NPCData NPCData;
    public Conscript humanConscript;

    [Header("Interaction")]
    public Sprite armSprite;

    [Header("Dialogue")]
    public DialogueTree dialogueTree;
}