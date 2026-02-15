using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Components/Animation Npc")]
public class PlayAnimationNpcComponent : NPCBehaviour
{
    [SerializeField] Material[] animation;
    [SerializeField] float durationFrame = 0.25f;

    public override void Execute(NPCObject npc)
    {
        NpcVisuals.PlayAnimationWithMaterialsNPC(npc.renderNPC, animation, durationFrame);
    }
}
