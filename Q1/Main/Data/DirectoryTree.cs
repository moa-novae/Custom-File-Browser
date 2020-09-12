using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Q1Entity;

namespace Q1
{
    /// <summary>
    /// All directory items within the root folder is represented by a tree data structure. If a file is within a folder, the file
    /// is considered as the child node of the folder. The tree class contains the root node of the tree which is the root directory. 
    /// It also contains other helpful methods and properties
    /// </summary>
    public class DirectoryTree
    {
        #region Constructor

        public DirectoryTree(string startingDir)
        {
            DirectoryItem rootItem = new DirectoryItem(startingDir, DirectoryItemType.Folder);
            // construct root node of the tree 
            RootNode = new DirectoryTreeNode(rootItem.FullPath, rootItem);
            AddChildrenToTree(RootNode);
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Root node of the directory tree. This is the root directory 
        /// </summary>
        public DirectoryTreeNode RootNode { get; private set; }

        /// <summary>
        /// Flattens the tree into a list containing all the tree nodes
        /// </summary>
        public List<DirectoryTreeNode> FlattenedTreeList { get { return Flatten(RootNode).ToList(); } }

        /// <summary>
        /// Flattens the directory tree into a dictionary with the key being the full directory path of the tree node
        /// and the value being the tree node
        /// </summary>
        public Dictionary<string, DirectoryTreeNode> FlattenedTreeDictionary
        {
            get
            {
                var flattenedTreeNodes = Flatten(RootNode);
                // check if enumerable is not emtpy or null
                if (flattenedTreeNodes.Any())
                    return flattenedTreeNodes.ToDictionary(n => n.Item.FullPath, n => n);
                return new Dictionary<string, DirectoryTreeNode>();
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// Enumerates over the tree. Pass in the root node to start.
        /// can be used to generate list and dictionary form of the tree
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<DirectoryTreeNode> Flatten(DirectoryTreeNode node)
        {
            if (node == null)
            {
                yield break;
            }
            yield return node;
            if (node.GetAllChildren().Count() == 0)
                yield break;
            foreach (var child in node.GetAllChildren().Values)
            {
                foreach (var flattenedNode in Flatten(child))
                {
                    yield return flattenedNode;
                }
            }
        }

        /// <summary>
        /// A recursive method to construct the tree from the root node
        /// </summary>
        /// <param name="node"></param>
        private void AddChildrenToTree(DirectoryTreeNode node)
        {

            List<DirectoryItem> childItems = new List<DirectoryItem>();
            List<DirectoryTreeNode> childNodes = new List<DirectoryTreeNode>();
            // check if it is folder
            if (node.Item.Type == DirectoryItemType.Folder)
                // get all directory items within the folder
                childItems = DirectoryStructure.GetDirectoryContents(node.Item.FullPath);
            // If folder is not empty
            if (childItems != null)
                // construct the child nodes
                childNodes = childItems.Select(item => new DirectoryTreeNode(new DirectoryInfo(item.FullPath).FullName, item))
                                       .ToList();
            // Add child node to the current node and check if any child nodes are folders and repeat the process for each child item
                foreach (DirectoryTreeNode childNode in childNodes)
                {
                    node.Add(childNode);
                    AddChildrenToTree(childNode);
                }
        }

        /// <summary>
        /// Go over the entire tree and check if each node fits the search criteria
        /// </summary>
        /// <param name="searchString"></param>
        public void SetAllDirectoryTreeNodeEligibility(string searchString)
        {
            // loop over the tree
            foreach (DirectoryTreeNode node in FlattenedTreeList)
            {
                // check if searchString is not a substring of each directory item's full path in a case insensitive way
                if (node.Item.Name.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) == -1)
                    node.IsCriteriaMatched = false;
                else
                {
                    node.IsCriteriaMatched = true;
                    var ancestors = node.Ancestors;
                    // if a child item satifies search string, all its ancestors must be displayed in the tree view
                    foreach (var ancestor in ancestors)
                    {
                        ancestor.IsCriteriaMatched = true;
                    }
                }
            }
        }

        /// <summary>
        /// When the search field is cleared, reset the tree nodes' eligibility
        /// </summary>
        public void SetAllDirectoryTreeNodeEligibleNull()
        {
            foreach (DirectoryTreeNode node in FlattenedTreeList)
            {
                // null means no search criteria present, thus display the node
                // false means do not display
                // true means display
                node.IsCriteriaMatched = null;
            }
        }

        /// <summary>
        /// Get a tree node based on the full path provided
        /// </summary>
        /// <param name="fullPath">the full path of the tree node</param>
        /// <returns></returns>
        public DirectoryTreeNode GetNode(string fullPath)
        {
            /* Since tree is built as nested dictionaries, with the key being the directory path and value the node,
            * We can path from root to the required node, step by step, going down the directory path
            * Ex: If root node is C:\, and we need c:\programfiles\RequiredNode, 
            * we can access the required node by doing this:
            * programfiles = rootNode.children[c:\programfiles]
            * requiredNode = programfiles[requiredNode]
            * 
            * Doing this should be faster than traversing across tree and comparing all nodes
            **/

            // the bottom of the stack is the node we want to access
            // the top of the stack is the root node
            // By travelling down the stack, we can access the desired node from root node
            Stack<string> directoryPaths = new Stack<string>();

            // To construct the stack,
            // A pointer starts from the desired path and goes up to root by traveling to each node's parent
            // until the root node is reached
            // The full path of each node travelled is push onto the stack
            DirectoryInfo directoryPointer = new DirectoryInfo(fullPath);
            DirectoryInfo rootDirectory = new DirectoryInfo(RootNode.Item.FullPath);
            
            // Loop as long as the pointer is not at the root path
            while (directoryPointer.FullName != rootDirectory.FullName)
            {
                directoryPaths.Push(directoryPointer.FullName);
                directoryPointer = directoryPointer.Parent;
            }

            // Travel down the stack to get from the root node to the desired node
            DirectoryTreeNode treePointer = RootNode;
            while (directoryPaths.Count() > 0)
            {
                treePointer = treePointer.GetChild(directoryPaths.Pop());
            }
            return treePointer;

        }

        #endregion
    }
}
