using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementManager : MonoBehaviour
{
    [SerializeField] Animator camAnm;

    [SerializeField] GameObject[] canvasCenter;
    [SerializeField] GameObject[] canvasLeft;
    [SerializeField] CanvasGroup canvasGroupFade;
    [SerializeField] float speedFade = 0.25f;

    [SerializeField] Transform playerTransformDesk;
    [SerializeField] DataState dataState;
    [SerializeField] GameObject[] allBuletsObject;

    public MoveState MoveState { private set; get; }
    public DeskState DeskState { private set; get; }
    public ShootState ShootState { private set; get; }

    public IStateMovement CurrentState { private set; get; }
    public int CurrentBullet { private set; get; }

    private float nextTimeToUpdate;
    private int maxBullet;

    private void Awake()
    {
        maxBullet = allBuletsObject.Length;
        CurrentBullet = maxBullet;
        Center();

        MoveState = new MoveState(dataState, playerTransformDesk);
        DeskState = new DeskState(dataState);
        ShootState = new ShootState(dataState, playerTransformDesk);
        MoveState.Exit();
        DeskState.Exit();
        ShootState.Exit();
        SwitchState(MoveState);
    }

    private void Update()
    {
        if (CurrentState != null && nextTimeToUpdate < Time.time)
        {
            // Update aktualizowany co 0.25s dla optymalizacji
            CurrentState.Update();
            nextTimeToUpdate = Time.time + 0.25f;
        }
    }

    public void Left()
    {
        camAnm.SetInteger("state", 1);

        for (int i = 0; i < canvasCenter.Length; i++)
        {
            canvasCenter[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < canvasLeft.Length; i++)
        {
            canvasLeft[i].gameObject.SetActive(true);
        }
    }

    public void Center()
    {
        camAnm.SetInteger("state", 0);

        for (int i = 0; i < canvasCenter.Length; i++)
        {
            canvasCenter[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < canvasLeft.Length; i++)
        {
            canvasLeft[i].gameObject.SetActive(false);
        }
    }

    public void SwitchState(IStateMovement state, bool withFade = false)
    {
        if (state == CurrentState)
            return;

        Debug.Log("Start swtch state");
        if(withFade)
        {
            StartCoroutine(SwitchStateFade(state));
            return;
        }

        CurrentState?.Exit();

        CurrentState = state;
        CurrentState.Enter();
    }

    private IEnumerator SwitchStateFade(IStateMovement state)
    {
        while(canvasGroupFade.alpha < 1f)
        {
            canvasGroupFade.alpha += speedFade * Time.deltaTime;
            yield return null;
        }
        canvasGroupFade.alpha = 1f;
        CurrentState?.Exit();

        CurrentState = state;
        CurrentState.Enter();
        while (canvasGroupFade.alpha > 0)
        {
            canvasGroupFade.alpha -= speedFade * Time.deltaTime;
            yield return null;
        }
        canvasGroupFade.alpha = 0f;
    }

    public void RemoveBullet()
    {
        if(CurrentBullet > 0)
        {
            allBuletsObject[CurrentBullet-1].gameObject.SetActive(false);
            CurrentBullet--;
        }
    }
}

public interface IStateMovement
{
    public void Enter();
    public void Exit();
    public void Update();
}

public class DeskState : IStateMovement
{
    private DataState dataState;

    public DeskState(DataState dataState)
    {
        this.dataState = dataState;
    }

    public void Enter()
    {
        if(dataState == null)
        {
            return;
        }
        if(dataState.playerDesk != null)
        {
            dataState.playerDesk.SetActive(true);
        }
        Extensions.SetActiveAll(dataState.objectActiveWhenSit, true);
        Extensions.SetEnabledAll(dataState.lightsActiveWhenSit, true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Exit()
    {
        if (dataState == null)
        {
            return;
        }
        if (dataState.playerDesk != null)
        {
            dataState.playerDesk.SetActive(false);
        }
        Extensions.SetActiveAll(dataState.objectActiveWhenSit, false);
        Extensions.SetEnabledAll(dataState.lightsActiveWhenSit, false);        
    }

    public void Update()
    {
        
    }
}

public class MoveState : IStateMovement
{
    private DataState dataState;
    Transform playerTransformDesk;

    public MoveState(DataState dataState, Transform playerTransformDesk)
    {
        this.playerTransformDesk = playerTransformDesk;
        this.dataState = dataState;
    }

    public void Enter()
    {
        if (dataState == null)
        {
            return;
        }
        if (dataState.playerMove != null)
        {
            if (!dataState.doNotResetPlayerMovePositionFirstTime)
            {
                dataState.playerMove.transform.position = playerTransformDesk.position;
                dataState.playerMove.transform.rotation = playerTransformDesk.rotation;
            }else
                dataState.doNotResetPlayerMovePositionFirstTime = false;

            dataState.playerMove.SetActive(true);
        }
        Extensions.SetActiveAll(dataState.objectActiveWhenMove, true);
        Extensions.SetEnabledAll(dataState.lightsActiveWhenMove, true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Exit()
    {
        if (dataState == null)
        {
            return;
        }
        if (dataState.playerMove != null)
        {
            dataState.playerMove.SetActive(false);
        }
        Extensions.SetActiveAll(dataState.objectActiveWhenMove, false);
        Extensions.SetEnabledAll(dataState.lightsActiveWhenMove, false);
    }

    public void Update()
    {

    }
}

public class ShootState : IStateMovement
{
    private DataState dataState;
    Transform playerTransformDesk;

    public ShootState(DataState dataState, Transform playerTransformDesk)
    {
        this.dataState = dataState;
        this.playerTransformDesk = playerTransformDesk;
    }

    public void Enter()
    {
        if(GameManager.Instance.MovementManager.CurrentBullet <= 0)
        {
            GameManager.Instance.MovementManager.SwitchState(GameManager.Instance.MovementManager.DeskState);
            return;
        }

        if (dataState == null)
        {
            return;
        }
        if (dataState.playerShoot != null)
        {
            dataState.playerShoot.SetActive(true);
        }

        Extensions.SetActiveAll(dataState.objectActiveWhenShoot, true);
        Extensions.SetEnabledAll(dataState.lightsActiveWhenShoot, true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        dataState.inputActionUndoShoot.action.Enable();
        dataState.inputActionUndoShoot.action.started += ctx => UndoShoot();
    }

    public void Exit()
    {
        if (dataState == null)
        {
            return;
        }
        if (dataState.playerShoot != null)
        {
            dataState.playerShoot.SetActive(false);
        }

        Extensions.SetActiveAll(dataState.objectActiveWhenShoot, false);
        Extensions.SetEnabledAll(dataState.lightsActiveWhenShoot, false);
        dataState.inputActionUndoShoot.action.Disable();
    }

    public void Update()
    {
        
    }

    private void UndoShoot()
    {
        dataState.inputActionUndoShoot.action.Disable();
        GameManager.Instance.MovementManager.SwitchState(GameManager.Instance.MovementManager.DeskState, true);
    }
}

[Serializable]
public class DataState
{
    public GameObject playerMove;
    public GameObject playerDesk;
    public GameObject playerShoot;

    public Light[] lightsActiveWhenMove;
    public Light[] lightsActiveWhenSit;
    public Light[] lightsActiveWhenShoot;

    public GameObject[] objectActiveWhenMove;
    public GameObject[] objectActiveWhenSit;
    public GameObject[] objectActiveWhenShoot;

    public bool doNotResetPlayerMovePositionFirstTime = true;

    public InputActionReference inputActionUndoShoot;
}
