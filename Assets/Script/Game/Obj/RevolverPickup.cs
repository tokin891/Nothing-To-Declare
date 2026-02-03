using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RevolverPickup : MonoBehaviour
{
    [SerializeField] Vector3 offsetPosHighlight;
    [SerializeField] float smoothHighlight = 8f;
    [SerializeField] float offsetLightIntenstiyHighligh = 1f;
    [SerializeField] InputActionReference InputAction;
    [SerializeField] Camera camDesk;
    [SerializeField] AudioClip clipWhosh;
    [SerializeField] AudioClip clipEquip;


    private Vector3 awakePos;
    private float lightIntensityAwake;
    private Light light;
    private bool isEnter;
    private bool isClick;
    private Coroutine coroutineEnter;
    private Coroutine coroutineEnd;
    private float updateOptimization;
    private GameObject objectHit;
    private bool readyToPick;
    private float nextTimeToPlayWhoshSound;

    private void Awake()
    {
        awakePos = transform.position;
        light = GetComponentInChildren<Light>();
        lightIntensityAwake = light.intensity;
    }

    private void OnEnable()
    {
        InputAction.action.Enable();
        InputAction.action.started += ctx => Click();
    }

    private void OnDisable()
    {
        InputAction.action.Disable();
    }

    private void Update()
    {
        if (updateOptimization < Time.time)
        {
            updateOptimization = Time.time + 0.05f;

            if (MovementManager.instance.CurrentState is not DeskState && isEnter)
            {
                coroutineEnd = StartCoroutine(Exit());
                Debug.Log("Update revolver pickup");
            }

            if (objectHit != null)
            {
                if (!isEnter && objectHit.TryGetComponent<RevolverPickup>(out RevolverPickup r))
                {
                    OnPointerEnter();
                    isEnter = true;
                }
                if (isEnter && !objectHit.TryGetComponent<RevolverPickup>(out RevolverPickup r2))
                {
                    OnPointerExit();
                    isEnter = false;
                }
            }else
            {
                if (isEnter)
                {
                    OnPointerExit();
                    isEnter = false;
                }
            }

                CatchRay();  
        }
    }

    private void CatchRay()
    {
        Vector3 mPos = Input.mousePosition;
        Ray mRay = camDesk.ScreenPointToRay(mPos);
        RaycastHit hit;

        bool hitRevolver = Physics.Raycast(mRay, out hit);

        if (hitRevolver)
        {
            objectHit = hit.collider.gameObject;
        }else
            objectHit = null;
    }

    public void OnPointerEnter()
    {
        if (MovementManager.instance.CurrentState is not DeskState)
            return;

        coroutineEnter = StartCoroutine(Enter());
    }

    public void OnPointerExit()
    {
       coroutineEnd = StartCoroutine(Exit());
    }

    public void Click() 
    { 
        if(readyToPick) 
            isClick = true; 
    }

    private IEnumerator Enter()
    {
        if (coroutineEnd != null)
            StopCoroutine(coroutineEnd);
        if (nextTimeToPlayWhoshSound < Time.time)
        {
            SoundsManager.instance.PlayOneTimeClip(clipWhosh, 0.35f);
            nextTimeToPlayWhoshSound = Time.time + 1f;
        }

        readyToPick = true;
        Vector3 targetPos = awakePos + offsetPosHighlight;
        bool fastClick = false;

        light.intensity = lightIntensityAwake + offsetLightIntenstiyHighligh;

        while (Vector3.Distance(transform.position, targetPos) > 0.005f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothHighlight * Time.deltaTime);
            if(isClick)
            {
                transform.position = targetPos;
                fastClick = true;
            }
            yield return null;
        }

        transform.position = targetPos;
        yield return new WaitUntil(() => (isClick == true||fastClick));
        isClick = false;
        fastClick = false;

        MovementManager.instance.SwitchState(MovementManager.instance.ShootState, true);
        SoundsManager.instance.PlayOneTimeClip(clipEquip, 0.35f);
    }

    private IEnumerator Exit()
    {
        if (coroutineEnter != null)
            StopCoroutine(coroutineEnter);

        readyToPick = false;
        light.intensity = lightIntensityAwake;
        Vector3 targetPos = awakePos;

        while (Vector3.Distance(transform.position, targetPos) > 0.005f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothHighlight * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
    }
}
