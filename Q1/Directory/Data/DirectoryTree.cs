using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q1
{
    class DirectoryTree
    {
        public DirectoryTreeNode RootNode { get; private set; }

        #region Constructor

        DirectoryTree(string startingDir)
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

        #endregion
    }
}
