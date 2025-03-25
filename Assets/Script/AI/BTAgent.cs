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
    protected Vector3 remmemberedLocation;

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

    public Node.Status CanSee(Vector3 target, string tag, float maxDistance, float maxAngle)
    {
        Vector3 directionToTarget = target - this.transform.position;
        float angle = Vector3.Angle(directionToTarget, transform.forward);

        if(angle <= maxAngle || directionToTarget.magnitude <= maxDistance)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(this.transform.position, directionToTarget, out hitInfo))
            {
                if (hitInfo.collider.gameObject.CompareTag(tag))
                {
                    return Node.Status.Success;
                }
            }
        }

        return Node.Status.Failure;
    }

    public Node.Status Flee(Vector3 location, float fleeDistance)
    {
        if(state == ActionState.Idle)
        {
            remmemberedLocation = this.transform.position + (transform.position - location).normalized * fleeDistance;
        }
        return GoToLocation(remmemberedLocation);
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


    public Node.Status IsOpen()
    {
        if(Blackboard.Instance.timeOfDay < Blackboard.Instance.openingTime 
            || Blackboard.Instance.timeOfDay > Blackboard.Instance.closingTime)
        {
            return Node.Status.Failure;
        }
        return Node.Status.Success;
    }
}
