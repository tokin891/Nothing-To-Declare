using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent (typeof(Camera))]
public class CameraInteract : MonoBehaviour
{
    [SerializeField] float maxDistanceRay;
    [SerializeField] InputActionReference inputAction;
    [SerializeField] Image crosshairImage;

    private float catchNextRayTime;
    private float catchNextRayTriggerTime;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        inputAction.action.started += ctx => InteractWithObject();
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
                crosshairImage.color = Color.green;
            }
            else
            {
                crosshairImage.color = Color.white;
            }
        }
    }

    private void CatchRay()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistanceRay))
        {
            if(hit.collider.TryGetComponent<IInteract>(out IInteract inter))
            {
                inter.Interact();
            }
        }
    }

    private bool IsRayCatchingSomething()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistanceRay))
        {
            if (hit.collider.TryGetComponent<IInteract>(out IInteract inter))
            {
                return true;
            }
        }
        return false;
    }

    public void InteractWithObject()
    {
        if (catchNextRayTriggerTime < Time.time)
        {
            catchNextRayTriggerTime = Time.time + 1f;
            CatchRay();
        }
    }
}
