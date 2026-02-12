using UnityEngine;

public class AuthorNPC : MonoBehaviour, IActionNPC
{
    private const string stressAction = "stress";
    private const string happyAction = "happy";
    private const string muscleAction = "muscle";

    [SerializeField] MeshRenderer render;
    [SerializeField] Material[] stressMaterial;
    [SerializeField] Material[] happyMaterial;
    [SerializeField] Material muscleMaterial;

    public void InputAction(string actionID)
    {
        switch(actionID)
        {
            case stressAction:
                NPCMovement.FlashMaterialNPC(render, stressMaterial[Random.Range(0, stressMaterial.Length)], Random.Range(5, 7));
                break;
            case happyAction:
                NPCMovement.FlashMaterialNPC(render, happyMaterial[Random.Range(0, happyMaterial.Length)], Random.Range(5, 7));
                break;
            case muscleAction:
                NPCMovement.FlashMaterialNPC(render, muscleMaterial, Random.Range(3, 5));
                break;
        }
    }
}
