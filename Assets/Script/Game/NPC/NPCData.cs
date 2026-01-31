using System;
using UnityEngine;

[CreateAssetMenu(menuName ="NPC/Create NPC")]
public class NPCData : ScriptableObject
{
    [Header("Inforamtions")]
    public Sprite humanFace;
    public string humanName;
    public string humanSecondName;
    public string humanAge;
    public string humanWeight;
    public string humanHeight = "180";
    public string humanEyesColor = "black";
    public string humanDate = "2000.01.01";
    public string humanGender;
    public string humanAdditionalInfo;
    public CountryHuman humanCountry;
    public int humanID;

    public NPCDataConscript[] AllConscriptsNPC;

    public NPCDataConscript GetConscriptData(Conscript conscript)
    {
        foreach(var cons in AllConscriptsNPC)
        {
            if(cons.humanConscript == conscript)
                { return cons; }
        }

        return null;
    }
}

[Serializable, CreateAssetMenu(menuName ="NPC/Create Conscript NPC")]
public class NPCDataConscript:ScriptableObject
{
    public NPCData NPCData;
    public Conscript humanConscript;

    [Header("Interaction")]
    public Sprite armSprite;

    [Header("Dialogue")]
    public DialogueTree dialogueTree;
}

public enum CountryHuman
{
    Poland,
    America,
    Russia
}

public enum Conscript
{
    Good,
    Bad
}
