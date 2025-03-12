using UnityEngine;
using Utilities.BehaviourTree;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    public GameObject Diamond;
    public GameObject Van;
    public GameObject BackDoor;
    public GameObject FrontDoor;
    protected BehaviourTree tree;
    protected NavMeshAgent agent;
    ActionState state = ActionState.Idle;

    private const float AcceptableDistance = 3.5f;

    Node.Status treeStatus = Node.Status.Running;

    public enum ActionState
    {
        Idle = 0,
        Working = 1
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        tree = new BehaviourTree();

        Sequence steal = new Sequence("Steal Something");
        Selector openDoor = new Selector("Open Door");
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);

        openDoor.AddChild(goToBackDoor);
        openDoor.AddChild(goToFrontDoor);

        steal.AddChild(openDoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        tree.AddChild(steal);
    }

    public Node.Status GoToDiamond()
    {
        return GoToLocation(Diamond.transform.position);
    }

    public Node.Status GoToVan()
    {
        return GoToLocation(Van.transform.position);
    }

    public Node.Status GoToBackDoor()
    {
        return GoToLocation(BackDoor.transform.position);
    }

    public Node.Status GoToFrontDoor()
    {
        return GoToLocation(FrontDoor.transform.position);
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

    private void Start()
    {
        Debug.Log(tree.PrintTree());
    }

    private void Update()
    {
        if(treeStatus == Node.Status.Running)
        {
            treeStatus = tree.Process();
        }
    }
}
