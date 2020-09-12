using Q1.Services;
using Q1.State;

namespace Q1
{
    // I definitely need to do more work on dependency injection and write more testable code
    // For the purpose of this project, I pass some of the dependencies through constructor while acknowldging that more work needs to be done

    // Main view models shows the user browser and the file browser
    public class MainViewModel : BaseViewModel
    {
        #region Constructor
        public MainViewModel(UserStateServices userServices, UserState userState, DirectoryItemStateServices directoryItemServices, DirectoryItemState directoryItemsState)
        {
            FileBrowser = new DirectoryStructureViewModel(directoryItemServices, directoryItemsState, userState);
            // UserBrowser needs directoryItemService to update the list of directory items a user owns
            UserBrowser = new UserViewModel(userServices, userState, directoryItemServices);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Sub view model for file browsing
        /// </summary>
        public DirectoryStructureViewModel FileBrowser { get; set; }
        /// <summary>
        /// Sub view model for showing users and their possession
        /// </summary>
        public UserViewModel UserBrowser { get; set; }

        #endregion


    }
}
