using UnityEngine;
using Utilities.BehaviourTree;
using UnityEngine.AI;
using System;

public class RobberBehaviour : BTAgent
{
    public GameObject Diamond;
    public GameObject Paint;
    public GameObject[] stealables;
    public GameObject Van;
    public GameObject BackDoor;
    public GameObject FrontDoor;
    public GameObject Cop;
    [Range(0f, 1000f)] public int money = 800;
    [SerializeField] private float fleeDistance = 10f;

    Node.Status treeStatus = Node.Status.Running;

    protected GameObject heldItem;


    protected override void Awake()
    {
        base.Awake();

        Sequence steal = new Sequence("Steal Something");
        Leaf hasMoney = new Leaf("Needs Money", HasMoney);
        PrioritisingSelector openDoor = new PrioritisingSelector("Open Door");
        RandomSelector selectObjectToSteal = new RandomSelector("Select Object To Steal");
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor, 2);
        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor, 1);

        for (int i = 0; i < stealables.Length; i++)
        {
            int index = i; 
            Leaf goStealItem = new Leaf($"Steal Item {stealables[index].name}", () => GoStealItem(index));
            selectObjectToSteal.AddChild(goStealItem);
        }

        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Inverter invertMoney = new Inverter("Invert Money");
        Inverter invertCanSeeCop = new Inverter("Can't see Cop");

        Sequence lookForCop = new Sequence("Look For Cop");
        Leaf canSee = new Leaf("Can See Cop", CanSeeCop);
        Leaf flee = new Leaf("Flee Cop", FleeFromCop);
        
        Selector beThief = new Selector("Be A Thief");

        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);

        invertMoney.AddChild(hasMoney);
        invertCanSeeCop.AddChild(canSee);

        steal.AddChild(invertMoney);
        steal.AddChild(invertCanSeeCop);
        steal.AddChild(openDoor);
        steal.AddChild(invertCanSeeCop);
        steal.AddChild(selectObjectToSteal);
        steal.AddChild(invertCanSeeCop);
        steal.AddChild(goToVan);

        lookForCop.AddChild(canSee);
        lookForCop.AddChild(flee);        

        beThief.AddChild(lookForCop);
        beThief.AddChild(steal);
        
        tree.AddChild(beThief);
    }

    public Node.Status CanSeeCop()
    {
        return CanSee(Cop.transform.position, "Cop", fleeDistance, 90);
    }

    public Node.Status FleeFromCop()
    {
        return Flee(Cop.transform.position, fleeDistance);
    }

    private Node.Status GoStealItem(int index)
    {
        Debug.Log($"index {index}");
        if(stealables[index].activeSelf == false)
        {
            return Node.Status.Failure;
        }

        Node.Status s = GoToLocation(stealables[index].transform.position);

        if (s == Node.Status.Success)
        {
            stealables[index].transform.parent = this.transform;
            heldItem = stealables[index];
            return Node.Status.Success;
        }
        else
        {
            return s;
        }
    }

    public Node.Status HasMoney()
    {
        if(money < 500)
        {
            return Node.Status.Failure;
        }

        return Node.Status.Success;
    }

    public Node.Status GoToVan()
    {
        Node.Status s = GoToLocation(Van.transform.position);
        if (s == Node.Status.Success)
        {
            heldItem.SetActive(false);
            money += 300;
            return Node.Status.Success;
        }
        else
        {
            return s;
        }
    }

    public Node.Status GoToBackDoor()
    {
        return GoToDoor(BackDoor);
    }

    public Node.Status GoToFrontDoor()
    {
        return GoToDoor(FrontDoor);
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GoToLocation(door.transform.position);

        if(s == Node.Status.Success)
        {
            if (!door.GetComponent<Lock>().IsLocked)
            {
                door.GetComponent<NavMeshObstacle>().enabled = false;
                return Node.Status.Success;
            }

            return Node.Status.Failure;
        }
        else
        {
            return s;
        }
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log(tree.PrintTree());
    }
}
