using System.Collections.Generic;

namespace Q1.State
{
    public class DirectoryItemState
    {
        public DirectoryTree Tree { get; set; }

        public Dictionary<string, DirectoryTreeNode> RootChildren { get { return Tree.RootNode.GetAllChildren(); } }


        public DirectoryItemState(string rootpath)
        {
            Tree = new DirectoryTree(rootpath);
        }
    }
}
