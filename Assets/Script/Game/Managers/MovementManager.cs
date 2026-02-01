using System;
using System.Collections;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager instance;

    [SerializeField] Animator camAnm;

    [SerializeField] GameObject[] canvasCenter;
    [SerializeField] GameObject[] canvasLeft;
    [SerializeField] CanvasGroup canvasGroupFade;
    [SerializeField] float speedFade = 0.25f;

    [SerializeField] Transform playerTransformDesk;
    [SerializeField] DataState dataState;

    public MoveState MoveState { private set; get; }
    public DeskState DeskState { private set; get; }

    private IStateMovement currentState;
    private float nextTimeToUpdate;

    private void Awake()
    {
        instance = this;
        Center();

        MoveState = new MoveState(dataState, playerTransformDesk);
        DeskState = new DeskState(dataState);
        MoveState.Exit();
        DeskState.Exit();
        SwitchState(MoveState);
    }

    private void Update()
    {
        if (currentState != null && nextTimeToUpdate < Time.time)
        {
            // Update aktualizowany co 0.25s dla optymalizacji
            currentState.Update();
            nextTimeToUpdate = Time.time + 0.25f;
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            SwitchState(DeskState, true);
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            SwitchState(MoveState, true);
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
        if (state == currentState)
            return;

        Debug.Log("Start swtch state");
        if(withFade)
        {
            StartCoroutine(SwitchStateFade(state));
            return;
        }

        currentState?.Exit();

        currentState = state;
        currentState.Enter();
    }

    private IEnumerator SwitchStateFade(IStateMovement state)
    {
        while(canvasGroupFade.alpha < 1f)
        {
            canvasGroupFade.alpha += speedFade * Time.deltaTime;
            Debug.Log("alpha: " +  canvasGroupFade.alpha);
            yield return null;
        }
        canvasGroupFade.alpha = 1f;
        currentState?.Exit();

        currentState = state;
        currentState.Enter();
        while (canvasGroupFade.alpha > 0)
        {
            canvasGroupFade.alpha -= speedFade * Time.deltaTime;
            yield return null;
        }
        canvasGroupFade.alpha = 0f;
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
        if(dataState.objectActiveWhenSit != null)
        {
            if (dataState.objectActiveWhenSit.Length > 0)
            {
                for (int i = 0; i < dataState.objectActiveWhenSit.Length; i++)
                {
                    dataState.objectActiveWhenSit[i].SetActive(true);
                }
            }
        }
        if (dataState.lightsActiveWhenSit != null)
        {
            if (dataState.lightsActiveWhenSit.Length > 0)
            {
                for (int i = 0; i < dataState.lightsActiveWhenSit.Length; i++)
                {
                    dataState.lightsActiveWhenSit[i].gameObject.SetActive(true);
                }
            }
        }

        Cursor.lockState = CursorLockMode.None;
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
        if (dataState.objectActiveWhenSit != null)
        {
            if (dataState.objectActiveWhenSit.Length > 0)
            {
                for (int i = 0; i < dataState.objectActiveWhenSit.Length; i++)
                {
                    dataState.objectActiveWhenSit[i].SetActive(false);
                }
            }
        }
        if (dataState.lightsActiveWhenSit != null)
        {
            if (dataState.lightsActiveWhenSit.Length > 0)
            {
                for (int i = 0; i < dataState.lightsActiveWhenSit.Length; i++)
                {
                    dataState.lightsActiveWhenSit[i].gameObject.SetActive(false);
                }
            }
        }
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
        if (dataState.objectActiveWhenMove != null)
        {
            if (dataState.objectActiveWhenMove.Length > 0)
            {
                for (int i = 0; i < dataState.objectActiveWhenMove.Length; i++)
                {
                    dataState.objectActiveWhenMove[i].SetActive(true);
                }
            }
        }
        if (dataState.lightsActiveWhenMove != null)
        {
            if (dataState.lightsActiveWhenMove.Length > 0)
            {
                for (int i = 0; i < dataState.lightsActiveWhenMove.Length; i++)
                {
                    dataState.lightsActiveWhenMove[i].gameObject.SetActive(true);
                }
            }
        }
        Cursor.lockState = CursorLockMode.Locked;
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
        if (dataState.objectActiveWhenMove != null)
        {
            if (dataState.objectActiveWhenMove.Length > 0)
            {
                for (int i = 0; i < dataState.objectActiveWhenMove.Length; i++)
                {
                    dataState.objectActiveWhenMove[i].SetActive(false);
                }
            }
        }
        if (dataState.lightsActiveWhenMove != null)
        {
            if (dataState.lightsActiveWhenMove.Length > 0)
            {
                for (int i = 0; i < dataState.lightsActiveWhenMove.Length; i++)
                {
                    dataState.lightsActiveWhenMove[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void Update()
    {

    }
}

[Serializable]
public class DataState
{
    public GameObject playerMove;
    public GameObject playerDesk;

    public Light[] lightsActiveWhenMove;
    public Light[] lightsActiveWhenSit;

    public GameObject[] objectActiveWhenMove;
    public GameObject[] objectActiveWhenSit;

    public bool doNotResetPlayerMovePositionFirstTime = true;
}
