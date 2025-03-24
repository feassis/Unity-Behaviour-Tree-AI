using System;

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
            if (CurrentChild >= Children.Count)
            {
                this.Reset();
                return Status.Success;
            }


            Status childStatus = Children[CurrentChild].Process();
            if (childStatus == Status.Running)
            {
                return Status.Running;
            }

            if(childStatus == Status.Failure)
            {
                this.Reset();
                return childStatus;
            }

            if(childStatus == Status.Success) 
            {
                CurrentChild++;
            }


            return Status.Running;
        }
    }

    public class Loop : Node
    {
        Func<bool> abortCondition;
        public Loop(string n, System.Func<bool> abortCondition)
        {
            Name = n;
            this.abortCondition = abortCondition;
        }

        public override Status Process()
        {
            if (abortCondition())
            {
                this.Reset();
                return Status.Success;
            }


            Status childStatus = Children[CurrentChild].Process();
            if (childStatus == Status.Running)
            {
                return Status.Running;
            }

            if (childStatus == Status.Failure)
            {
                this.Reset();
                return childStatus;
            }

            CurrentChild++;

            if(CurrentChild >= Children.Count)
            {
                CurrentChild = 0;
            }

            return Status.Running;
        }
    }
}

