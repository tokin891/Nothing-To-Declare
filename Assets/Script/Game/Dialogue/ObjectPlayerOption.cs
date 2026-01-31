using UnityEngine;

public class ObjectPlayerOption : MonoBehaviour
{
    public string nextID;
    public PlayerInteractionType interactionType = PlayerInteractionType.None;

    public void ChoseOption()
    {
        DialogueManager.Instance.ChoseDialogueByID(nextID, interactionType);
    }
}
