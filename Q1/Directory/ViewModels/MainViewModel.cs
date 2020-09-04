using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public RelayCommand CreateNewUserWindowCommand { get; private set; }
        public RelayCommand CreateEditUserWindowCommand { get; private set; }

        #endregion

        #region Constructor
        public MainViewModel ()
        {
            this.FileBrowser = new DirectoryStructureViewModel();
            this.UserBrowser = new UserViewModel();
            CreateNewUserWindowCommand = new RelayCommand(CreateNewUserWindow, CanCreateNewUserWindow);
            
        }

        #endregion
        


      


        public void CreateNewUserWindow(object message)
        {
            AddUserView view = new AddUserView();
            view.Show();
        }

        public void CreateEditUserWindow(object message)
        {
            
        }

        public bool CanCreateNewUserWindow(object message)
        {
            return true;
        }

        
    }
}
