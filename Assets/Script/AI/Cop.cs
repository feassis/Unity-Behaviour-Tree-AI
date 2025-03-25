using System.Collections.Generic;
using UnityEngine;
using Utilities.BehaviourTree;
using static UnityEditor.FilePathAttribute;

public class Cop : BTAgent
{
    [SerializeField] private List<GameObject> pathwayPoints  = new List<GameObject>();
    [SerializeField] private RobberBehaviour robber;
    [SerializeField] private float seeDistance = 8f;
    [SerializeField] private float seeAngle = 75f;
    [SerializeField] private float chaseDistance = 8f;
    

    protected override void Awake()
    {
        base.Awake();

        DependancySequence selectPatrolPoints = 
            new DependancySequence("Select Patrol Points", RobberIsOnSight, () => agent.ResetPath());

        for (int i = 0; i < pathwayPoints.Count; i++)
        {
            int index = i;

            Leaf goToPathwayPoint = new Leaf($"Go to pathway {index}", () => GoToPatrolPathway(index));
            selectPatrolPoints.AddChild(goToPathwayPoint);
        }

        Sequence chaseRobberSequence = new Sequence("Chase");
        Leaf canSeeRobber = new Leaf("Can See Robber", CanSeeRobber);
        Leaf chaseRobber = new Leaf("Chase Robber", ChaseRobber);

        chaseRobberSequence.AddChild(canSeeRobber);
        chaseRobberSequence.AddChild(chaseRobber);

        Selector beCop = new Selector("Be cop");

        beCop.AddChild(chaseRobberSequence);
        beCop.AddChild(selectPatrolPoints);

        tree.AddChild(beCop);
    }

    private bool RobberIsOnSight() => CanSeeRobber() == Node.Status.Success;

    private Node.Status CanSeeRobber()
    {
        return CanSee(robber.transform.position, "Robber", seeDistance, seeAngle);
    }

    private Node.Status ChaseRobber()
    {
        if (state == ActionState.Idle)
        {
            remmemberedLocation = this.transform.position - (transform.position - robber.transform.position).normalized * chaseDistance;
        }
        return GoToLocation(remmemberedLocation);
    }

    private Node.Status GoToPatrolPathway(int index)
    {
        return GoToLocation(pathwayPoints[index].transform.position);
    }
}
