using System.Threading;
using UnityEngine;


public class WendigoStateMachine : MonoBehaviour
{
    private WendigoResting resting;
    private StalkingBehaviour stalking;
    private WendigoChasing chasing;
    private WendigoBehaviour activeState = null;
    private WendigoBehaviour nextState = null;
    private GameManager gameManager;

    public int maxPlayerSightings = 2;
    public float maxSearchTime = 60.0f;

    void Start()
    {
        resting = GetComponent<WendigoResting>();
        stalking = GetComponent<StalkingBehaviour>();
        chasing = GetComponent<WendigoChasing>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        activeState = resting;
        nextState = null;
    }

    void SetNewState(WendigoBehaviour newState)
    {
        nextState = newState;
        if (activeState)
        {
            activeState.ExitState();
        }
        else
        {
            UpdateStateTransition();
        }
    }

    void UpdateStateTransition()
    {
        if (nextState != null)
        {
            if (activeState == null || activeState.isComplete)
            {
                activeState = nextState;
                nextState = null;
                activeState.EnterState();
            }
        }
    }

    void UpdateActiveState()
    {
        if (activeState)
        {
            activeState.Run();
        }
    }

    void UpdateCurrentState()
    {
        if (!gameManager.isNight || gameManager.safeArea)
        {   
            Debug.Log("Entering Resting State");
            SetNewState(resting);
        }
        if (activeState == resting && gameManager.isNight && !gameManager.safeArea)
        {
            Debug.Log("Entering Stalking State");
            SetNewState(stalking);
        }
        if (activeState == stalking && stalking.playerSightings >= maxPlayerSightings)
        {
            Debug.Log("Entering Chasing State");
            SetNewState(chasing);
        }
        if (activeState == chasing && chasing.searchTime > maxSearchTime)
        {
            Debug.Log("Entering Stalking State");
            SetNewState(stalking);
        }
        // if (// lure isn't placed && player in lair)
        // {
        //     SetNewState(chasing);
        // }
    }

    void Update()
    {
        UpdateCurrentState();
        UpdateActiveState();
        UpdateStateTransition();
    }
}