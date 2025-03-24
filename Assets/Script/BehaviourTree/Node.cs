using System.Collections.Generic;

namespace Utilities.BehaviourTree
{
    public class Node
    {
        public Status CurrentStatus;
        public List<Node> Children = new List<Node>();
        public int CurrentChild = 0;
        public string Name;
        public int SortOrder;

        public Node() { }

        public Node(string name)
        {
            Name = name;
        }

        public Node(string name, int order)
        {
            Name = name;
            SortOrder = order;
        }

        public virtual void AddChild(Node n)
        {
            Children.Add(n);
        }

        public enum Status
        {
            Success = 0,
            Running = 1,
            Failure = 2
        }

        public void Reset()
        {
            foreach (var child in Children)
            {
                child.Reset();
            }

            CurrentChild = 0;
        }

        public virtual Status Process()
        {
            return Children[CurrentChild].Process();
        }

        public virtual string PrintNodeTree(int depth)
        {
            string nodeTreeDescription = string.Empty;

            nodeTreeDescription += new string('-', depth);

            if (Children.Count > 0)
            {
                nodeTreeDescription += Name;

                nodeTreeDescription += "\n";

                foreach (Node n in Children)
                {
                    nodeTreeDescription += n.PrintNodeTree(depth + 1);
                    nodeTreeDescription += "\n";
                }
            }
            else
            {
                nodeTreeDescription += $"> Leaf: {Name}";
            }

            return nodeTreeDescription;
        }
    }
}

