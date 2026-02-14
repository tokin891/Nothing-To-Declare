using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private Dictionary<string, int> savedRespectModifier;
    private Dictionary<string, bool> visitedNpcs;
    private List<NpcSaveEntry> saveNpcs;

    private string SavePath => Path.Combine(Application.persistentDataPath, "ntd_save_playerStats.json");

    public int RespectTotal { set; get; }

    private void Awake()
    {
        visitedNpcs = new Dictionary<string, bool>();
        visitedNpcs.Clear();
        savedRespectModifier = new Dictionary<string, int>();
        savedRespectModifier.Clear();
        saveNpcs = new List<NpcSaveEntry>();
        saveNpcs.Clear();
    }

    #region Methods

    public void TrySaveTotalRespectByOptionEffect(NPCData npcData, Conscript cons, OptionEffect optionEffect)
    {
        string save = GetRespectModifierKey(npcData, cons, optionEffect.optionID);
        if (!HasSavedRespecModifierByOptionId(optionEffect, npcData, cons))
        {
            NpcSaveEntry nse = GetNpcSaveByData(npcData);
            nse.KeyRespect.Add(save);
            nse.ValueRespect.Add(optionEffect.respectModifier);
            savedRespectModifier.Add(save, optionEffect.respectModifier);
            Debug.Log("Save :" + "'" + save + "' and value: " + optionEffect.respectModifier.ToString());
            RespectTotal += optionEffect.respectModifier;
        }
        else
            Debug.Log("Its been saved or it doesn't exist");
    }

    public int TryGetRespectFromNpcData(NPCData data, Conscript cons)
    {
        var matches = savedRespectModifier.Where(kvp =>
        kvp.Key.StartsWith(GetRespectModifierKey(data, cons,"")));
        int respectOfNpc = 0;

        foreach(var match in matches)
        {
            respectOfNpc += match.Value;
        }

        return respectOfNpc;
    }

    public void TrySaveNpc(NPCData npcData, Conscript con)
    {
        string path = GetNpcSaveKey(npcData, con);
        if (visitedNpcs.ContainsKey(path))
            return;

        NpcSaveEntry nse = new NpcSaveEntry();
        nse.HasVisited = true;
        nse.KeyNpc = GetNpcSaveKey(npcData, con);
        visitedNpcs.Add(path, true);
        saveNpcs.Add(nse);
    }
    public bool CheckIsHumanWasHere(NPCData npcData)
    {
        List<string> allPathToNpc = new List<string>();
        for (int i = 0; i < npcData.AllConscriptsNPC.Length; i++)
        {
            allPathToNpc.Add(GetNpcSaveKey(npcData, npcData.AllConscriptsNPC[i].humanConscript));
        }
        
        foreach (var path in allPathToNpc)
        {
            if (visitedNpcs.ContainsKey(path))
            {
                Debug.Log("human was here");
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Save

    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            RespectTotal = RespectTotal,

            SaveNpcs = saveNpcs,

            Day = GameManager.Instance.NpcFlowController.GetCurrentDay,

            CurrentState = GetMovementStateString(GameManager.Instance.MovementManager.CurrentState)
        };

        string json = JsonUtility.ToJson(data, true);        
        File.WriteAllText(SavePath, json);
    }

    [ContextMenu("Load Game")]
    public void LoadGame()
    {
        if (!File.Exists(SavePath)) return;

        string json = File.ReadAllText(SavePath);

        SaveData data = JsonUtility.FromJson<SaveData>(json);
        savedRespectModifier.Clear();
        visitedNpcs.Clear();
        for (int i = 0; i < data.SaveNpcs.Count; i++)
        {
            for (int j = 0; j < data.SaveNpcs[i].KeyRespect.Count; j++)
            {
                savedRespectModifier.Add(data.SaveNpcs[i].KeyRespect[i], data.SaveNpcs[i].ValueRespect[i]);
            }
        }
        for (int i = 0; i < data.SaveNpcs.Count; i++)
        {
            visitedNpcs.Add(data.SaveNpcs[i].KeyNpc, data.SaveNpcs[i].HasVisited);
        }
        saveNpcs = data.SaveNpcs;

        //GameManager.Instance.MovementManager.SwitchState(
        //    GetStateFromString(data.CurrentState));
        Debug.Log(data.CurrentState);

        RespectTotal = data.RespectTotal;
        Debug.Log("Wczytano zapis.");
    }

    private string GetMovementStateString(IStateMovement state)
    {
        if (state.ToString() == GameManager.Instance.MovementManager.DeskState.ToString())
            return "Desk";
        if (state.ToString() == GameManager.Instance.MovementManager.ShootState.ToString())
            return "Shoot";
        if (state.ToString() == GameManager.Instance.MovementManager.MoveState.ToString())
            return "Move";
        return "";
    }
    private IStateMovement GetStateFromString(string state)
    {
        if (state == "Desk")
            return GameManager.Instance.MovementManager.DeskState;
        if (state == "Shoot")
            return GameManager.Instance.MovementManager.ShootState;
        if (state == "Move")
            return GameManager.Instance.MovementManager.MoveState;

        return GameManager.Instance.MovementManager.MoveState;
    }

    #endregion

    #region Logic

    private bool HasSavedRespecModifierByOptionId(OptionEffect optionEffect, NPCData npcData, Conscript cons)
    {
        if (GetNpcSaveByData(npcData).KeyRespect.Contains(GetRespectModifierKey(npcData, cons, optionEffect.optionID)))
        {
            return true;
        }

        return false;
    }

    private NpcSaveEntry GetNpcSaveByData(NPCData data)
    {
        NpcSaveEntry nse = null;

        foreach(var n in saveNpcs)
        {
            List<string> allPathToNpc = new List<string>();
            for (int i = 0; i < data.AllConscriptsNPC.Length; i++)
            {
                allPathToNpc.Add(SaveKeyGenerator.ForNpc(data, data.AllConscriptsNPC[i].humanConscript));
            }
            foreach(var p in allPathToNpc)
            {
                if (n.KeyNpc == p)
                    return n;
            }
        }

        return nse;
    }

    public string GetNpcSaveKey(NPCData npcd, Conscript con) 
    {
        return SaveKeyGenerator.ForNpc(npcd, con);
    }
    private string GetRespectModifierKey(NPCData data, Conscript cons, string option) => SaveKeyGenerator.ForRespect(data, cons, option);

    #endregion
}

public class SaveData
{
    public int RespectTotal;

    public List<NpcSaveEntry> SaveNpcs;

    public int Day;

    public string CurrentState;
}

[System.Serializable]
public class NpcSaveEntry
{
    public NpcSaveEntry()
    {
        KeyRespect = new List<string>();
        ValueRespect = new List<int>();
    }

    public string KeyNpc;
    public bool HasVisited;
    public List<string> KeyRespect;
    public List<int> ValueRespect;
}