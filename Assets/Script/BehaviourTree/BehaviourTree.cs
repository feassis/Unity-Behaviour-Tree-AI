
using System.Diagnostics;

namespace Utilities.BehaviourTree
{
    public class BehaviourTree : Node
    {
        public BehaviourTree()
        {
            Name = "Tree";
        }

        public BehaviourTree(string name) : base(name)
        {
        }

        public override Status Process()
        {
            return Children[CurrentChild].Process();
        }

        public string PrintTree()
        {
            string treeDescription = string.Empty;
            treeDescription += Name + "\n";

            foreach (var child in Children)
            {
                treeDescription += child.PrintNodeTree(1);
                treeDescription += "\n";
            }

            return treeDescription;
        }
    }


}

