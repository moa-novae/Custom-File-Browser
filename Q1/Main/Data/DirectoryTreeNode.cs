using System.Collections;
using System.Collections.Generic;
using Q1Entity;

namespace Q1
{
    public class DirectoryTreeNode : IEnumerable<DirectoryTreeNode>
    {
        private readonly Dictionary<string, DirectoryTreeNode> _children =
                                            new Dictionary<string, DirectoryTreeNode>();

        public readonly string ID;
        public DirectoryTreeNode Parent { get; private set; }
        public List<DirectoryTreeNode> Ancestors { get { return GetAncestors(); } }
        public bool? IsCriteriaMatched { get; set; } = null;

        public DirectoryItem Item { get; set; }

        public DirectoryTreeNode(string id, DirectoryItem item)
        {
            ID = id;
            Item = item;
        }

        public DirectoryTreeNode GetChild(string id)
        {
            if (_children.ContainsKey(id))
            {
                return _children[id];
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, DirectoryTreeNode> GetAllChildren()
        {
            return _children;
        }

        public void Add(DirectoryTreeNode item)
        {
            if (item.Parent != null)
            {
                item.Parent._children.Remove(item.ID);
            }

            item.Parent = this;
            _children.Add($"{ item.ID}", item);
        }


        public IEnumerator<DirectoryTreeNode> GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public int Count
        {
            get { return _children.Count; }
        }

        private List<DirectoryTreeNode> GetAncestors()
        {
            List<DirectoryTreeNode> ancestors = new List<DirectoryTreeNode>();

            void AddParent(DirectoryTreeNode node)
            {
                if (node.Parent != null)
                {
                    ancestors.Add(node.Parent);
                    AddParent(node.Parent);
                }
            }
            AddParent(this);
            return ancestors;

        }
    }
}