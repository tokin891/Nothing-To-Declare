using UnityEngine;

[CreateAssetMenu(menuName ="NPC/Components/Change Material By Action")]
public class ChangeMaterialNpcComponentna : NPCBehaviour
{
    public Material[] material;
    [Tooltip("If you set 0 then it will change pernametly")]
    public float duration;

    public override void Execute(NPCObject npc)
    {
        if (duration == 0)
            NPCMovement.SetMaterialNPC(npc.renderNPC, material[Random.Range(0,material.Length)]);
        else
            NPCMovement.FlashMaterialNPC(npc.renderNPC, material[Random.Range(0, material.Length)], duration);
    }
}
