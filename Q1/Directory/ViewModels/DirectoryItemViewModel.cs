using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;

namespace Q1
{
    /// <summary>
    /// A view model for each directory item
    /// </summary>
    public class DirectoryItemViewModel : BaseViewModel
    {
        #region public properties

        public DirectoryTreeNode Node { get; set; }
        public DirectoryItem Item { get; set; }
        
        /// <summary>
        /// name of directory item
        /// </summary>
        public string Name { get { return Item.Type == DirectoryItemType.Drive ? Item.FullPath : DirectoryStructure.GetFileFolderName(Item.FullPath); } }

        

        /// <summary>
        /// A list of all children contained inside this item
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Children { get; set; }

        // Files cannot be expanded
        /// <summary>
        /// indicates if item can be expanded
        /// </summary>
        public bool CanExpand { get { return Item.Type != DirectoryItemType.File; } }

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

        #endregion

      
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fullpath">The full path of this item</param>
        /// <param name="type">The type of item</param>
        public DirectoryItemViewModel(DirectoryTreeNode node)
        {
            // Set the directoryItem this view model represents
            Item = node.Item;

            // Set Node for easier access 
            Node = node;
            // setup the children as needed
            ClearChildren();

        }

        #endregion

        #region Helper Function

        /// <summary>
        /// Removes all children from the list, adding a dummy item to show the expand icon if required
        /// </summary>
        private void ClearChildren()
        {
            // Clear items
            Children = new ObservableCollection<DirectoryItemViewModel>();

            // Show the expand arrow if we are not a file
            if (Item.Type != DirectoryItemType.File)
                Children.Add(null);
        }

        #endregion

        /// <summary>
        /// Expand this directory and find all children
        /// </summary>
        public void Expand()
        {
            if (Item.Type == DirectoryItemType.File)
                return;
            //When expand, find all children
            
            Dictionary<string, DirectoryTreeNode> childrenNodes = Node.GetAllChildren();
            Children = new ObservableCollection<DirectoryItemViewModel>(
                childrenNodes.Values.Select(childNode => new DirectoryItemViewModel(childNode)));
        }
    }
}
