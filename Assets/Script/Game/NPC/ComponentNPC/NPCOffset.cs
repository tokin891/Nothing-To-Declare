using UnityEngine;

public class NPCOffset : MonoBehaviour
{
    public Vector3 positionOffset;
    public Vector3 scaleMultiplier = Vector3.one; // U¿ywamy mno¿nika dla skali
    public Transform transformNPC;

    private void Awake()
    {
        if (transformNPC == null) transformNPC = transform;

        // Ustawianie offsetu pozycji
        transformNPC.localPosition += positionOffset;

        // Skalowanie (bezpieczniejsze mno¿enie)
        Vector3 currentScale = transformNPC.localScale;
        transformNPC.localScale = new Vector3(
            currentScale.x * scaleMultiplier.x,
            currentScale.y * scaleMultiplier.y,
            currentScale.z * scaleMultiplier.z
        );
    }
}