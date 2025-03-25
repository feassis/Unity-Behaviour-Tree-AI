using UnityEngine;
using Utilities.BehaviourTree;

public class Worker : BTAgent
{
    public GameObject Office;

    GalleryEnjoyer patreon;

    protected override void Awake()
    {
        base.Awake();

        Leaf goToPatreon = new Leaf("Go to patreon", GoToPatreon);
        Leaf goToOffice = new Leaf("Go to Office", GoToOffice);
        Leaf allocatePatreon = new Leaf("Allocate patreon", AllocatePatron);

        DependancySequence goToPatreonIfOpen = new DependancySequence("Try to go to patreon", IsClosed, () => agent.ResetPath());
        goToPatreonIfOpen.AddChild(goToPatreon);

        Sequence serveClient = new Sequence("Serve Clients");
        serveClient.AddChild(allocatePatreon);
        serveClient.AddChild(goToPatreonIfOpen);

        Selector beWorker = new Selector("Be worker");
        beWorker.AddChild(serveClient);
        beWorker.AddChild(goToOffice);



        tree.AddChild(beWorker);
    }

    private bool IsClosed() => IsOpen() == Node.Status.Failure;

    protected override void Start()
    {
        base.Start();
    }

    public Node.Status AllocatePatron()
    {
        if (Blackboard.Instance.patreon.Count == 0 && patreon == null) return Node.Status.Failure;

        if (patreon == null)
        {
            patreon = Blackboard.Instance.patreon.Pop();
        }

        return Node.Status.Success;
    }

    public Node.Status GoToPatreon()
    {
        if (patreon == null) return Node.Status.Failure;

        Node.Status status = GoToLocation(patreon.transform.position);

        if(status == Node.Status.Success)
        {
            patreon.Ticket = true;
            patreon = null;
            return Node.Status.Success;
        }

        return status;
    }

    public Node.Status GoToOffice()
    {
        Node.Status s = GoToLocation(Office.transform.position);
        patreon = null;
        return s;
    }
}
