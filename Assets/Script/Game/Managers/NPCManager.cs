using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    [Header("NPC References")]
    [SerializeField] float speed;
    [SerializeField] Transform targetPositionNPC;
    [SerializeField] NPCObject[] allNPCs;

    [Header("File")]
    [SerializeField] GameObject fileContainer;
    [SerializeField] Image iconNPC;
    [SerializeField] TMP_Text nameNPC;
    [SerializeField] TMP_Text secondNameNPC;
    [SerializeField] TMP_Text birthNPC;
    [SerializeField] TMP_Text eyesNPC;
    [SerializeField] TMP_Text ageNPC;
    [SerializeField] TMP_Text WeightNPC;
    [SerializeField] TMP_Text GenderNPC;
    [SerializeField] TMP_Text AdditionalInfoNPC;
    [SerializeField] TMP_Text CountryNPC;
    [SerializeField] TMP_Text IDNPC;

    [Header("Arms")]
    [SerializeField] GameObject armContainer;
    [SerializeField] Image armImage;

    private NPCData npcData;
    private NPCDataConscript npcDataConscript;
    private Vector3 startPositionNPC;

    public Conscript CurrentConscript { get; private set; }
    public NPCObject CurrentNPCObject { get; private set; }

    private void Awake()
    {
        Instance = this;
        startPositionNPC = Vector3.zero;
    }

    public void Initialize()
    {
        CurrentNPCObject = allNPCs[Random.Range(0,allNPCs.Length)];

        SetupNPC(CurrentNPCObject.NPCDatas);
    }

    public void SetupNPC(NPCData npc)
    {
        npcData = npc;
        npcDataConscript = npc.AllConscriptsNPC[Random.Range(0, npc.AllConscriptsNPC.Length)];
        CurrentConscript = npcDataConscript.humanConscript;
        Debug.Log("Conscript: " + npcDataConscript.humanConscript.ToString());
        StartCoroutine(ShowNPC());
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
        DialogueManager.Instance.StartDialogue(npcDataConscript.dialogueTree); // Open Dialogue
    }

    public void EndDialogue()
    {
        StartCoroutine(HideNPCCorutine());

        // Logic stuff

        // Clear 
        npcData = null;
        npcDataConscript = null;
    }
    private IEnumerator HideNPCCorutine()
    {
        yield return new WaitForSeconds(1.5f);  //Wait 3 second
        
        while (Vector3.Distance(CurrentNPCObject.transform.position, startPositionNPC) > 0.01f)
        {
            CurrentNPCObject.transform.position = Vector3.MoveTowards(CurrentNPCObject.transform.position, startPositionNPC, speed * Time.deltaTime);
            yield return null; // czekaj do nastêpnej klatki
        }

        CurrentNPCObject.transform.position = startPositionNPC; // wyrównanie pozycji na koniec
        CurrentNPCObject.gameObject.SetActive(false);
        CurrentNPCObject = null;
        startPositionNPC = Vector3.zero;
    }

    public void InteractionNode(PlayerInteractionType interactionType)
    {
        switch (interactionType)
        {
            case PlayerInteractionType.Files:
                ShowFile(npcData);
                break;
            case PlayerInteractionType.Arms:
                ShowArms(npcData);
                break;
        }
    }

    private void ShowFile(NPCData data)
    {
        fileContainer.SetActive(true);
        iconNPC.sprite = data.humanFace;
        nameNPC.text = "First Name: \n" + data.humanName;
        secondNameNPC.text = "Second Name: \n" + data.humanSecondName;
        eyesNPC.text = "Eyes Color: \n" + data.humanEyesColor;
        birthNPC.text = "Date of Birth: \n" + data.humanDate;
        ageNPC.text = "Age: " + data.humanAge;
        WeightNPC.text = "Weight: " + data.humanWeight + "kg" + " Height: " + data.humanHeight + "cm";
        GenderNPC.text = "Gender: " + data.humanGender;
        AdditionalInfoNPC.text = "Additional Info: " + data.humanAdditionalInfo;
        CountryNPC.text = "Country: " + data.humanCountry.ToString();
        IDNPC.text = "ID: " + (data.humanID * 1239567 + 9835543).ToString();
    }

    private void ShowArms(NPCData data)
    {
        armImage.sprite = npcDataConscript.armSprite;
        armContainer.SetActive(true);
    }

    public void HideFile()
    {
        DialogueManager.Instance.StopInteraction();
        iconNPC.sprite = null;
        nameNPC.text = "";
        secondNameNPC.text = "";
        ageNPC.text = "";
        WeightNPC.text = "";
        GenderNPC.text = "";
        AdditionalInfoNPC.text = "";
        CountryNPC.text = "";
        IDNPC.text = "";
        fileContainer.SetActive(false);
    }

    public void HideArms()
    {
        DialogueManager.Instance.StopInteraction();
        armContainer.SetActive(false);
        armImage.sprite = null;
    }
}
