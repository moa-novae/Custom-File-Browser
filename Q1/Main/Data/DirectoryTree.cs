using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Q1Entity;

namespace Q1
{
    public class DirectoryTree
    {
        public DirectoryTreeNode RootNode { get; private set; }
        public List<DirectoryTreeNode> FlattenedTreeList { get { return Flatten(RootNode).ToList(); } }
        public Dictionary<string, DirectoryTreeNode> FlattenedTreeDictionary
        {
            get
            {
                var flattenedTreeNodes = Flatten(RootNode);
                // check if enumerable is not emtpy or null
                if (flattenedTreeNodes.Any())
                    return flattenedTreeNodes.ToDictionary(n => n.Item.FullPath, n => n);
                return null;
            }
        }
        #region Constructor

        public DirectoryTree(string startingDir)
        {
            DirectoryItem rootItem;

            rootItem = new DirectoryItem(startingDir, DirectoryItemType.Folder);
            RootNode = new DirectoryTreeNode(rootItem.FullPath, rootItem);
            AddChildrenToTree(RootNode);

        }

        #endregion

        #region Helper

        // pass in root node to flatten tree
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

        private static void AddChildrenToTree(DirectoryTreeNode node)
        {

            List<DirectoryItem> childItems = null;
            List<DirectoryTreeNode> childNodes = null;
            if (node.Item.Type == DirectoryItemType.Folder)
                childItems = DirectoryStructure.GetDirectoryContents(node.Item.FullPath);
            if (childItems != null)
                childNodes = childItems.Select(item => new DirectoryTreeNode(new DirectoryInfo(item.FullPath).FullName, item))
                                       .ToList();
            if (childNodes != null)
                foreach (DirectoryTreeNode childNode in childNodes)
                {
                    node.Add(childNode);
                    AddChildrenToTree(childNode);
                }
        }



        public void SetAllDirectoryTreeNodeEligibility(string searchString)
        {
            // loop over the tree
            foreach (DirectoryTreeNode node in FlattenedTreeList)
            {
                // check if searchString is substring of full path in case insensitive way
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

        public void SetAllDirectoryTreeNodeEligibleNull()
        {
            foreach (DirectoryTreeNode node in FlattenedTreeList)
            {
                node.IsCriteriaMatched = null;
            }
        }

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
            Stack<string> directoryPaths = new Stack<string>();

            // A pointer starts from the desired path and moves up
            DirectoryInfo directoryPointer = new DirectoryInfo(fullPath);
            DirectoryInfo rootDirectory = new DirectoryInfo(RootNode.Item.FullPath);
            while (directoryPointer.FullName != rootDirectory.FullName)
            {
                directoryPaths.Push(directoryPointer.FullName);
                directoryPointer = directoryPointer.Parent;
            }

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
