public static class SaveKeyGenerator
{
    private const string NPC_PREFIX = "NPC";
    private const string RESPECT_SUFFIX = "RESPECT";

    public static string ForNpc(NPCData data, Conscript con)
        => $"{NPC_PREFIX}:{data.humanID}:{con}";

    public static string ForRespect(NPCData data, Conscript con, string optionID)
        => $"{ForNpc(data,con)}:{RESPECT_SUFFIX}:{optionID}";
}