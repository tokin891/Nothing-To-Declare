using UnityEngine;

public static class Extensions
{
    // Rozszerzenie dla tablicy GameObjectów
    public static void SetActiveAll(this GameObject[] objects, bool state)
    {
        if (objects == null) return;
        foreach (var obj in objects)
        {
            if (obj != null) obj.SetActive(state);
        }
    }

    // Mo¿esz dodaæ te¿ rozszerzenie dla tablicy Œwiate³ (Lights)
    public static void SetEnabledAll(this Light[] lights, bool state)
    {
        if (lights == null) return;
        foreach (var light in lights)
        {
            if (light != null) light.enabled = state;
        }
    }
}