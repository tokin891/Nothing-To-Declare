using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class NPCMovement : MonoBehaviour
{
    [Header("NPC References")]
    [SerializeField] float speed;
    [SerializeField] Transform targetPositionNPC;
    [Tooltip("set it only if you want to test some npc")]
    public NPCObject testNpc;

    private NPCData npcData;
    private NPCDataConscript npcDataConscript;
    private Vector3 startPositionNPC;

    public Conscript CurrentConscript { get; private set; }
    public NPCObject CurrentNPCObject { get; private set; }

    private Coroutine coroutineEnd;
    private Coroutine coroutineStart;

    public Coroutine changeMaterialForDelayC;

    private void Awake()
    {
        startPositionNPC = Vector3.zero;
    }

    public void SetupNPC(NPCObject npcObj, NPCData data, NPCDataConscript conscript)
    {
        CurrentNPCObject = npcObj;
        npcData = data;
        npcObj.NPCData = data;
        npcDataConscript = conscript;
        CurrentConscript = npcDataConscript.humanConscript;
        Debug.Log("Conscript: " + npcDataConscript.humanConscript.ToString());
        npcObj.NPCData = npcData;
        npcObj.renderNPC.enabled = true;
        npcObj.Initialize();
        coroutineStart = StartCoroutine(ShowNPC());
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

        CurrentNPCObject.NPCData = null;
        CurrentNPCObject.renderNPC.enabled = false;
        CurrentNPCObject.transform.position = startPositionNPC;
        CurrentNPCObject.gameObject.SetActive(false);
        CurrentNPCObject = null;
        startPositionNPC = Vector3.zero;
    }

    public static void FlashMaterialNPC(MeshRenderer npcRender, Material changeTo, float duration)
    {
        if (GameManager.Instance.NPCMovement.changeMaterialForDelayC != null)
            return;
        GameManager.Instance.NPCMovement.changeMaterialForDelayC = 
            GameManager.Instance.NPCMovement.StartCoroutine(GameManager.Instance.NPCMovement.ChangeMaterialNPCForTimeI
            (npcRender, changeTo, duration));
    }

    private IEnumerator ChangeMaterialNPCForTimeI(MeshRenderer npcR, Material m, float d)
    {
        Material defaultM = npcR.material;
        npcR.material = m;
        yield return new WaitForSeconds(d);
        npcR.material = defaultM;
        changeMaterialForDelayC = null;
    }

    public static void SetMaterialNPC(MeshRenderer npcRender, Material mat)
    {
        npcRender.material = mat;
    }
}
