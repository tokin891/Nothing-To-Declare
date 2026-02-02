using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DialogueManager DialogueManager;
    public NPCManager NPCManager;

    private void Awake()
    {
        Instance = this;
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
