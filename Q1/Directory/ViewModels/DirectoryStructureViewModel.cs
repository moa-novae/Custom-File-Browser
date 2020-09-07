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
        private ObservableCollection<DirectoryItemViewModel> items = new ObservableCollection<DirectoryItemViewModel>();
        #region public properties
        /// <summary>
        /// A list of all directories on the machine
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Items {
            get
            {
                return items;
            }
            set
            {
                items.Clear();
                foreach (var child in value)
                {
                    items.Add(child);
                }
            }
        }

        public DirectoryItem SelectedDirectoryItem { get; set; }
        private DirectoryTree directoryTree { get; set; }
        public string SearchString { get; set; } = "";
       
        public RelayCommand SelectedItemChangedCommand { get; private set; }
        public RelayCommand OpenEditDirectoryItemWindowCommand { get; private set; }
        public RelayCommand ShowAllTreeNodesCommand { get; private set; }
        public RelayCommand SearchAllTreeNodesCommand { get; private set; }
        private void SelectedItemChanged(object args)
        {
            var selectedDirectoryItemViewModel = (DirectoryItemViewModel)args;
            if (selectedDirectoryItemViewModel != null)
                SelectedDirectoryItem = (DirectoryItem)selectedDirectoryItemViewModel.Item;
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DirectoryStructureViewModel()
        {
            DirectoryTree tree = new DirectoryTree(@"C:\WpfTest");
            directoryTree = tree;
            Dictionary<string, DirectoryTreeNode> rootChildren = tree.RootNode.GetAllChildren();
            Items = new ObservableCollection<DirectoryItemViewModel>(
                rootChildren.Values.Select(childNode => new DirectoryItemViewModel(childNode)));

            // set RelayCommands
            SelectedItemChangedCommand = new RelayCommand(args => SelectedItemChanged(args));
            OpenEditDirectoryItemWindowCommand = new RelayCommand(OpenEditDirectoryItemWindow, CanOpenEditDirectoryItemWindow);
            ShowAllTreeNodesCommand = new RelayCommand(ClearSearchCriteria, IsSearchStringNotEmpty);
            SearchAllTreeNodesCommand = new RelayCommand(FilterForEligibleTreeNodes, IsSearchStringNotEmpty);
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

        public void FilterForEligibleTreeNodes(object message)
        {
            directoryTree.SetAllDirectoryTreeNodeEligibility(SearchString);
            var ItemsThatMatchCriteria = new ObservableCollection<DirectoryItemViewModel>();
            // only show first level item that meets criteria
            foreach (var item in Items)
            {
                if (item.Node.IsCriteriaMatched != false)
                    ItemsThatMatchCriteria.Add(item);
            }
            void ExpandFoldersContainingSearchedItems (DirectoryItemViewModel viewModel)
            {
                if (viewModel.Node.IsCriteriaMatched == true)
                    viewModel.IsExpanded = true;
                foreach(var childViewModel in viewModel.Children)
                {
                    ExpandFoldersContainingSearchedItems(childViewModel);
                }
            }
            foreach (var item in ItemsThatMatchCriteria)
            {
                ExpandFoldersContainingSearchedItems(item);
            }    

            Items = new ObservableCollection<DirectoryItemViewModel> (ItemsThatMatchCriteria);
        }
        
        
        public void ClearSearchCriteria(object message)
        {
            directoryTree.SetAllDirectoryTreeNodeEligibleNull();
            SearchString = "";
            Dictionary<string, DirectoryTreeNode> rootChildren = directoryTree.RootNode.GetAllChildren();
            Items = new ObservableCollection<DirectoryItemViewModel>(
                rootChildren.Values.Select(childNode => new DirectoryItemViewModel(childNode) { IsExpanded = false })); 
        }

        public bool IsSearchStringNotEmpty(object message)
        {
            return SearchString != "";
        }

        
    }
}