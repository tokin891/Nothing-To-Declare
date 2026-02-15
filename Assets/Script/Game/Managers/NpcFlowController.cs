using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class NpcFlowController : MonoBehaviour
{
    [Header("Settings")]
    public TMP_Text countCandidat;
    public TMP_Text countDay;

    [Space]
    [SerializeField] private float delayBetweenNpcs = 3f;
    [SerializeField] private int candidatPerDay;
    [Range(0,1)]
    [SerializeField] private float killerCandidatPercent;  

    [Header("Day")]
    public DayOption[] dayOptions;

    [Header("Only For Editor Test")]
    [SerializeField] private bool candidatCanRepet = false;
    private bool ccr
    {
        get
        {
#if UNITY_EDITOR
            return candidatCanRepet;
#endif
            return false;
        }
    }

    private int currentDay = 1;
    private int npcsLeftToday;
    private int totalNpcsToday;
    private int currentNpcIndex = 0;

    public int GetCurrentDay => currentDay;
    public int GetCurrentNpcIndex => currentNpcIndex;
    public List<DayNpcData> AllNpcForToday { private set; get; }


    public void Initialize()
    {
        StartNewDay(candidatPerDay); 
    }

    public void StartNewDay(int npcCount)
    {
        UpdateUI();

        PrepareDayQueue(currentDay);
        SpawnNextNpcWithDelay();
    }

    public void OnNpcFinished(bool eliminated)
    {
        npcsLeftToday--;

        if (eliminated)
        {
            Debug.Log("NPC zosta³ wyeliminowany. Notujemy to w statystykach...");
        }

        if (npcsLeftToday > 0)
        {
            SpawnNextNpcWithDelay();
            currentNpcIndex++;
        }
        else
        {
            Debug.Log("Koniec dnia! Brak kandydatów.");
        }
        UpdateUI();
    }

    private void PrepareDayQueue(int day)
    {
        var dayConfig = dayOptions.FirstOrDefault(d => d.selectDay == day);
        AllNpcForToday = new List<DayNpcData>();
        AllNpcForToday.Clear();

        if (dayConfig != null)
        {
            AllNpcForToday.AddRange(dayConfig.dayNpcDatas);
        }

        int randomsNeeded = candidatPerDay - AllNpcForToday.Count;
        int randomsKillerNeeded = (int)(randomsNeeded * killerCandidatPercent);
        int restFromKiller = (randomsNeeded - randomsKillerNeeded);
        int randomsGoodNeeded = (int)(restFromKiller - (restFromKiller * UnityEngine.Random.Range(0.35f, 0.65f)));
        int randomsBadNeeded = randomsNeeded - randomsKillerNeeded - randomsGoodNeeded;

        for (int i = 0; i < randomsKillerNeeded; i++)
        {
            DayNpcData dnd = GenerateRandomNpcData(Conscript.Killer);
            if (dnd.npc != null)
            {
                AllNpcForToday.Add(dnd);
            }
        }
        for (int i = 0; i < randomsGoodNeeded; i++)
        {
            DayNpcData dnd = GenerateRandomNpcData(Conscript.Good);
            if (dnd.npc != null)
            {
                AllNpcForToday.Add(dnd);
            }
        }
        for (int i = 0; i < randomsBadNeeded; i++)
        {
            DayNpcData dnd = GenerateRandomNpcData(Conscript.Bad);
            if (dnd.npc != null)
            {
                AllNpcForToday.Add(dnd);
            }
        }

        AllNpcForToday = AllNpcForToday.OrderBy(x => UnityEngine.Random.value).ToList();
        GameManager.Instance.PlayerStats.GenereteSaveQueueNpc(AllNpcForToday, day);

        totalNpcsToday = AllNpcForToday.Count;
        npcsLeftToday = totalNpcsToday;
        UpdateUI();
    }

    private DayNpcData GenerateRandomNpcData(Conscript con)
    {
        // Tutaj logika losowania zwyk³ego NPC z puli ogólnej
        DayNpcData fnd = new DayNpcData();
        NPCData npc = null;
        int maxAttempts = 100;

        Func<NPCData, bool> wasVisitedCheck = npc =>
        {
            if (ccr) return false; // pozwalamy na powtórki
            return GameManager.Instance.PlayerStats.TryCheckIsHumanWasVisited(npc);
        };

        Func<NPCData, bool> isInListCheck = npc =>
        {
            if (ccr) return false; // pozwalamy na powtórki
            return IsListContainsNpc(AllNpcForToday, npc);
        };


        while (maxAttempts > 0)
        {
            npc = GameManager.Instance.GetAllNpcDatas[
                UnityEngine.Random.Range(0, GameManager.Instance.GetAllNpcDatas.Length)];

            if (npc != null &&
                !IsFabularNpc(npc) &&
                !wasVisitedCheck(npc) &&
                !isInListCheck(npc) &&
                DoesNpcHaveConscript(con, npc))
            {
                break;
            }

            maxAttempts--;
        }

        if (maxAttempts <= 0)
            npc = null;

        fnd.npc = npc;
        if(npc != null)
            fnd.selectConscript = GetConscriptDataByEnum(con, npc);
        return fnd;
    }

    private bool IsListContainsNpc(List<DayNpcData> list, NPCData npc)
    {
        foreach(var l in list)
        {
            if (l.npc == npc)
                return true;
        }

        return false;
    }

    private bool DoesNpcHaveConscript(Conscript conscriptEnum, NPCData data)
    {
        List<NPCDataConscript> conscriptsData = data.AllConscriptsNPC.ToList();
        foreach (var c in conscriptsData)
            if (c.humanConscript == conscriptEnum)
                return true;
        return false;
    }

    private NPCDataConscript GetConscriptDataByEnum(Conscript conscriptEnum, NPCData data)
    {
        List<NPCDataConscript> conscriptsData = data.AllConscriptsNPC.ToList();
        foreach (var c in conscriptsData)
            if (c.humanConscript == conscriptEnum)
                return c;
        return null;
    }

    private void SpawnNextNpcWithDelay()
    {
        GameManager.Instance.SetupNPCWithDelay(delayBetweenNpcs);
    }

    private void UpdateUI()
    {
        countCandidat.text = $"{npcsLeftToday}/{totalNpcsToday}";
        countDay.text = $"Day: {currentDay}";
    }

    public static bool IsFabularNpc(NPCData npcData)
    {
        if (GameManager.Instance.NpcFlowController == null || npcData == null) return false;

        // Szukamy ustawieñ dla obecnego dnia
        foreach (var dayOpt in GameManager.Instance.NpcFlowController.dayOptions)
        {
            foreach(var fabNpc in dayOpt.dayNpcDatas)
            {
                if (npcData == fabNpc.npc)
                    return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class DayOption
{
    public int selectDay;
    [Tooltip("Those npc's must be in selected day")]
    public DayNpcData[] dayNpcDatas;
}
[System.Serializable]
public class DayNpcData
{
    public NPCData npc;
    public NPCDataConscript selectConscript;
}
