using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DialogueManager DialogueManager;
    public NPCManager NPCManager;

    private void Awake()
    {
        DialogueManager.Initialize();
    }
}
