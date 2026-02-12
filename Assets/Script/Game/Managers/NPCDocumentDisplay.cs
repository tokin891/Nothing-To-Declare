using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCDocumentDisplay : MonoBehaviour
{
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

    public void InteractionNode(PlayerInteractionType interactionType, NPCData npcData)
    {
        switch (interactionType)
        {
            case PlayerInteractionType.Files:
                ShowFile(npcData);
                break;
            case PlayerInteractionType.Arms:
                ShowArms(npcData);
                break;
            case PlayerInteractionType.Shoot:
                GameManager.Instance.MovementManager.SwitchState(GameManager.Instance.MovementManager.ShootState, true);
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
        armImage.sprite = data.GetConscriptData(GameManager.Instance.CurrentConscript).armSprite;
        armContainer.SetActive(true);
    }

    public void HideFile()
    {
        GameManager.Instance.DialogueManager.StopInteraction();
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
        GameManager.Instance.DialogueManager.StopInteraction();
        armContainer.SetActive(false);
        armImage.sprite = null;
    }
}
