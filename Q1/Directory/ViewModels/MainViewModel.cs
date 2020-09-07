using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
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
        public UserServices UserServices { get; set; }
        
       
        public RelayCommand CreateEditItemWindowCommand { get; private set; }
        

       
        #endregion

        #region Constructor
        public MainViewModel (UserServices userServices, UsersStore userStore)
        {
            UserServices = userServices;
            FileBrowser = new DirectoryStructureViewModel();
            UserBrowser = new UserViewModel(userServices, userStore);
           
            
        }

        #endregion

        
    }
}
