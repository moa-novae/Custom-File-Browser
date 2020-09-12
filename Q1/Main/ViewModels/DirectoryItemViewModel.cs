using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Q1Entity;
namespace Q1
{
    /// <summary>
    /// A view model for each directory item
    /// </summary>
    public class DirectoryItemViewModel : BaseViewModel
    {
        #region Constructor
    
        public DirectoryItemViewModel(DirectoryTreeNode node)
        {
            // Set the directoryItem this view model represents
            Item = node.Item;

            // Set the DirectoryTreeNode this view model represents for easier access 
            Node = node;

            // only setup children when set IsExpanded to true
            if (IsExpanded == true)
            {
                Expand();
            }
            else
            {
                ClearChildren();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// DirectoryTreeNode which contains information such as node children 
        /// </summary>
        public DirectoryTreeNode Node { get; set; }

        /// <summary>
        /// DirectoryItem which contains inforamtion such as path of directory item
        /// </summary>
        public DirectoryItem Item { get; set; }

        /// <summary>
        /// name of directory item
        /// </summary>
        public string Name { get { return Item.Type == DirectoryItemType.Drive ? Item.FullPath : DirectoryStructure.GetFileFolderName(Item.FullPath); } }


        /// <summary>
        /// A list of all direct children contained inside this directory item. Each child has its own view model
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Children
        {
            get
            {
                return children;
            }
            set
            {
                if (children == null)
                    children = new ObservableCollection<DirectoryItemViewModel>();
                children.Clear();
                foreach (var child in value)
                {
                    children.Add(child);
                }
            }
        }

        /// <summary>
        /// Indicates to treeview in xaml if item can be expanded
        /// </summary>
        public bool CanExpand { get { return Item.Type != DirectoryItemType.File; } }

        /// <summary>
        /// Check if current tree view node is expanded
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return Children?.Count(f => f != null) > 0;
            }
            set
            {
                // If the UI tells us to expand
                if (value == true)
                    //find all children
                    Expand();
                // if the UI tells us to close
                else
                    ClearChildren();

            }
        }

        private ObservableCollection<DirectoryItemViewModel> children;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Add the appropriate children of this directory item for the tree view
        /// </summary>
        public void Expand()
        {
            if (Item.Type == DirectoryItemType.File)
                return;
            //When expand, find all children

            Dictionary<string, DirectoryTreeNode> childrenNodes = Node.GetAllChildren();
            Children = new ObservableCollection<DirectoryItemViewModel>(
                childrenNodes.Values.Where(child => child.IsCriteriaMatched != false).Select(childNode => new DirectoryItemViewModel(childNode)));
        }

        /// <summary>
        /// Removes all children from the list, adding a dummy item to show the expand icon if required
        /// </summary>
        private void ClearChildren()
        {
            // Clear items
            Children = new ObservableCollection<DirectoryItemViewModel>();

            // Show the expand arrow if we are not a file
            if (Item.Type != DirectoryItemType.File)
            {
                Children.Add(null);
            }

        }

        #endregion

    }
}
