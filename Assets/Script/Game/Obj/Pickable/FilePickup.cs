using UnityEngine;

public class FilePickup : PickableObjectBehaviour
{
    public override void OnClickSuccesfully()
    {
        Debug.Log("See file");
        if(GameManager.Instance.GetCurrentNpcData != null)
        {
            GameManager.Instance.NPCDocumentDisplay.InteractionNode(PlayerInteractionType.Files,GameManager.Instance.GetCurrentNpcData);
        }
    }
}
