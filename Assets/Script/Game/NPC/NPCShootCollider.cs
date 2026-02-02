using UnityEngine;

public class NPCShootCollider : MonoBehaviour, IShootable
{
    private NPCObject npcObject;

    private void Awake()
    {
        npcObject = GetComponentInParent<NPCObject>();
    }

    public void EnterCrosshair()
    {
        
    }

    public void Shoot()
    {
        npcObject.gameObject.SetActive(false);
    }
}
