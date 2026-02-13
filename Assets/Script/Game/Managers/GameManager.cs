using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] NpcMovement npcMovement;
    [SerializeField] SoundsManager soundsManager;
    [SerializeField] NpcFlowController npcFlowController;
    [SerializeField] NpcDocumentDisplay npcDocumentDisplay;
    [SerializeField] MovementManager movementManager;
    [SerializeField] NpcVisuals npcVisuals;
    [SerializeField] PlayerStats playerStats;
    public NPCObject NpcObject;
    private NPCData[] allNpcDataFromResources;

    [HideInInspector] public DialogueManager DialogueManager => dialogueManager;
    [HideInInspector] public NpcFlowController NpcFlowController => npcFlowController;
    [HideInInspector] public NpcDocumentDisplay NPCDocumentDisplay => npcDocumentDisplay;
    [HideInInspector] public MovementManager MovementManager => movementManager;
    [HideInInspector] public NpcMovement NPCMovement => npcMovement;
    [HideInInspector] public SoundsManager SoundsManager => soundsManager;
    [HideInInspector] public NpcVisuals NpcVisuals => npcVisuals;
    [HideInInspector] public PlayerStats PlayerStats => playerStats;

    [HideInInspector] public NPCData GetCurrentNpcData => NPCMovement.CurrentNPCObject.NpcData;
    [HideInInspector] public Conscript CurrentConscript => NPCMovement.CurrentConscript;
    [HideInInspector] public NPCData[] GetAllNpcDatas => allNpcDataFromResources;
    public bool IsNpcInOffice { get
        {
            if (npcMovement.CurrentNPCObject != null)
                return true;
            return false;
        } }

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
        DayOption.DayNpcData npcCurrent = GetNextCandidat();
        npcMovement.SetupNPC(NpcObject,npcCurrent.npc , npcCurrent.selectConscript);
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

    private DayOption.DayNpcData GetNextCandidat()
    {
        return NpcFlowController.AllNpcForToday[NpcFlowController.GetCurrentNpcIndex];
    }
}
