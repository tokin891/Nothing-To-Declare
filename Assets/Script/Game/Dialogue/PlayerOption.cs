[System.Serializable]
public class PlayerOption
{
    public LocalizedLine playerLine;
    public PlayerInteractionType typeInteraction;
    // nextNodeId usuwamy - xNode u¿ywa portów
}
public enum PlayerInteractionType
{
    None,
    Arms,
    Files,
    Shoot
}
