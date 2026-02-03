using UnityEngine;

public class ObjectPlayerOption : MonoBehaviour
{
    public DialogueNode nextNode; // Zamiast string nextID
    public PlayerInteractionType interactionType = PlayerInteractionType.None;

    public void ChoseOption()
    {
        // Wywo³ujemy now¹ metodê managera
        DialogueManager.Instance.ChoseDialogueByNode(nextNode, interactionType);
    }
}