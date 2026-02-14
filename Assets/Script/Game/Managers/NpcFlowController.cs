using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class NpcFlowController : MonoBehaviour
{
    [Header("Settings")]
    public TMP_Text countCandidat;
    public TMP_Text countDay;

    [Header("Settings")]
    [SerializeField] private float delayBetweenNpcs = 3f;
    [SerializeField] private int candidatPerDay;

    [Header("Day")]
    public DayOption[] dayOptions;

    private int currentDay = 1;
    private int npcsLeftToday;
    private int totalNpcsToday;
    private int currentNpcIndex = 0;

    public int GetCurrentDay => currentDay;
    public int GetCurrentNpcIndex => currentNpcIndex;
    public List<DayOption.DayNpcData> AllNpcForToday { private set; get; }


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
        AllNpcForToday = new List<DayOption.DayNpcData>();
        AllNpcForToday.Clear();

        if (dayConfig != null)
        {
            AllNpcForToday.AddRange(dayConfig.dayNpcDatas);
        }
        //for (int i = 0; i < dayConfig.dayNpcDatas.Length; i++)
        //{
        //    GameManager.Instance.PlayerStats.TrySaveNpc(
        //        dayConfig.dayNpcDatas[i].npc,
        //        dayConfig.dayNpcDatas[i].selectConscript.humanConscript);
        //}

        int randomsNeeded = candidatPerDay - AllNpcForToday.Count;
        for (int i = 0; i < randomsNeeded; i++)
        {
            DayOption.DayNpcData dnd = GenerateRandomNpcData();
            if (dnd.npc != null)
            {
                //GameManager.Instance.PlayerStats.TrySaveNpc(dnd.npc, dnd.selectConscript.humanConscript);
                AllNpcForToday.Add(dnd);
            }
        }

        AllNpcForToday = AllNpcForToday.OrderBy(x => Random.value).ToList();

        totalNpcsToday = AllNpcForToday.Count;
        npcsLeftToday = totalNpcsToday;
        UpdateUI();
    }

    private DayOption.DayNpcData GenerateRandomNpcData()
    {
        // Tutaj logika losowania zwyk³ego NPC z puli ogólnej
        DayOption.DayNpcData fnd = new DayOption.DayNpcData();
        NPCData npc = null;
        int maxAttempts = 100;
        while((npc == null || IsFabularNpc(npc) || 
            GameManager.Instance.PlayerStats.CheckIsHumanWasHere(npc) ||
            IsListContainsNpc(AllNpcForToday,npc)) && maxAttempts > 0)
        {
            npc = GameManager.Instance.GetAllNpcDatas[
                Random.Range(0, GameManager.Instance.GetAllNpcDatas.Length)];
            maxAttempts--;
        }
        if (maxAttempts <= 0)
            npc = null;

        fnd.npc = npc;
        if(npc != null)
            fnd.selectConscript = npc.AllConscriptsNPC[Random.Range(0, npc.AllConscriptsNPC.Length)];
        return fnd;
    }

    private bool IsListContainsNpc(List<DayOption.DayNpcData> list, NPCData npc)
    {
        foreach(var l in list)
        {
            if (l.npc == npc)
                return true;
        }

        return false;
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

    [System.Serializable]
    public class DayNpcData
    {
        public NPCData npc;
        public NPCDataConscript selectConscript;
    }
}
