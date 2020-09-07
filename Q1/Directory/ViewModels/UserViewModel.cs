using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Q1.State;
using Q1.Views;

namespace Q1
{
    /// <summary>
    /// The view model for the application main Directory view
    /// </summary>
    public class UserViewModel : BaseViewModel
    {
        #region public properties
        /// <summary>
        /// A list of all directories on the machine
        /// </summary>
        public ObservableCollection<User> AllUsers { get { return usersStore.CurrentUsers; } }
        public User SelectedUser { get; set; }
        private UsersStore usersStore { get; set; }
        private UserServices userServices { get; set; }
        public RelayCommand DeleteUserCommand { get; set; }
        public RelayCommand SelectedItemChangedCommand { get; set; }
        public RelayCommand OpenViewUserWindowCommand { get; private set; }
        public RelayCommand OpenNewUserWindowCommand { get; private set; }
        public RelayCommand OpenEditUserWindowCommand { get; private set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserViewModel(UserServices uServe, UsersStore uStore)
        {
            usersStore = uStore;
            userServices = uServe;
            SelectedItemChangedCommand = new RelayCommand(args => SelectedItemChanged(args));
            DeleteUserCommand = new RelayCommand(DeleteUser, IsAnUserSelected);
            OpenViewUserWindowCommand = new RelayCommand(OpenViewUserWindow, IsAnUserSelected);
            OpenNewUserWindowCommand = new RelayCommand(CreateNewUserWindow);
            OpenEditUserWindowCommand = new RelayCommand(OpenEditUserWindow, IsAnUserSelected);
        }
        private void SelectedItemChanged(object args)
        {
            SelectedUser = (User)args;
        }
        private bool IsAnUserSelected(object message)
        {
            if (SelectedUser != null)
                return true;
            else
                return false;
        }

        private void OpenViewUserWindow(object message)
        {
            ViewUserView view = new ViewUserView();
            view.DataContext = SelectedUser;
            view.Show();
        }

        private void OpenEditUserWindow(object message)
        {
            UserFormView view = new UserFormView(userServices, SelectedUser);
            view.Show();

        }

        public void CreateNewUserWindow(object message)
        {

            UserFormView view = new UserFormView(userServices);
            view.Show();
        }

        public void DeleteUser(object message)
        {
            int uId = SelectedUser.UserId;
            userServices.Delete(uId);

        }


        #endregion
    }
}