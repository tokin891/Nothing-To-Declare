using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private Dictionary<string, int> savedRespectModifier;
    private Dictionary<string, NpcSaveEntry> saveNpcs;
    private List<QueueNpcsSave> allQueuesNpc;

    private string SavePath => Path.Combine(Application.persistentDataPath, "ntd_save_playerStats.json");

    public int RespectTotal { set; get; }

    private void Awake()
    {
        savedRespectModifier = new Dictionary<string, int>();
        savedRespectModifier.Clear();
        saveNpcs = new Dictionary<string, NpcSaveEntry>();
        saveNpcs.Clear();
        allQueuesNpc = new List<QueueNpcsSave>();
        allQueuesNpc.Clear();
    }

    #region Methods

    public void TrySaveTotalRespectByOptionEffect(NPCData npcData, Conscript cons, OptionEffect optionEffect)
    {
        string save = GetRespectModifierKey(npcData, cons, optionEffect.optionID);

        if (HasSavedRespecModifierByOptionId(optionEffect, npcData, cons))
            return;

        NpcSaveEntry nse = GetNpcSaveByData(npcData);
        if (nse == null)
        {
            Debug.Log("Theres no Npc save to attach respect modifier");
            return;
        }

        nse.KeyRespect.Add(save);
        nse.ValueRespect.Add(optionEffect.respectModifier);
        savedRespectModifier.Add(save, optionEffect.respectModifier);
        RespectTotal += optionEffect.respectModifier;

        Debug.Log("Save :" + "'" + save + "' and value: " + optionEffect.respectModifier.ToString());
    }
    public void TryGenereteSaveNpc(NPCData npcData, Conscript con)
    {
        string key = GetNpcSaveKey(npcData, con);
        if (!saveNpcs.ContainsKey(key))
        {
            saveNpcs[key] = new NpcSaveEntry
            {
                HasVisited = false
            };
        }
        else
            Debug.Log("Npc was saved");
    }
    public void TryToSaveNpcVisited(NPCData npcData, Conscript con)
    {
        if (saveNpcs.TryGetValue(GetNpcSaveKey(npcData, con), out NpcSaveEntry entry))
        {
            entry.HasVisited = true;
        }
        else
            Debug.Log("No npc found to save");
    }
    public void GenereteSaveQueueNpc(List<DayNpcData> dataQueueNpcs, int day)
    {
        if (allQueuesNpc == null)
            return;
        foreach (var queue in allQueuesNpc)
        {
            if (queue.QueueDay == day)
            {
                Debug.Log("Queue for day " + day + " exists");
                return;
            }
        }

        List<string> allKeysForNpc = new List<string>();
        foreach (var k in dataQueueNpcs)
            allKeysForNpc.Add(GetNpcSaveKey(k.npc, k.selectConscript.humanConscript));

        QueueNpcsSave queueNpcs = new QueueNpcsSave
        {
            QueueDay = day,
            NpcKeys = allKeysForNpc
        };
        allQueuesNpc.Add(queueNpcs);
    }

    public int TryGetRespectFromNpcData(NPCData data, Conscript cons)
    {
        var matches = savedRespectModifier.Where(kvp =>
        kvp.Key.StartsWith(GetRespectModifierKey(data, cons, "")));
        int respectOfNpc = 0;

        foreach (var match in matches)
        {
            respectOfNpc += match.Value;
        }

        return respectOfNpc;
    }
    public bool TryCheckIsHumanWasSaved(NPCData npcData)
    {
        for (int i = 0; i < npcData.AllConscriptsNPC.Length; i++)
        {
            if (saveNpcs.ContainsKey(GetNpcSaveKey(npcData, npcData.AllConscriptsNPC[i].humanConscript)))
                return true;
        }

        return false;
    }
    public bool TryCheckIsHumanWasVisited(NPCData npcData)
    {
        for (int i = 0; i < npcData.AllConscriptsNPC.Length; i++)
        {
            if (saveNpcs.ContainsKey(GetNpcSaveKey(npcData, npcData.AllConscriptsNPC[i].humanConscript)))
                return saveNpcs[GetNpcSaveKey(npcData, npcData.AllConscriptsNPC[i].humanConscript)].HasVisited;
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

            SaveNpcs = saveNpcs.Values.ToList(),
            KeySaveNpcs = saveNpcs.Keys.ToList(),

            AllQueuesNpc = allQueuesNpc,

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
        saveNpcs.Clear();
        allQueuesNpc.Clear();
        for (int i = 0; i < data.SaveNpcs.Count; i++)
        {
            for (int j = 0; j < data.SaveNpcs[i].KeyRespect.Count; j++)
            {
                savedRespectModifier.Add(data.SaveNpcs[i].KeyRespect[j], data.SaveNpcs[i].ValueRespect[j]);
            }
        }
        if (data.KeySaveNpcs.Count != data.SaveNpcs.Count)
        {
            Debug.LogError("Save file corrupted: mismatched NPC keys and entries.");
            return;
        }
        for (int i = 0; i < data.SaveNpcs.Count; i++)
        {
            saveNpcs.Add(data.KeySaveNpcs[i],
                data.SaveNpcs[i]);
        }
        allQueuesNpc = data.AllQueuesNpc;

        //GameManager.Instance.MovementManager.SwitchState(
        //    GetStateFromString(data.CurrentState));
        //Debug.Log(data.CurrentState);

        RespectTotal = data.RespectTotal;
        Debug.Log("Wczytano zapis.");
    }

    #endregion

    #region Logic

    private bool HasSavedRespecModifierByOptionId(OptionEffect optionEffect, NPCData npcData, Conscript cons)
    {
        var entry = GetNpcSaveByData(npcData);
        if (entry == null) 
            return false; 
        return entry.KeyRespect.Contains(GetRespectModifierKey(npcData, cons, optionEffect.optionID));
    }

    private NpcSaveEntry GetNpcSaveByData(NPCData data)
    {
        foreach (var cons in data.AllConscriptsNPC)
        {
            string key = SaveKeyGenerator.ForNpc(data, cons.humanConscript);

            if (saveNpcs.TryGetValue(key, out var entry))
                return entry;
        }

        return null;
    }

    #endregion

    public string GetNpcSaveKey(NPCData npcd, Conscript con) => SaveKeyGenerator.ForNpc(npcd, con);
    private string GetRespectModifierKey(NPCData data, Conscript cons, string option) => SaveKeyGenerator.ForRespect(data, cons, option);

    private string GetMovementStateString(IStateMovement state)
    {
        var mm = GameManager.Instance.MovementManager;
        if (state == mm.DeskState) return "Desk"; 
        if (state == mm.ShootState) return "Shoot"; 
        if (state == mm.MoveState) return "Move"; 
        return "";
    }
    private IStateMovement GetStateFromString(string state)
    {
        var mm = GameManager.Instance.MovementManager;
        if (state == "Desk")
            return mm.DeskState;
        if (state == "Shoot")
            return mm.ShootState;
        if (state == "Move")
            return mm.MoveState;

        return mm.MoveState;
    }
}

public class SaveData
{
    public int RespectTotal;

    public List<NpcSaveEntry> SaveNpcs;
    public List<string> KeySaveNpcs;

    public List<QueueNpcsSave> AllQueuesNpc;

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

    public bool HasVisited;

    public List<string> KeyRespect;
    public List<int> ValueRespect;
}

[System.Serializable]
public class QueueNpcsSave
{
    public int QueueDay;
    public List<string> NpcKeys;
}