using UnityEngine;

public class ObjectPlayerOption : MonoBehaviour
{
    public DialogueNode nextNode; // Zamiast string nextID
    public PlayerInteractionType interactionType = PlayerInteractionType.None;

    public void ChoseOption()
    {
        if (nextNode == null && interactionType != PlayerInteractionType.None)
        {
            GameManager.Instance.NPCDocumentDisplay.InteractionNode(interactionType, GameManager.Instance.GetCurrentNpcData);
        }
        // Wywo³ujemy now¹ metodê managera
        GameManager.Instance.DialogueManager.ChoseDialogueByNode(nextNode, interactionType);
    }
}