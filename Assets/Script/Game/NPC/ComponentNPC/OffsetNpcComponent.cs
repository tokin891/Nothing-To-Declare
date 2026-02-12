using UnityEngine;

[CreateAssetMenu(menuName ="NPC/Components/Offset Position and Scale")]
public class OffsetNpcComponent : NPCBehaviour
{
    public Vector3 positionOffset;
    public Vector3 scaleMultiplier = Vector3.one; // U¿ywamy mno¿nika dla skali

    public override void Execute(NPCObject npc)
    {
        // Ustawianie offsetu pozycji
        npc.TransformOfRenderMesh.localPosition = npc.PositionAwakeOfRenderMesh + positionOffset;

        // Skalowanie (bezpieczniejsze mno¿enie)
        Vector3 currentScale = Vector3.one;
        npc.TransformOfRenderMesh.localScale = new Vector3(
            currentScale.x * scaleMultiplier.x,
            currentScale.y * scaleMultiplier.y,
            currentScale.z * scaleMultiplier.z
        );
    }
}