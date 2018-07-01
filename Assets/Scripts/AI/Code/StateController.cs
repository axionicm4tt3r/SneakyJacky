using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{

    public State currentState;
    public EnemyStats enemyStats; 
    public Transform eyes; 
    public State remainState; 


    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public EnemyAI enemyAI; 
    [HideInInspector] public int nextWayPoint;
    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public Vector3 targetLastKnownPosition;
    [HideInInspector] public float stateTimeElapsed;
    [HideInInspector] public ConcurrentDictionary<string, float> stateRepeatTimers = new ConcurrentDictionary<string, float>();

    private bool aiActive;

    private float debugWireSphereRadius = 1f;

    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetupAI(true);
    }

    public void SetupAI(bool aiActivationFromTankManager)
    {
        //wayPointList = wayPointsFromTankManager;
        aiActive = aiActivationFromTankManager;
        if (aiActive)
        {
            navMeshAgent.enabled = true;
        }
        else
        {
            navMeshAgent.enabled = false;
        }
    }

    void Update()
    {
        if (!aiActive)
            return;
        ElapseStateRepeatTimers();
        currentState.UpdateState(this);
    }

    void OnDrawGizmos()
    {
        if (currentState != null && eyes != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(eyes.position, debugWireSphereRadius);
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckRepeatTimers(string name, float duration)
    {
        if (!stateRepeatTimers.ContainsKey(name))
            stateRepeatTimers.TryAdd(name, 0f);

        if (stateRepeatTimers[name] >= duration)
        {
            stateRepeatTimers[name] = 0f;
            return true;
        }

        return false;
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void ElapseStateRepeatTimers()
    {
        foreach (var key in stateRepeatTimers.Keys)
        {
            stateRepeatTimers[key] += Time.deltaTime;
        }
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }
}