using System.Collections;
using UnityEngine;

public class BadGuyNPC : MonoBehaviour, IActionNPC
{
    private const string stressActionID = "BadGuy_Sprite_Stress";
    private const string musicActionID = "BadGuyMusic";

    private MeshRenderer mRender;
    [SerializeField] private Material stressMaterial;
    [SerializeField] AudioClip clipMusic;
    [SerializeField] float speedTuningMusic = 0.05f;

    private void Awake()
    {
        mRender = GetComponentInChildren<MeshRenderer>();
    }

    public void InputAction(string actionID)
    {
        switch(actionID)
        {
            case stressActionID:
                NPCManager.ChangeMaterialNPCForTime(mRender, stressMaterial, Random.Range(3, 6));
                break;
            case musicActionID:
                SoundsManager.instance.PlayAudioAndBlendWithAudioSourceForTime(clipMusic, speedTuningMusic, 0.05f, true, Random.Range(6, 10), SoundsManager.instance.mainAmbientSource, 0.4f);
                break;
        }
    }
}
