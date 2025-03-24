namespace Utilities.BehaviourTree
{
    public class Leaf : Node
    {
        public delegate Status Tick();
        public Tick ProcessMethod;

        public delegate Status TickM(int index);
        public TickM ProcessMethodM;

        public int Index;

        public Leaf() { }

        public Leaf(string name, Tick procressMethod)
        {
            Name = name;
            ProcessMethod = procressMethod;
        }

        public Leaf(string name, TickM procressMethodM, int index)
        {
            Name = name;
            ProcessMethodM = procressMethodM;
            Index = index;
        }

        public Leaf(string name, Tick procressMethod, int order)
        {
            Name = name;
            ProcessMethod = procressMethod;
            SortOrder = order;
        }

        public Leaf(string name, TickM procressMethodM,int index, int order)
        {
            Name = name;
            ProcessMethodM = procressMethodM;
            SortOrder = order;
            Index = index;
        }

        public override Status Process()
        {
            Status s = Status.Failure;

            if (ProcessMethod != null)
            {
                s = ProcessMethod();
            }
            else if(ProcessMethodM != null)
            {
                s = ProcessMethodM(Index);
            }

            //UnityEngine.Debug.Log($"{Name} - {s}");
            return s;
        }
    }
}

