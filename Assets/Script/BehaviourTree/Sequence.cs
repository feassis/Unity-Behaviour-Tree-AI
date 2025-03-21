﻿using UnityEngine;

namespace Utilities.BehaviourTree
{
    public class Sequence : Node
    {
        public Sequence(string n)
        {
            Name = n;
        }

        public override Status Process()
        {
            if(CurrentChild >= Children.Count)
            {
                CurrentChild = 0;
                return Status.Success;
            }


            Status childStatus = Children[CurrentChild].Process();
            if (childStatus == Status.Running)
            {
                return Status.Running;
            }

            if(childStatus == Status.Failure)
            {
                return childStatus;
            }

            if(childStatus == Status.Success) 
            {
                CurrentChild++;
            }


            return Status.Running;
        }
    }
}

