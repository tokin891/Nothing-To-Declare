using UnityEngine;
using TMPro;

public class NpcFlowController : MonoBehaviour
{
    [Header("Settings")]
    public TMP_Text countCandidat;
    public TMP_Text countDay;

    [Header("Settings")]
    [SerializeField] private float delayBetweenNpcs = 3f;

    private int currentDay = 1;
    private int npcsLeftToday;
    private int totalNpcsToday;

    public void Initialize()
    {
        StartNewDay(10); 
    }

    public void StartNewDay(int npcCount)
    {
        totalNpcsToday = npcCount;
        npcsLeftToday = npcCount;
        UpdateUI();

        SpawnNextNpcWithDelay();
    }

    public void OnNpcFinished(bool eliminated)
    {
        npcsLeftToday--;
        UpdateUI();

        if (eliminated)
        {
            Debug.Log("NPC zosta³ wyeliminowany. Notujemy to w statystykach...");
        }

        if (npcsLeftToday > 0)
        {
            SpawnNextNpcWithDelay();
        }
        else
        {
            Debug.Log("Koniec dnia! Brak kandydatów.");
        }
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
}
