namespace Utilities.BehaviourTree
{
    public class Selector : Node
    {
        public Selector(string n)
        {
            Name = n;
        }

        public override Status Process()
        {
            if (CurrentChild >= Children.Count)
            {
                CurrentChild = 0;
                return Status.Failure;
            }


            Status childStatus = Children[CurrentChild].Process();
            if (childStatus == Status.Running)
            {
                return Status.Running;
            }

            if (childStatus == Status.Failure)
            {
                CurrentChild++;
            }

            if (childStatus == Status.Success)
            {
                CurrentChild = 0;
                return childStatus;
            }


            return Status.Running;
        }
    }
}

