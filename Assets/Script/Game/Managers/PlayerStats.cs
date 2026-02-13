using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class PlayerStats : MonoBehaviour
{
    private Dictionary<string, int> savedRespectModifier;
    private Dictionary<string, bool> savedHumansWhoWhere;

    private string SavePath => Path.Combine(Application.persistentDataPath, "ntd_save_file.json");

    public int RespectTotal { set; get; }

    public List<string> humanKeys = new List<string>();
    public List<bool> humanValues = new List<bool>();

    private void Awake()
    {
        savedHumansWhoWhere = new Dictionary<string, bool>();
        savedHumansWhoWhere.Clear();
        savedRespectModifier = new Dictionary<string, int>();
        savedRespectModifier.Clear();
    }

    private void Update()
    {
        humanKeys = savedHumansWhoWhere.Keys.ToList();
        humanValues = savedHumansWhoWhere.Values.ToList();
    }

    #region Methods

    public void TrySaveTotalRespectByOptionEffect(NPCData npcData, Conscript cons, OptionEffect optionEffect)
    {
        string save = GetPathToSearchRespectValue(npcData, cons) + optionEffect.optionID;
        if (!HasSavedRespecModifierByOptionId(optionEffect, npcData, cons))
        {
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
        kvp.Key.StartsWith(GetPathToSearchRespectValue(data, cons)));
        int respectOfNpc = 0;

        foreach(var match in matches)
        {
            respectOfNpc += match.Value;
        }

        return respectOfNpc;
    }

    public void TrySaveNpc(NPCData npcData, Conscript con)
    {
        string path = GetPathToNpc(npcData, con);
        if (savedHumansWhoWhere.ContainsKey(path))
            return;

            savedHumansWhoWhere.Add(path, true);
    }
    public bool CheckIsHumanWasHere(NPCData npcData)
    {
        List<string> allPathToNpc = new List<string>();
        for (int i = 0; i < npcData.AllConscriptsNPC.Length; i++)
        {
            allPathToNpc.Add(GetPathToNpc(npcData, npcData.AllConscriptsNPC[i].humanConscript));
        }

        
        foreach (var path in allPathToNpc)
        {
            if (savedHumansWhoWhere.ContainsKey(path))
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
            keys = savedRespectModifier.Keys.ToList(),
            values = savedRespectModifier.Values.ToList(),

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
        for (int i = 0; i < data.keys.Count; i++)
        {
            savedRespectModifier.Add(data.keys[i], data.values[i]);
        }

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

    private OptionEffect SearchForOptionId(DialogueTree tree, NPCData data, Conscript conscript, string optionID)
    {
        string searchingOptionID = GetPathToOptionID(data, conscript) + optionID;

        for (int i = 0; i < tree.nodes.Count; i++)
        {
            for (int j = 0; j < tree.nodes[i].options.Count; j++)
            {
                if (!string.IsNullOrEmpty(tree.nodes[i].options[j].optionEffect.optionID))
                {
                    return tree.nodes[i].options[j].optionEffect;
                }
            }
        }
        return null;
    }   
    private bool HasSavedRespecModifierByOptionId(OptionEffect optionEffect, NPCData npcData, Conscript cons)
    {
        if (savedRespectModifier.ContainsKey(GetPathToSearchRespectValue(npcData, cons) + optionEffect.optionID))
        {
            return true;
        }

        return false;
    }

    public string GetPathToNpc(NPCData npcd, Conscript con) => GetPathToNpcWithConscript(con) + npcd.humanID;
    private string GetPathToNpcWithConscript(Conscript con) => GetPathToBaseNpc() + ("con_" + con.ToString()+"_");
    private string GetPathToBaseNpc() => "npc_"; 
    private string GetPathToOptionID(NPCData data, Conscript cons) => "optionId_"+ data.humanID + cons.ToString();
    private string GetPathToSearchRespectValue(NPCData data, Conscript cons) => GetPathToOptionID(data, cons) + "_respect_";

    #endregion
}

public class SaveData
{
    public int RespectTotal;
    public List<string> keys;
    public List<int> values;

    public int Day;

    public string CurrentState;
}