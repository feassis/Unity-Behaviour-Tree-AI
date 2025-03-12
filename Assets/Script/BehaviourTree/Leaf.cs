﻿namespace Utilities.BehaviourTree
{
    public class Leaf : Node
    {
        public delegate Status Tick();
        public Tick ProcessMethod;

        public Leaf() { }

        public Leaf(string name, Tick procressMethod)
        {
            Name = name;
            ProcessMethod = procressMethod;
        }

        public override Status Process()
        {
            if(ProcessMethod == null)
            {
                return Status.Failure;
            }

            return ProcessMethod();
        }
    }
}

