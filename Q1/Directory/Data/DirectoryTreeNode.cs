using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//https://stackoverflow.com/questions/9860207/build-a-simple-high-performance-tree-data-structure-in-c-sharp
namespace Q1
{
    public class DirectoryTreeNode : IEnumerable<DirectoryTreeNode>
    {
        private readonly Dictionary<string, DirectoryTreeNode> _children =
                                            new Dictionary<string, DirectoryTreeNode>();

        public readonly string ID;
        public DirectoryTreeNode Parent { get; private set; }

        public DirectoryItem Item { get; private set; }

        public DirectoryTreeNode(string id, DirectoryItem item)
        {
            this.ID = id;
            this.Item = item;
        }

        public DirectoryTreeNode GetChild(string id)
        {
            return this._children[id];
        }

        public Dictionary<string, DirectoryTreeNode> GetAllChildren ()
        {
            return this._children;
        }

        public void Add(DirectoryTreeNode item)
        {
            if (item.Parent != null)
            {
                item.Parent._children.Remove(item.ID);
            }

            item.Parent = this;
            this._children.Add(item.ID, item);
        }

        public IEnumerator<DirectoryTreeNode> GetEnumerator()
        {
            return this._children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

       
        public int Count
        {
            get { return this._children.Count; }
        }
    }
}