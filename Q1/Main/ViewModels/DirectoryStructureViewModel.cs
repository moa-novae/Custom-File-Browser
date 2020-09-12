using System.Collections.ObjectModel;
using System.Linq;
using Q1Entity;
using Q1.Services;
using Q1.State;

namespace Q1
{
    /// <summary>
    /// This view model serve as a container for all the root items in the root directory. 
    /// Each root item is its own view model (DirectoryitemViewModel)
    /// </summary>
    public class DirectoryStructureViewModel : BaseViewModel
    {
        #region Constructor

        public DirectoryStructureViewModel(DirectoryItemStateServices dirServ, DirectoryItemState dirState, UserState us)
        {
            // directoryTreeState contains an instance of current directory in tree structure
            directoryTreeState = dirState;
            userState = us;
            // Responsible for interacting with db and setting state
            directoryItemServices = dirServ;

            // Generate sub view models (DirectoryItemViewModel) for each of the root items in the root folder. Each root item in the root folder is the root of its own tree
            ChildViewModels = new ObservableCollection<DirectoryItemViewModel>();
            if (directoryTreeState.RootChildren != null)
            {
                ChildViewModels = new ObservableCollection<DirectoryItemViewModel>(
                    directoryTreeState.RootChildren?.Values.Select(childNode => new DirectoryItemViewModel(childNode)));
            }

            // set RelayCommands
            SelectedItemChangedCommand = new RelayCommand(args => SelectedItemChanged(args));
            OpenEditDirectoryItemWindowCommand = new RelayCommand(OpenEditDirectoryItemWindow, CanOpenEditDirectoryItemWindow);
            ShowAllTreeNodesCommand = new RelayCommand(ClearSearchCriteria, IsSearchStringNotEmpty);
            SearchAllTreeNodesCommand = new RelayCommand(FilterForEligibleTreeNodes, IsSearchStringNotEmpty);
        }
        #endregion

        #region Properties
        /// <summary>
        /// A list of all directories on the machine
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> ChildViewModels
        {
            get
            {
                return childViewModels;
            }
            set
            {
                childViewModels.Clear();
                foreach (var child in value)
                {
                    childViewModels.Add(child);
                }
            }
        }

        // Property for keeping track the search string in the search bar
        public string SearchString { get; set; } = "";
        public DirectoryItem SelectedDirectoryItem { get; set; }
        private UserState userState { get; set; }
        private DirectoryItemState directoryTreeState { get; set; }
        private DirectoryItemStateServices directoryItemServices { get; set; }
        private ObservableCollection<DirectoryItemViewModel> childViewModels = new ObservableCollection<DirectoryItemViewModel>();

        #endregion

        #region Public commands

        public RelayCommand SelectedItemChangedCommand { get; private set; }
        public RelayCommand OpenEditDirectoryItemWindowCommand { get; private set; }
        public RelayCommand ShowAllTreeNodesCommand { get; private set; }
        public RelayCommand SearchAllTreeNodesCommand { get; private set; }

        #endregion

        #region Private methods for commands

        /// <summary>
        /// Open new windows allowing user to view and edit directory item properties
        /// </summary>
        /// <param name="message"></param>
        private void OpenEditDirectoryItemWindow(object message)
        {
            EditDirectoryItemView view = new EditDirectoryItemView(directoryItemServices, directoryTreeState, SelectedDirectoryItem, userState);
            view.Show();
        }

        private bool CanOpenEditDirectoryItemWindow(object message)
        {
            return SelectedDirectoryItem != null ? true : false;
        }
        private void SelectedItemChanged(object args)
        {
            var selectedDirectoryItemViewModel = (DirectoryItemViewModel)args;
            if (selectedDirectoryItemViewModel != null)
                SelectedDirectoryItem = selectedDirectoryItemViewModel.Item;
        }

        #endregion

        #region Methods and properties for searching directory items

        private void FilterForEligibleTreeNodes(object message)
        {
            directoryTreeState.Tree.SetAllDirectoryTreeNodeEligibility(SearchString);
            var ItemsThatMatchCriteria = new ObservableCollection<DirectoryItemViewModel>();
            // only show first level item that meets criteria
            foreach (var item in ChildViewModels)
            {
                if (item.Node.IsCriteriaMatched != false)
                    ItemsThatMatchCriteria.Add(item);
            }

            // Local recursive method that ensures all folder containing the searched item expands
            void ExpandFoldersContainingSearchedItems(DirectoryItemViewModel viewModel)
            {
                if (viewModel.Node.IsCriteriaMatched == true)
                {
                    // Find current tree node's children that passed the filtering
                    var eligibleChildTreeNodes = viewModel.Node.GetAllChildren().Values.Where(child => child.IsCriteriaMatched != false);
                    // Generate new children view models from these nodes
                    var childViewModels = new ObservableCollection<DirectoryItemViewModel>(eligibleChildTreeNodes.Select(childNode => new DirectoryItemViewModel(childNode)));
                    // clear old child view models, and add new children that are eligible to each parent view model
                    viewModel.Children = childViewModels;
                    viewModel.IsExpanded = true;
                }
                foreach (var childViewModel in viewModel.Children)
                {
                    ExpandFoldersContainingSearchedItems(childViewModel);
                }
            }

            foreach (var item in ItemsThatMatchCriteria)
            {
                ExpandFoldersContainingSearchedItems(item);
            }

            // Set the child view models with the filterd items
            ChildViewModels = new ObservableCollection<DirectoryItemViewModel>(ItemsThatMatchCriteria);
        }

        // method for clearing out the search bar and resetting the directory tree view
        private void ClearSearchCriteria(object message)
        {
            directoryTreeState.Tree.SetAllDirectoryTreeNodeEligibleNull();
            SearchString = "";
            ChildViewModels = new ObservableCollection<DirectoryItemViewModel>(
                directoryTreeState.RootChildren.Values.Select(childNode => new DirectoryItemViewModel(childNode) { IsExpanded = false }));
        }

        private bool IsSearchStringNotEmpty(object message)
        {
            return SearchString != "";
        }

        #endregion

    }
}