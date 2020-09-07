using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Q1
{
    public class DirectoryTree
    {
        public DirectoryTreeNode RootNode { get; private set; }
        public List<DirectoryTreeNode> FlattenedTreeList { get { return Flatten(RootNode).ToList(); } }
        #region Constructor

        public DirectoryTree(string startingDir)
        {
            DirectoryItem rootItem = new DirectoryItem
            {
                FullPath = startingDir,
                Type = DirectoryItemType.Folder
            };
            RootNode = new DirectoryTreeNode("root", rootItem);
            AddChildrenToTree(RootNode);
        }

        #endregion

        #region Helper

        // pass in root node to flatten tree
        IEnumerable<DirectoryTreeNode> Flatten(DirectoryTreeNode node)
        {
            yield return node;
            if (node.GetAllChildren().Count() == 0)
                yield break; 
            foreach(var child in node.GetAllChildren().Values)
            {
                foreach (var flattenedNode in Flatten(child))
                {
                    yield return flattenedNode;
                }
            }
        }
   
        private static void AddChildrenToTree (DirectoryTreeNode node)
        {

            List<DirectoryItem> childItems = null;
            List<DirectoryTreeNode> childNodes = null;
            if (node.Item.Type == DirectoryItemType.Folder)
                childItems = DirectoryStructure.GetDirectoryContents(node.Item.FullPath);
            if (childItems != null)
                childNodes = childItems.Select(item => new DirectoryTreeNode(item.FullPath, item))
                                       .ToList();
            if (childNodes != null)
                foreach (DirectoryTreeNode childNode in childNodes)
                {
                    node.Add(childNode);
                    AddChildrenToTree(childNode);
                }
        }

        

        public void SetAllDirectoryTreeNodeEligibility (string searchString)
        {
            // loop over the tree
            foreach(DirectoryTreeNode node in FlattenedTreeList)
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

        public void SetAllDirectoryTreeNodeEligibleNull ()
        {
            foreach(DirectoryTreeNode node in FlattenedTreeList)
            {
                node.IsCriteriaMatched = null;
            }
        }

        #endregion
    }
}
