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
        instance = this;
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
        if(MovementManager.instance.CurrentBullet <= 0)
        {
            MovementManager.instance.SwitchState(MovementManager.instance.DeskState);
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
        if (dataState.objectActiveWhenShoot != null)
        {
            if (dataState.objectActiveWhenShoot.Length > 0)
            {
                for (int i = 0; i < dataState.objectActiveWhenShoot.Length; i++)
                {
                    dataState.objectActiveWhenShoot[i].SetActive(true);
                }
            }
        }
        if (dataState.lightsActiveWhenShoot != null)
        {
            if (dataState.lightsActiveWhenShoot.Length > 0)
            {
                for (int i = 0; i < dataState.lightsActiveWhenShoot.Length; i++)
                {
                    dataState.lightsActiveWhenShoot[i].gameObject.SetActive(true);
                }
            }
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (dataState.objectActiveWhenShoot != null)
        {
            if (dataState.objectActiveWhenShoot.Length > 0)
            {
                for (int i = 0; i < dataState.objectActiveWhenShoot.Length; i++)
                {
                    dataState.objectActiveWhenShoot[i].SetActive(false);
                }
            }
        }
        if (dataState.lightsActiveWhenShoot != null)
        {
            if (dataState.lightsActiveWhenShoot.Length > 0)
            {
                for (int i = 0; i < dataState.lightsActiveWhenShoot.Length; i++)
                {
                    dataState.lightsActiveWhenShoot[i].gameObject.SetActive(false);
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
    public GameObject playerShoot;

    public Light[] lightsActiveWhenMove;
    public Light[] lightsActiveWhenSit;
    public Light[] lightsActiveWhenShoot;

    public GameObject[] objectActiveWhenMove;
    public GameObject[] objectActiveWhenSit;
    public GameObject[] objectActiveWhenShoot;

    public bool doNotResetPlayerMovePositionFirstTime = true;
}
