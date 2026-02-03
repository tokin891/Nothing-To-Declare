using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] DialogueManager DialogueManager;
    [SerializeField] NPCManager NPCManager;
    [SerializeField] SoundsManager SoundsManager;

    private void Awake()
    {
        Instance = this;
        SoundsManager.Initialize();
    }

    public void ClearNPC()
    {
        NPCManager.ClearDialogueNPC();
        DialogueManager.EndDialogue();
    }

    public void SetupNPC()
    {
        NPCManager.SetupNPC(NPCManager.allNPCs[Random.Range(0, NPCManager.allNPCs.Length)]);
    }

    public void SetupNPCWithDelay(float delay)
    {
        StartCoroutine(SetupCoroutine(delay));
    }

    IEnumerator SetupCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        SetupNPC();
    }
}
