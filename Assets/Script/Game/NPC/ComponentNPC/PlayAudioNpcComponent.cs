using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Components/Play Audio On Action")]
public class PlayAudioNpcComponent : NPCBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] TypePlayingAudio Type = TypePlayingAudio.AddictiveLoop;

    [Header("Settings")]
    [SerializeField] float targetVolume;
    [SerializeField , Tooltip("Work with addictive loop")] float speedAmplify;
    [SerializeField, Tooltip("Work with addictive loop and 0 means no ending")] float duration = 0f;

    public override void Execute(NPCObject npc)
    {
        switch(Type)
        {
            case TypePlayingAudio.AddictiveLoop:
                GameManager.Instance.SoundsManager.PlayAudio(clip, speedAmplify, targetVolume, true, duration);
                break;
            case TypePlayingAudio.AddictiveOneShoot:
                GameManager.Instance.SoundsManager.PlayOneTimeClip(clip, targetVolume);
                break;
        }
    }

    private enum TypePlayingAudio
    {
        AddictiveLoop,
        AddictiveOneShoot
    }
}
