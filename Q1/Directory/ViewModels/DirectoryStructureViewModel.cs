using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Q1
{ 
    /// <summary>
    /// The view model for the application main Directory view
    /// </summary>
    public class DirectoryStructureViewModel : BaseViewModel
    {
        #region public properties
        /// <summary>
        /// A list of all directories on the machine
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Items { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DirectoryStructureViewModel()
        {
            DirectoryTree tree = new DirectoryTree(@"C:\Program Files\AMD");
            Dictionary<string, DirectoryTreeNode> rootChildren = tree.RootNode.GetAllChildren();
            Items = new ObservableCollection<DirectoryItemViewModel>(
                rootChildren.Values.Select(childNode => new DirectoryItemViewModel(childNode)));
        }
        #endregion
    }
}