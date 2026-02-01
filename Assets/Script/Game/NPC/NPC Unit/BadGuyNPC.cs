using System.Collections;
using UnityEngine;

public class BadGuyNPC : MonoBehaviour, IActionNPC
{
    private const string stressActionID = "BadGuy_Sprite_Stress";

    private MeshRenderer mRender;
    private Material defualtMaterial;
    [SerializeField] private Material stressMaterial;

    private void Awake()
    {
        mRender = GetComponentInChildren<MeshRenderer>();
        defualtMaterial = mRender.material;
    }

    public void InputAction(string actionID)
    {
        switch(actionID)
        {
            case stressActionID:
                StartCoroutine(ShowStress());
                break;
        }
    }

    private IEnumerator ShowStress()
    {
        mRender.material = stressMaterial;
        yield return new WaitForSeconds(Random.Range(3, 6));

        mRender.material = defualtMaterial;
    }
}
