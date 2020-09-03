using System.Collections.ObjectModel;
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
            var children = DirectoryStructure.GetLogicalDrives();
            this.Items = new ObservableCollection<DirectoryItemViewModel>(
                children.Select(drive => new DirectoryItemViewModel(drive.FullPath, DirectoryItemType.Drive)));
        }
        #endregion
    }
}