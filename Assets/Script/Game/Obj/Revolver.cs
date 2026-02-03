using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class Revolver : MonoBehaviour
{
    [SerializeField] float maxDistancePistol;
    [SerializeField] InputActionReference inputAction;
    [SerializeField] Image crosshairImage;
    [SerializeField] Animator[] revolverAnm;
    [SerializeField] float flashFadeInSpeed = 0.001f;
    [SerializeField] float flashFadeOutSpeed = 0.1f;
    [SerializeField] CanvasGroup flashCanvas;
    [SerializeField] AudioClip shootClip;

    private float catchNextRayTime;
    private float catchNextRayTriggerTime;
    private Camera cam;
    private bool doNotLoop;
    private bool cantShoot;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        inputAction.action.started += ctx => TryShoot();
    }

    private void OnEnable()
    {
        inputAction.action.Enable();
    }
    private void OnDisable()
    {
        inputAction.action.Disable();
    }

    void Update()
    {
        if (catchNextRayTime < Time.time)
        {
            catchNextRayTime = Time.time + 0.1f;
            if (IsRayCatchingSomething())
            {
                if (!doNotLoop)
                {
                    crosshairImage.color = Color.red;
                    doNotLoop = true;
                }
            }
            else if (doNotLoop)
            {
                crosshairImage.color = Color.white;
                doNotLoop = false;
            }
        }
    }

    private void CatchRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistancePistol))
        {
            if (hit.collider.TryGetComponent<IShootable>(out IShootable shootingObj))
            {
                shootingObj.Shoot();
                Shoot(shootingObj);
            }
        }
    }

    private void Shoot(IShootable shootingObj)
    {
        for (int i = 0; i < revolverAnm.Length; i++)
        {
            revolverAnm[i].SetTrigger("shoot");
        }
        StartCoroutine(ShootCorutine(shootingObj));
    }

    private IEnumerator ShootCorutine(IShootable shootingObj)
    {
        cantShoot = true;
        SoundsManager.instance.PlayOneTimeClip(shootClip, 0.75f);
        yield return new WaitForSeconds(0.1f);
        flashCanvas.alpha = 0f;
        while(flashCanvas.alpha < 1f)
        {
            flashCanvas.alpha += Time.deltaTime * flashFadeInSpeed;
            yield return null;
        }
        shootingObj.Shoot();
        MovementManager.instance.RemoveBullet();
        flashCanvas.alpha = 1f;
        while (flashCanvas.alpha > 0f)
        {
            flashCanvas.alpha -= Time.deltaTime * flashFadeOutSpeed;
            yield return null;
        }
        GameManager.Instance.ClearNPC();
        yield return new WaitForSeconds(2.5f);
        MovementManager.instance.SwitchState(MovementManager.instance.DeskState, true);
        GameManager.Instance.SetupNPCWithDelay(4);
        cantShoot = false;
    }

    private bool IsRayCatchingSomething()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistancePistol))
        {
            if (hit.collider.TryGetComponent<IShootable>(out IShootable shootingObj))
            {
                shootingObj.EnterCrosshair();
                return true;
            }
        }
        return false;
    }

    public void TryShoot()
    {
        if (cantShoot)
            return;

        if (catchNextRayTriggerTime < Time.time)
        {
            catchNextRayTriggerTime = Time.time + 2.5f;
            CatchRay();
        }
    }
}

public interface IShootable
{
    public void EnterCrosshair();
    public void Shoot();
}