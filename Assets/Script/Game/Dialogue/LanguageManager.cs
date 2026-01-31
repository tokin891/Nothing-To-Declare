using UnityEngine;
using System;

public static class LanguageManager
{
    public static SystemLanguage Current = SystemLanguage.Polish;
    public static event Action OnLanguageChanged;

    public static void SetLanguage(SystemLanguage lang)
    {
        Current = lang;
        OnLanguageChanged?.Invoke();
    }
}
