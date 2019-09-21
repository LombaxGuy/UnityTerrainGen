using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    public State currentState;
    public Transform eyes;
    public State remainState;

    private bool aiActive = true;

    public Transform playerTransform;

    public Transform target;
    public Vector3 targetLastPosition;

    public float viewDistance = 10;
    public float fieldOfViewAngle = 120;

    [HideInInspector] public NavMeshAgent navMeshAgent;

    public List<Transform> wayPointList;
    public int nextWayPoint;

    [HideInInspector] public float searchStateDuriation = 20;
    [HideInInspector] public float searchDuration = 2;
    [HideInInspector] public float searchRadius = 3;

    private float stateTimeElapsed1 = 0;
    private float stateTimeElapsed2 = 0;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	private void Update ()
    {
        if (!aiActive)
            return;

        currentState.UpdateState(this);
	}

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CountDownElapsed1(float duration)
    {
        stateTimeElapsed1 += Time.deltaTime;
        return (stateTimeElapsed1 >= duration);
    }

    public bool CountDownElapsed2(float duration)
    {
        stateTimeElapsed2 += Time.deltaTime;
        return (stateTimeElapsed2 >= duration);
    }

    public void ResetCountDown2()
    {
        stateTimeElapsed2 = 0;
    }

    public void OnExitState()
    {
        stateTimeElapsed1 = 0;
        stateTimeElapsed2 = 0;
    }

    public Vector3 GetNavSpherePoint(Vector3 origin, float distance, int layermask = -1)
    {
        Vector3 randomDirection = Random.insideUnitSphere;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, 1, layermask);

        randomDirection = ((navHit.position - origin)).normalized;

        randomDirection *= Random.Range(distance, distance * 1.5f);

        return randomDirection;
    }

    private void OnDrawGizmos()
    {
        if (currentState != null && eyes != null)
        {
            Gizmos.color = currentState.gizmoStateColor;
            Gizmos.DrawWireSphere(eyes.position, 0.5f);

            Gizmos.DrawRay(eyes.position, eyes.forward * viewDistance);
            Gizmos.DrawRay(eyes.position, Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * (eyes.forward * viewDistance));
            Gizmos.DrawRay(eyes.position, Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * (eyes.forward * viewDistance));
        }

        if (navMeshAgent)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(navMeshAgent.destination, 0.5f);
        }
    }
}
