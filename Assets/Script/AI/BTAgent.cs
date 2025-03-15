using UnityEngine;
using Utilities.BehaviourTree;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(NavMeshAgent))]
public class BTAgent : MonoBehaviour
{
    protected BehaviourTree tree;
    protected NavMeshAgent agent;
    protected ActionState state = ActionState.Idle;

    protected WaitForSeconds waitForSeconds;

    private const float AcceptableDistance = 3.5f;

    Node.Status treeStatus = Node.Status.Running;

    public enum ActionState
    {
        Idle = 0,
        Working = 1
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        tree = new BehaviourTree();
        waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1f));
    }

    protected virtual void Start()
    {
        StartCoroutine(Behave());
    }

    protected Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);

        if(state == ActionState.Idle)
        {
            agent.SetDestination(destination);
            state = ActionState.Working;
        }
        else if(Vector3.Distance(agent.pathEndPosition, destination) >= AcceptableDistance)
        {
            state = ActionState.Idle;
            return Node.Status.Failure;
        }
        else if(distanceToTarget < AcceptableDistance)
        {
            state = ActionState.Idle;
            return Node.Status.Success;
        }

        return Node.Status.Running;
    }
     
    protected IEnumerator Behave()
    {
        while (true)
        {
            treeStatus = tree.Process();
            yield return waitForSeconds;
        }
    }
}
