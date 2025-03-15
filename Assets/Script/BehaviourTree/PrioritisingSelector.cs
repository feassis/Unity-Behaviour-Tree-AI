namespace Utilities.BehaviourTree
{
    public class PrioritisingSelector : Node
    {
        Node[] nodeArray;
        bool sorted = false;

        public PrioritisingSelector(string n)
        {
            Name = n;
        }

        protected void OrderNodes()
        {
            nodeArray = Children.ToArray();
            Sort(nodeArray, 0, Children.Count - 1);
            Children = new System.Collections.Generic.List<Node> (nodeArray);
        }

        public override Status Process()
        {
            if (!sorted)
            {
                OrderNodes();
                sorted = true;
            }
            if (CurrentChild >= Children.Count)
            {
                CurrentChild = 0;
                sorted = false;
                return Status.Failure;
            }


            Status childStatus = Children[CurrentChild].Process();
            if (childStatus == Status.Running)
            {
                return Status.Running;
            }

            if (childStatus == Status.Failure)
            {
                Children[CurrentChild].SortOrder = Children.Count + 1;
                CurrentChild++;
            }

            if (childStatus == Status.Success)
            {
                Children[CurrentChild].SortOrder = 1;
                CurrentChild = 0;
                sorted = false;
                return childStatus;
            }


            return Status.Running;
        }

        //QuickSort
        //Adapted from: https://exceptionnotfound.net/quick-sort-csharp-the-sorting-algorithm-family-reunion/
        int Partition(Node[] array, int low,
                                    int high)
        {
            Node pivot = array[high];

            int lowIndex = (low - 1);

            //2. Reorder the collection.
            for (int j = low; j < high; j++)
            {
                if (array[j].SortOrder <= pivot.SortOrder)
                {
                    lowIndex++;

                    Node temp = array[lowIndex];
                    array[lowIndex] = array[j];
                    array[j] = temp;
                }
            }

            Node temp1 = array[lowIndex + 1];
            array[lowIndex + 1] = array[high];
            array[high] = temp1;

            return lowIndex + 1;
        }

        void Sort(Node[] array, int low, int high)
        {
            if (low < high)
            {
                int partitionIndex = Partition(array, low, high);
                Sort(array, low, partitionIndex - 1);
                Sort(array, partitionIndex + 1, high);
            }
        }
    }
}

