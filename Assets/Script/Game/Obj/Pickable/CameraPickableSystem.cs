using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraPickableSystem : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] InputActionReference InputAction;

    private GameObject objectHit;
    private PickableObjectBehaviour pob;
    private float updateOptimization;
    private bool isHover;

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

            CatchRay();

            if (GameManager.Instance.MovementManager.CurrentState is not DeskState && isHover && pob != null)
            {
                pob.ExitPick();
                pob = null;
                Debug.Log("Update revolver pickup");
            }

            if (pob == null && objectHit != null)
            {
                if (!objectHit.TryGetComponent<PickableObjectBehaviour>(out pob))
                {
                    return;
                }
            }

            if (objectHit != null)
            {
                if (!isHover && pob != null)
                {
                    pob.EnterPick();
                    isHover = true;
                }
                if (isHover && objectHit.gameObject != pob.gameObject)
                {
                    pob.ExitPick();
                    isHover = false;
                    pob = null;
                }
            }
            else
            {
                if (isHover)
                {
                    pob.ExitPick();
                    isHover = false;
                    pob = null;
                }
            }
        }
    }

    public void Click()
    {
        if (GameManager.Instance.MovementManager.CurrentState is not DeskState)
            return;

        if (pob != null)
        {
            pob.Click();
        }
    }

    private void CatchRay()
    {
        Vector3 mPos = Input.mousePosition;
        Ray mRay = cam.ScreenPointToRay(mPos);
        RaycastHit hit;

        bool hitObj = Physics.Raycast(mRay, out hit);

        if (hitObj)
        {
            objectHit = hit.collider.gameObject;
        }
        else
            objectHit = null;
    }
}

public abstract class PickableObjectBehaviour : MonoBehaviour
{
    [Header("Position Settings")]
    [SerializeField] Vector3 offsetPosHighlight;
    [SerializeField] float smoothHighlight = 8f;

    [Header("Rotation Settings")]
    [SerializeField] Vector3 offsetRotHighlight; // Dodajemy offset rotacji (w stopniach)

    [Header("Visuals & Audio")]
    [SerializeField] float offsetLightIntenstiyHighligh = 1f;
    [SerializeField] AudioClip clipWhosh;
    [SerializeField] AudioClip clipEquip;
    [SerializeField] Light light;

    private Vector3 awakePos;
    private Quaternion awakeRot; // Przechowywanie pocz¹tkowej rotacji
    private float lightIntensityAwake;
    private bool isClick;
    private Coroutine coroutineEnter;
    private Coroutine coroutineEnd;
    private float nextTimeToPlayWhoshSound;

    private void Awake()
    {
        awakePos = transform.position;
        awakeRot = transform.rotation; // Zapamiêtujemy rotacjê
        lightIntensityAwake = light.intensity;
    }

    private IEnumerator Enter()
    {
        if (coroutineEnd != null)
            StopCoroutine(coroutineEnd);
        bool fastClick = false;

        if (nextTimeToPlayWhoshSound < Time.time)
        {
            GameManager.Instance.SoundsManager.PlayOneTimeClip(clipWhosh, 0.35f);
            nextTimeToPlayWhoshSound = Time.time + 1f;
        }

        Vector3 targetPos = awakePos + offsetPosHighlight;
        // Obliczamy docelow¹ rotacjê (lokalny offset dodany do pocz¹tkowej rotacji)
        Quaternion targetRot = awakeRot * Quaternion.Euler(offsetRotHighlight);

        isClick = false;
        light.intensity = lightIntensityAwake + offsetLightIntenstiyHighligh;

        // Warunek pêtli sprawdzaj¹cy zarówno pozycjê, jak i k¹t
        while (Vector3.Distance(transform.position, targetPos) > 0.005f ||
               Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothHighlight * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smoothHighlight * Time.deltaTime);
            if(isClick)
            {
                transform.position = targetPos;
                transform.rotation = targetRot;
                fastClick = true;
            }
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        yield return new WaitUntil(() => (isClick == true || fastClick == true));
        isClick = false;
        fastClick = false;

        ExitPick();
        GameManager.Instance.SoundsManager.PlayOneTimeClip(clipEquip, 0.35f);
        OnClickSuccesfully();
    }

    private IEnumerator Exit()
    {
        if (coroutineEnter != null)
            StopCoroutine(coroutineEnter);

        light.intensity = lightIntensityAwake;
        Vector3 targetPos = awakePos;
        Quaternion targetRot = awakeRot;

        while (Vector3.Distance(transform.position, targetPos) > 0.005f ||
               Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothHighlight * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smoothHighlight * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    public abstract void OnClickSuccesfully();

    public void Click() => isClick = true;
    public void EnterPick() => coroutineEnter = StartCoroutine(Enter());
    public void ExitPick() => coroutineEnd = StartCoroutine(Exit());
}
