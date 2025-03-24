using Utilities.BehaviourTree;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;

public class GalleryEnjoyer : BTAgent
{
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject door;
    [SerializeField] private float boredoomThresholdMin = 5;
    [SerializeField] private float boredoomThresholdMax = 12;
    [SerializeField] private List<GameObject> art = new List<GameObject>();

    private float boredomThreshold;

    private float boredom = 200;

    public bool Ticket = false;

    protected override void Awake()
    {
        base.Awake();
        boredomThreshold = Random.Range(boredoomThresholdMin, boredoomThresholdMax);

        tree = new BehaviourTree("Gallery Enjoyer Tree");

        Leaf goHome = new Leaf("Go Home", GoHome);
        RandomSelector goSeeSomeArt = new RandomSelector("Go see some art");

        for(int i = 0; i < art.Count; i++)
        {
            int index = i;
            Leaf goArt = new Leaf($"Go see {art[index].name}", () => GoSeeArt(index));
            goSeeSomeArt.AddChild(goArt);
        }

        Leaf goToDoor = new Leaf("Go To Door", () => GoToDoor(door));

        Leaf beHome = new Leaf("Be Home", BeHome);

        Loop artLoop = new Loop("Seeing the meseum", IsNotBored);
        artLoop.AddChild(goSeeSomeArt);

        Loop getTicketLoop = new Loop("Get A Ticket Loop", HasTicket);
        Leaf isWaiting = new Leaf("waiting for ticket", IsWaiting);

        getTicketLoop.AddChild(isWaiting);

        Leaf isOpen = new Leaf("Is open", IsOpen);

        DependancySequence beArtEnjoyer = new DependancySequence("Be a Art Enjoyer", IsNotGalleryOpen,  () => IsOpen());
        beArtEnjoyer.AddChild(beHome);
        beArtEnjoyer.AddChild(goToDoor);
        beArtEnjoyer.AddChild(getTicketLoop);
        beArtEnjoyer.AddChild(artLoop);
        beArtEnjoyer.AddChild(goToDoor);
        beArtEnjoyer.AddChild(goHome);

        Selector beArtEnjoyerWithFallback = new Selector("Art Enjoyer WithFallback");
        beArtEnjoyerWithFallback.AddChild(beArtEnjoyer);
        beArtEnjoyerWithFallback.AddChild(goHome);

        tree.AddChild(beArtEnjoyerWithFallback);
    }

    private bool IsNotBored() => boredom < boredomThreshold;
    private bool IsNotGalleryOpen() => IsOpen() != Node.Status.Success;

    private Node.Status BeHome()
    {
        if (!IsNotBored())
        {
            return Node.Status.Success;
        }

        return Node.Status.Running;
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GoToLocation(door.transform.position);

        if (s == Node.Status.Success)
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

    public Node.Status GoSeeArt(int index)
    {
        Node.Status status = GoToLocation(art[index].transform.position);

        if(status == Node.Status.Success)
        {
            Debug.Log($"Seeing Art {art[index].name}");
            boredom = Mathf.Clamp(boredom - 50, 0, 2000); 
        }

        return status;
    }

    private Node.Status GoHome()
    {
        Node.Status s = GoToLocation(home.transform.position);

        if(s == Node.Status.Success)
        {
            return Node.Status.Success;
        }

        return s;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(GetBored());
    }

    public bool HasTicket() => Ticket;

    private IEnumerator GetBored()
    {
        while (true)
        {
            boredom += Random.Range(1, 5);

            yield return new WaitForSeconds(1f);
        }
    }

    public Node.Status IsWaiting()
    {
        if(Blackboard.Instance.RegisterPatron(this) == this)
        {
            return Node.Status.Success;
        }

        return Node.Status.Failure;
    }
}
