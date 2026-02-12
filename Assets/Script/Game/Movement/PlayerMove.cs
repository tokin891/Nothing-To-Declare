using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;

    [Header("References")] 
    public Transform cameraTransform; // kamera gracza
    [Header("Settings")]
    private float playerSpeed = 5.0f;
    public float sensitivity = 2f; 
    public float verticalClamp = 80f; 
    private float pitch = 0f;
    [SerializeField] bool lockMove;
    [SerializeField] float delayFS = 1.5f;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;

    private Vector3 lastPosition;
    private float checkLastPosition;
    private float updateLastPosition;

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
    }


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        Move();
        Look();

        if(lastPosition != transform.position)
        {
            lastPosition = transform.position;
            if (Time.time > checkLastPosition)
            {
                checkLastPosition = Time.time + delayFS;
                GameManager.Instance.SoundsManager.PlayFS();
            }
        }
    }

    private void Move()
    {
        if (lockMove)
            return;

        // Input
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);

        // Kierunek wzglêdem kamery
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Finalny kierunek ruchu
        Vector3 finalMove = camForward * move.z + camRight * move.x;

        // Prêdkoœæ
        finalMove *= playerSpeed;

        // Move
        controller.Move(finalMove * Time.deltaTime);
    }

    private void Look()
    {
        // Pobranie inputu
        Vector2 input = lookAction.action.ReadValue<Vector2>();

        // Skalowanie czu³oœci¹
        float mouseX = input.x * sensitivity * Time.deltaTime;
        float mouseY = input.y * sensitivity * Time.deltaTime;

        // Obrót poziomy (yaw) – obracamy postaæ
        transform.Rotate(Vector3.up * mouseX);

        // Obrót pionowy (pitch) – obracamy kamerê
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

}
