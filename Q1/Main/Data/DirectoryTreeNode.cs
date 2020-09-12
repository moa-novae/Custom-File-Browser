using System.Collections;
using System.Collections.Generic;
using Q1Entity;

namespace Q1
{
    // The tree nodes of the Directory tree. Each node has information such as children, parent, and other useful properties/methods
    public class DirectoryTreeNode : IEnumerable<DirectoryTreeNode>
    {
        #region Constructor
        /// <summary>
        /// Build tree node from DirectoryItem entity and id is the full path of the directoy item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        public DirectoryTreeNode(string id, DirectoryItem item)
        {
            ID = id;
            Item = item;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Full path of the directory item this tree node represents
        /// </summary>
        public readonly string ID;

        /// <summary>
        /// parent node of the current node
        /// </summary>
        public DirectoryTreeNode Parent { get; private set; }

        /// <summary>
        /// Nodes that are direct ancestors of this node
        /// </summary>
        public List<DirectoryTreeNode> Ancestors { get { return GetAncestors(); } }

        /// <summary>
        /// Checks if this node fits the search criteria 
        /// If value is null, it means that no search criteria is set and node is displayed
        /// if value is false, it will not be shown
        /// if true, node fits search criteria and will be shown
        /// </summary>
        public bool? IsCriteriaMatched { get; set; } = null;

        /// <summary>
        /// Refers to DirectoryItem entity
        /// </summary>
        public DirectoryItem Item { get; set; }

        /// <summary>
        /// Get child based on directory path
        /// </summary>
        /// <param name="id">Full directory path of child</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns all the children nodes of the current node in the form of a dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, DirectoryTreeNode> GetAllChildren()
        {
            return _children;
        }

        /// <summary>
        /// Returns the number of child nodes
        /// </summary>
        public int Count
        {
            get { return _children.Count; }
        }

        /// <summary>
        /// Adds a child node to the current node
        /// </summary>
        /// <param name="item"></param>
        public void Add(DirectoryTreeNode item)
        {
            if (item.Parent != null)
            {
                item.Parent._children.Remove(item.ID);
            }

            item.Parent = this;
            try
            {
                _children.Add($"{ item.ID}", item);
            }
            catch
            { }
        }

        private readonly Dictionary<string, DirectoryTreeNode> _children =
                                           new Dictionary<string, DirectoryTreeNode>();

        #endregion

        #region Enumerator

        /// <summary>
        /// Allows the child nodes to be enumerated over
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DirectoryTreeNode> GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Helper method

        /// <summary>
        /// property for storing the child nodes
        /// </summary>
        /// <summary>
        /// Helper method for finding all direct ancestors 
        /// </summary>
        /// <returns></returns>
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

        #endregion
    }
}