using System;
using static Unity.VisualScripting.Metadata;

namespace Utilities.BehaviourTree
{
    public class DependancySequence : Node
    {
        private Func<bool> abortCondition;
        private Action abortAction;

        public DependancySequence(string n, System.Func<bool> abortCondition, System.Action abortAction)
        {
            Name = n;
            this.abortCondition = abortCondition;
            this.abortAction = abortAction;
        }

        public override Status Process()
        {
            if (abortCondition())
            {
                abortAction();
                foreach (var n in Children)
                {
                    n.Reset();
                }
                return Status.Failure;
            }

            if (CurrentChild >= Children.Count)
            {
                CurrentChild = 0;
                return Status.Success;
            }

            Status childStatus = Children[CurrentChild].Process();

            if (childStatus == Status.Running)
            {
                return Status.Running;
            }

            if (childStatus == Status.Failure)
            {
                return childStatus;
            }

            if (childStatus == Status.Success)
            {
                CurrentChild++;
            }


            return Status.Running;
        }

    }
}

