[System.Serializable]
public class PlayerOption
{
    public LocalizedLine playerLine;      // tekst odpowiedzi gracza
    public string nextNodeId;             // do którego node przechodzimy
    public PlayerInteractionType typeInteraction;   // enum opcji
}
public enum PlayerInteractionType
{
    None,
    Arms,
    Files,
    Shoot
}
