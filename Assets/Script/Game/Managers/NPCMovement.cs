using System.Collections;
using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    [Header("NPC References")]
    [SerializeField] float speed;
    [SerializeField] Transform targetPositionNPC;
    [Tooltip("set it only if you want to test some npc")]
    public NPCData testNpc;

    private NPCData npcData;
    private NPCDataConscript npcDataConscript;
    private Vector3 startPositionNPC;

    public Conscript CurrentConscript { get; private set; }
    public NPCObject CurrentNPCObject { get; private set; }

    private Coroutine coroutineEnd;
    private Coroutine coroutineStart;

    private void Awake()
    {
        startPositionNPC = Vector3.zero;
    }

    public void SetupNPC(NPCObject npcObj, NPCData data, NPCDataConscript conscript)
    {
        NPCData useData = data;
        NPCDataConscript useConscript = conscript;

#if UNITY_EDITOR
        if (testNpc != null)
        {
            useData = testNpc;
            useConscript = testNpc.AllConscriptsNPC[0];
        }
#endif

        CurrentNPCObject = npcObj;
        npcData = useData;
        npcObj.NpcData = useData;
        npcDataConscript = useConscript;
        CurrentConscript = npcDataConscript.humanConscript;
        Debug.Log("Conscript: " + npcDataConscript.humanConscript.ToString());
        npcObj.NpcData = npcData;
        npcObj.renderNPC.enabled = true;
        npcObj.Initialize();
        GameManager.Instance.PlayerStats.TrySaveNpc(npcData, npcDataConscript.humanConscript);
        coroutineStart = StartCoroutine(ShowNPC());

        Debug.Log(npcData.humanName + " " + GameManager.Instance.PlayerStats.TryGetRespectFromNpcData(useData, CurrentConscript));
    }

    private IEnumerator ShowNPC()
    {
        startPositionNPC = CurrentNPCObject.transform.position;
        CurrentNPCObject.gameObject.SetActive(true);
        while (Vector3.Distance(CurrentNPCObject.transform.position, targetPositionNPC.position) > 0.01f)
        {
            CurrentNPCObject.transform.position = Vector3.MoveTowards(CurrentNPCObject.transform.position, targetPositionNPC.position, speed * Time.deltaTime);
            yield return null; // czekaj do nastêpnej klatki
        }

        CurrentNPCObject.transform.position = targetPositionNPC.position; // wyrównanie pozycji na koniec
        yield return new WaitForSeconds(1f);
        GameManager.Instance.DialogueManager.StartDialogue(npcDataConscript.dialogueTree); // Open Dialogue
    }

    public void EndDialogue()
    {
        coroutineEnd = StartCoroutine(HideNPCCorutine());
    }
    private IEnumerator HideNPCCorutine()
    {
        yield return new WaitForSeconds(1.5f);  //Wait 3 second
        
        while (Vector3.Distance(CurrentNPCObject.transform.position, startPositionNPC) > 0.01f)
        {
            CurrentNPCObject.transform.position = Vector3.MoveTowards(CurrentNPCObject.transform.position, startPositionNPC, speed * Time.deltaTime);
            yield return null; // czekaj do nastêpnej klatki
        }
        GameManager.Instance.NpcFlowController.OnNpcFinished(false);
        ClearDialogueNPC();
    }

    public void ClearDialogueNPC()
    {
        if (coroutineEnd != null)
        {
            StopCoroutine(coroutineEnd);
        }
        if (coroutineStart != null)
        {
            StopCoroutine(coroutineStart);
        }
    
        npcData = null;
        npcDataConscript = null;

        CurrentNPCObject.NpcData = null;
        CurrentNPCObject.renderNPC.enabled = false;
        CurrentNPCObject.transform.position = startPositionNPC;
        CurrentNPCObject.gameObject.SetActive(false);
        CurrentNPCObject = null;
        startPositionNPC = Vector3.zero;
    }   
}
