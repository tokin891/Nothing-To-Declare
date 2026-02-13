[System.Serializable]
public class PlayerOption
{
    public LocalizedLine playerLine;
    public PlayerInteractionType typeInteraction;
    public OptionEffect optionEffect;
}
public enum PlayerInteractionType
{
    None,
    Arms,
    Files,
    Shoot,
    Eye
}
[System.Serializable]
public class OptionEffect
{
    public string optionID;
    public int respectModifier;
}
