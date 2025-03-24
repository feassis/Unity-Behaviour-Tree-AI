using UnityEngine;
using Utilities.BehaviourTree;

public class Worker : BTAgent
{
    public GameObject Office;

    protected override void Awake()
    {
        base.Awake();

        Leaf goToPatreon = new Leaf("Go to patreon", GoToPatreon);
        Leaf goToOffice = new Leaf("Go to Office", GoToOffice);

        Selector beWorker = new Selector("Be worker");
        beWorker.AddChild(goToPatreon);
        beWorker.AddChild(goToOffice);

        tree.AddChild(beWorker);
    }

    protected override void Start()
    {
        base.Start();
    }

    public Node.Status GoToPatreon()
    {
        if(Blackboard.Instance.patreon == null) return Node.Status.Failure;

        Node.Status status = GoToLocation(Blackboard.Instance.patreon.transform.position);

        if(status == Node.Status.Success)
        {
            Blackboard.Instance.patreon.Ticket = true;
            Blackboard.Instance.patreon = null;
            return Node.Status.Success;
        }

        return status;
    }

    public Node.Status GoToOffice()
    {
        Node.Status s = GoToLocation(Office.transform.position);

        return s;
    }
}
