using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] NPCMovement npcMovement;
    [SerializeField] SoundsManager soundsManager;
    [SerializeField] NpcFlowController npcFlowController;
    [SerializeField] NPCDocumentDisplay npcDocumentDisplay;
    [SerializeField] MovementManager movementManager;
    public NPCObject NpcObject;
    private NPCData[] allNpcDataFromResources;

    public DialogueManager DialogueManager => dialogueManager;
    public NpcFlowController NpcFlowController => npcFlowController;
    public NPCDocumentDisplay NPCDocumentDisplay => npcDocumentDisplay;
    public MovementManager MovementManager => movementManager;
    public NPCMovement NPCMovement => npcMovement;
    public SoundsManager SoundsManager => soundsManager;

    public NPCData GetCurrentNpcData => NPCMovement.CurrentNPCObject.NPCData;
    public Conscript CurrentConscript => NPCMovement.CurrentConscript;
    public NPCData[] GetAllNpcDatas => allNpcDataFromResources;

    private void Start()
    {
        allNpcDataFromResources = Resources.LoadAll<NPCData>("NPC & Dialogue");  
    }

    private void Awake()
    {
        Instance = this;
        soundsManager.Initialize();
    }

    public void ClearNPC()
    {
        npcMovement.ClearDialogueNPC();
        dialogueManager.EndDialogue();
    }

    public void SetupNPC()
    {
        NPCData npcCurrent = GetNextCandidat();
        npcMovement.SetupNPC(NpcObject,npcCurrent , GetConscriptFromNpc(npcCurrent));
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

    private NPCData GetNextCandidat()
    {
        return GetAllNpcDatas[Random.Range(0, GetAllNpcDatas.Length)];
    }

    private NPCDataConscript GetConscriptFromNpc(NPCData data)
    {
        return data.AllConscriptsNPC[Random.Range(0, data.AllConscriptsNPC.Length)];
    }
}
