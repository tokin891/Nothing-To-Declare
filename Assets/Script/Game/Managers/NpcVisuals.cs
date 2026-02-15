using UnityEngine;
using System.Collections;

public class NpcVisuals : MonoBehaviour
{
    private Coroutine mainMaterialCoroutine;
    private Material defaultM;

    public static void FlashMaterialNPC(MeshRenderer npcRender, Material changeTo, float duration)
    {
        NpcVisuals nv = GameManager.Instance.NpcVisuals;
        nv.TryToStopMaterialCoroutine();

        nv.mainMaterialCoroutine =
            nv.StartCoroutine(nv.ChangeMaterialNPCForTimeI
            (npcRender, changeTo, duration));
    }

    public static void SetMaterialNPC(MeshRenderer npcRender, Material mat)
    {
        npcRender.material = mat;
    }

    public static void PlayAnimationWithMaterialsNPC(MeshRenderer npcRender, Material[] animations, float durationFrame)
    {
        NpcVisuals nv = GameManager.Instance.NpcVisuals;
        nv.TryToStopMaterialCoroutine();

        nv.mainMaterialCoroutine = nv.StartCoroutine(
            nv.PlayAnimationNPC(npcRender, animations, durationFrame));
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
        mainMaterialCoroutine = null;
        defaultM = null;
    }

    private IEnumerator PlayAnimationNPC(MeshRenderer meshRender, Material[] animations, float durationFrame)
    {
        if (defaultM == null)
        {
            defaultM = meshRender.material;
        }
        for (int i = 0; i < animations.Length; i++)
        {
            meshRender.material = animations[i];
            yield return new WaitForSeconds(durationFrame);
        }
        meshRender.material = defaultM;
        mainMaterialCoroutine = null;
        defaultM = null;
    }

    private void TryToStopMaterialCoroutine()
    {
        if (GameManager.Instance.NpcVisuals.mainMaterialCoroutine != null)
        {
            GameManager.Instance.NpcVisuals.StopCoroutine(GameManager.Instance.NpcVisuals.mainMaterialCoroutine);
        }
    }
}
