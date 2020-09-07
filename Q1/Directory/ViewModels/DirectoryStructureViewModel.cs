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

        public DirectoryItem SelectedDirectoryItem { get; set; }

        public RelayCommand SelectedItemChangedCommand { get; set; }
        public RelayCommand OpenEditDirectoryItemWindowCommand { get; private set; }

        private void SelectedItemChanged(object args)
        {
            var selectedDirectoryItemViewModel = (DirectoryItemViewModel)args;
            SelectedDirectoryItem = (DirectoryItem)selectedDirectoryItemViewModel.Item;
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DirectoryStructureViewModel()
        {
            // set RelayCommands
            SelectedItemChangedCommand = new RelayCommand(args => SelectedItemChanged(args));
            OpenEditDirectoryItemWindowCommand = new RelayCommand(OpenEditDirectoryItemWindow, CanOpenEditDirectoryItemWindow);

            DirectoryTree tree = new DirectoryTree(@"C:\Program Files\AMD");
            Dictionary<string, DirectoryTreeNode> rootChildren = tree.RootNode.GetAllChildren();
            Items = new ObservableCollection<DirectoryItemViewModel>(
                rootChildren.Values.Select(childNode => new DirectoryItemViewModel(childNode)));
        }
        #endregion
        /// <summary>
        /// Open new windows allowing user to view and edit directory item properties
        /// </summary>
        /// <param name="message"></param>
        public void OpenEditDirectoryItemWindow(object message)
        {
            EditDirectoryItemView view = new EditDirectoryItemView();
            view.Show();
        }

        public bool CanOpenEditDirectoryItemWindow(object message)
        {
            return SelectedDirectoryItem != null ? true : false;
        }
    }
}