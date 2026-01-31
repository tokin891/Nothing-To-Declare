using UnityEngine;

[System.Serializable]
public class LocalizedLine
{
    public string key;
    public string pl;
    public string en;
    public string de;

    public string Get()
    {
        return LanguageManager.Current switch
        {
            SystemLanguage.Polish => pl,
            SystemLanguage.English => en,
            SystemLanguage.German => de,
            _ => pl
        };
    }
}
