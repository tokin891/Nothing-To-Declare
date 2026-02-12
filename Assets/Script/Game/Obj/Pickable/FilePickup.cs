using UnityEngine;

public class FilePickup : PickableObjectBehaviour
{
    public override void OnClickSuccesfully()
    {
        Debug.Log("See file");
    }
}
