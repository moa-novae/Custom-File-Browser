using Q1.Services;
using Q1.State;

namespace Q1
{
    public class MainViewModel : BaseViewModel
    {
        #region Public properties
        /// <summary>
        /// Sub view model for file browsing
        /// </summary>
        public DirectoryStructureViewModel FileBrowser { get; set; }
        public UserViewModel UserBrowser { get; set; }
        public UserStateServices UserServices { get; set; }


        public RelayCommand CreateEditItemWindowCommand { get; private set; }



        #endregion

        #region Constructor
        public MainViewModel(UserStateServices userServices, UserState userState, DirectoryItemStateServices directoryItemServices, DirectoryItemState directoryItemsState)
        {
            UserServices = userServices;
            FileBrowser = new DirectoryStructureViewModel(directoryItemServices, directoryItemsState, userState);
            UserBrowser = new UserViewModel(userServices, userState);
        }

        #endregion


    }
}
