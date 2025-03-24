
namespace Utilities.BehaviourTree
{
    public class RandomSelector : Node
    {
        bool shuffled = false;
        public RandomSelector(string n)
        {
            Name = n;
        }

        public override Status Process()
        {
             if (!shuffled)
            {
                Children.Shuffle();
                shuffled = true;
            }


            if (CurrentChild >= Children.Count)
            {
                CurrentChild = 0;
                shuffled = false;
                return Status.Failure;
            }


            Status childStatus = Children[CurrentChild].Process();
            if (childStatus == Status.Running)
            {
                return Status.Running;
            }

            if (childStatus == Status.Failure)
            {
                CurrentChild = 0;
                shuffled = false;
                return Status.Failure;
            }

            if (childStatus == Status.Success)
            {
                CurrentChild++;
                return childStatus;
            }


            return Status.Running;
        }
    }
}
