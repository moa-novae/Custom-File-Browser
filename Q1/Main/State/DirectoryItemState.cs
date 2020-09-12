using System.Collections.Generic;

namespace Q1.State
{
    // state is responsible for keeping track of what are the directory items and what are its properties
    // For information on who owns the directories, use UserState
    public class DirectoryItemState
    {
        public DirectoryTree Tree { get; set; }

        public Dictionary<string, DirectoryTreeNode> RootChildren { get { return Tree.RootNode?.GetAllChildren(); } }


        public DirectoryItemState(string rootpath)
        {
            Tree = new DirectoryTree(rootpath);
        }
    }
}
