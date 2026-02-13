using UnityEngine;

public class ObjectPlayerOption : MonoBehaviour
{
    public DialogueNode nextNode; // Zamiast string nextID
    public PlayerInteractionType interactionType = PlayerInteractionType.None;
    public OptionEffect optionEffects;

    public void ChoseOption()
    {
        if(!string.IsNullOrEmpty(optionEffects.optionID))
        {
            if(optionEffects.respectModifier != 0)
            {
                GameManager.Instance.PlayerStats.TrySaveRespectModifierByOptionId(
                    GameManager.Instance.GetCurrentNpcData, GameManager.Instance.CurrentConscript,
                    optionEffects);
            }
        }

        if (nextNode == null && interactionType != PlayerInteractionType.None)
        {
            GameManager.Instance.NPCDocumentDisplay.InteractionNode(interactionType, GameManager.Instance.GetCurrentNpcData);
        }
        // Wywo³ujemy now¹ metodê managera
        GameManager.Instance.DialogueManager.ChoseDialogueByNode(nextNode, interactionType);
    }
}