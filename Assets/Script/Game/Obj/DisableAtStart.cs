using UnityEngine;

public class DisableAtStart : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
