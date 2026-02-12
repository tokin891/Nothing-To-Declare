using System;
using UnityEngine;
using System.Collections.Generic;

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

    [Header("Settings")]
    public Material normalMaterial;

    [Header("Custom Script")]
    [Tooltip("Set action id 'Start' if you want put it on start or actionID from node")]
    public List<NPCBehaviour> npcCustomScript;

    public NPCDataConscript GetConscriptData(Conscript conscript)
    {
        foreach(var cons in AllConscriptsNPC)
        {
            if(cons.humanConscript == conscript)
                { return cons; }
        }

        return null;
    }

    //public void InputAction(string actionID, NPCObject nObject)
    //{
    //    for (int i = 0; i < npcCustomScript.Count; i++)
    //    {
    //        if (npcCustomScript[i].actionID == actionID)
    //            npcCustomScript[i].Execute(nObject);
    //    }

    //    for (int i = 0; i < AllConscriptsNPC.Length; i++)
    //    {
    //        for (int j = 0; j < AllConscriptsNPC[i].npcConscriptCustomScripts.Count; j++)
    //        {
    //            if (AllConscriptsNPC[i].npcConscriptCustomScripts[j].actionID == actionID)
    //                AllConscriptsNPC[i].npcConscriptCustomScripts[j].Execute(nObject);
    //        }
    //    }
    //}
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
    Bad,
    Killer
}

// Test
[Serializable]
public abstract class NPCBehaviour:ScriptableObject
{
    public string actionID;
    public abstract void Execute(NPCObject npc);
}