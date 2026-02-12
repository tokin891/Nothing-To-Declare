using UnityEngine;
using System.Collections;

public class NpcVisuals : MonoBehaviour
{
    private Coroutine changeMaterialForDelayC;
    private Material defaultM;

    public static void FlashMaterialNPC(MeshRenderer npcRender, Material changeTo, float duration)
    {
        if (GameManager.Instance.NpcVisuals.changeMaterialForDelayC != null)
        {
            GameManager.Instance.NpcVisuals.StopCoroutine(GameManager.Instance.NpcVisuals.changeMaterialForDelayC);
        }
        GameManager.Instance.NpcVisuals.changeMaterialForDelayC =
            GameManager.Instance.NpcVisuals.StartCoroutine(GameManager.Instance.NpcVisuals.ChangeMaterialNPCForTimeI
            (npcRender, changeTo, duration));
    }

    private IEnumerator ChangeMaterialNPCForTimeI(MeshRenderer npcR, Material m, float d)
    {
        if (defaultM == null)
        {
            defaultM = npcR.material;
        }
        npcR.material = m;
        yield return new WaitForSeconds(d);
        npcR.material = defaultM;
        changeMaterialForDelayC = null;
        defaultM = null;
    }

    public static void SetMaterialNPC(MeshRenderer npcRender, Material mat)
    {
        npcRender.material = mat;
    }
}
