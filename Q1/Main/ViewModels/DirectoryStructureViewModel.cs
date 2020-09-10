using System.Collections.ObjectModel;
using System.Linq;
using Q1Entity;
using Q1.Services;
using Q1.State;

namespace Q1
{
    /// <summary>
    /// The view model for the application main Directory view
    /// </summary>
    public class DirectoryStructureViewModel : BaseViewModel
    {
        private ObservableCollection<DirectoryItemViewModel> childViewModels = new ObservableCollection<DirectoryItemViewModel>();
        #region public properties
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

        #endregion

        public DirectoryItem SelectedDirectoryItem { get; set; }
        private DirectoryItemState directoryTreeState { get; set; }
        private UserState userState { get; set; }
        private DirectoryItemStateServices directoryItemServices { get; set; }
        public string SearchString { get; set; } = "";

        #region Public commands

        public RelayCommand SelectedItemChangedCommand { get; private set; }
        public RelayCommand OpenEditDirectoryItemWindowCommand { get; private set; }
        public RelayCommand ShowAllTreeNodesCommand { get; private set; }
        public RelayCommand SearchAllTreeNodesCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DirectoryStructureViewModel(DirectoryItemStateServices dirServ, DirectoryItemState dirState, UserState us)
        {
            // directoryTreeState contains an instance of current directory tree
            directoryTreeState = dirState;
            directoryItemServices = dirServ;
            userState = us;

            // Generate sub view models for each of the root items in the root folder. Each root item in the root folder is the root of its own tree
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

        #region Methods for commands

        /// <summary>
        /// Open new windows allowing user to view and edit directory item properties
        /// </summary>
        /// <param name="message"></param>
        public void OpenEditDirectoryItemWindow(object message)
        {
            EditDirectoryItemView view = new EditDirectoryItemView(directoryItemServices, directoryTreeState, SelectedDirectoryItem, userState);
            view.Show();
        }

        public bool CanOpenEditDirectoryItemWindow(object message)
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

        #region Methods for searching directory items

        public void FilterForEligibleTreeNodes(object message)
        {
            directoryTreeState.Tree.SetAllDirectoryTreeNodeEligibility(SearchString);
            var ItemsThatMatchCriteria = new ObservableCollection<DirectoryItemViewModel>();
            // only show first level item that meets criteria
            foreach (var item in ChildViewModels)
            {
                if (item.Node.IsCriteriaMatched != false)
                    ItemsThatMatchCriteria.Add(item);
            }
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

            ChildViewModels = new ObservableCollection<DirectoryItemViewModel>(ItemsThatMatchCriteria);
        }


        public void ClearSearchCriteria(object message)
        {
            directoryTreeState.Tree.SetAllDirectoryTreeNodeEligibleNull();
            SearchString = "";
            ChildViewModels = new ObservableCollection<DirectoryItemViewModel>(
                directoryTreeState.RootChildren.Values.Select(childNode => new DirectoryItemViewModel(childNode) { IsExpanded = false }));
        }

        public bool IsSearchStringNotEmpty(object message)
        {
            return SearchString != "";
        }

        #endregion

    }
}