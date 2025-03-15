namespace Utilities.BehaviourTree
{
    public class Inverter : Node
    {
        public Inverter(string n)
        {
            Name = n;
        }

        public override void AddChild(Node n)
        {
            Children = new System.Collections.Generic.List<Node>();

            Children.Add(n);
        }


        public override Status Process()
        {
            Status childStatus = Children[0].Process();

            if (childStatus == Status.Failure)
            {
                return Status.Success;
            }

            if (childStatus == Status.Success)
            {
                return Status.Failure;
            }


            return Status.Running;
        }
    }
}

