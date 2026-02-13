using UnityEngine;

public class LoupePickup : PickableObjectBehaviour
{
    public override void OnClickSuccesfully()
    {
        if (!GameManager.Instance.IsNpcInOffice)
            return;
        GameManager.Instance.NPCDocumentDisplay.InteractionNode(PlayerInteractionType.Eye, GameManager.Instance.GetCurrentNpcData);
    }
}
