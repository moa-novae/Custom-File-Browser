using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Q1.Services;
using Q1.State;
using Q1.Views;
using Q1Entity;
namespace Q1
{
    /// <summary>
    /// The view model for the application main Directory view
    /// </summary>
    public class UserViewModel : BaseViewModel
    {
        #region Constructor

        public UserViewModel(UserStateServices uServe, UserState uStore, DirectoryItemStateServices dServe)
        {
            userState = uStore;
            userServices = uServe;
            directoryItemServices = dServe;
            SelectedItemChangedCommand = new RelayCommand(args => SelectedItemChanged(args));
            DeleteUserCommand = new RelayCommand(DeleteUser, IsAnUserSelected);
            DeleteUserDirectoryItemCommand = new RelayCommand(DeleteUserDirectoryItem, IsUserDirectoryItemselected);
            OpenViewUserWindowCommand = new RelayCommand(OpenViewUserWindow, IsAnUserSelected);
            OpenNewUserWindowCommand = new RelayCommand(CreateNewUserWindow);
            OpenEditUserWindowCommand = new RelayCommand(OpenEditUserWindow, IsAnUserSelected);
        }

        #endregion

        #region properties

        public ObservableCollection<User> AllUsers { get { return userState.CurrentUsers; } }
        public User SelectedUser { get; set; }
        public UserDirectoryItem SelectedUserDirectoryItem { get; set; }
        public RelayCommand DeleteUserCommand { get; set; }
        public RelayCommand DeleteUserDirectoryItemCommand { get; set; }
        public RelayCommand SelectedItemChangedCommand { get; set; }
        public RelayCommand OpenViewUserWindowCommand { get; private set; }
        public RelayCommand OpenNewUserWindowCommand { get; private set; }
        public RelayCommand OpenEditUserWindowCommand { get; private set; }
        private UserState userState { get; set; }
        private UserStateServices userServices { get; set; }
        private DirectoryItemStateServices directoryItemServices { get; set; }
        #endregion

        #region Private Methods
        /// <summary>
        /// Triggered by command whenever selected item in user broser changes
        /// </summary>
        /// <param name="args"></param>
        private void SelectedItemChanged(object args)
        {
            // If selected is user and not directory item in user browser
            if (args is User user)
            {
                // the user and the directoryitem cannot be both selected in user view
                SelectedUser = user;
                SelectedUserDirectoryItem = null;
            }
            // When user clicks on directory items owned by a user in user browser
            // args is UserDirectoryItem instead of DirectoryItem because 
            // userDirectoryItem was used as the itemsource which allows display of directory item path
            else if (args is UserDirectoryItem selectedUserDirectoryItem)
            {
                SelectedUserDirectoryItem = selectedUserDirectoryItem;
                SelectedUser = null;
            }
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
        private void CreateNewUserWindow(object message)
        {
            UserFormView view = new UserFormView(userServices);
            view.Show();
        }

        private void DeleteUser(object message)
        {
            userServices.Delete(SelectedUser);
        }
        /// <summary>
        /// Triggered by command to remove a directory from a user's possession in the user browser
        /// </summary>
        /// <param name="message"></param>
        private void DeleteUserDirectoryItem(object message)
        {
            DirectoryItem selectedDirectoryItem = SelectedUserDirectoryItem.DirectoryItem;
            User ownerToBeRemoved = SelectedUserDirectoryItem.User;
            bool CheckIfJoinContainsDirectoryItem(List<UserDirectoryItem> userDirectoryItems)
            {
                return userDirectoryItems.Any(udi => udi.DirectoryItemId == selectedDirectoryItem.DirectoryItemId);
            }

            // Find all the previous owners of the currently selected directory Item
            List<User> oldOwnersOfSelectedDirectoryItem = userState.CurrentUsers.Where(u => CheckIfJoinContainsDirectoryItem(u.UserDirectoryItems)).ToList();
            // remove the owner that no longer owns the selected directory item from the oldOwnersOfSelectedDirectoryItem
            List<User> newOwnersOfSelectedDirectoryItem = oldOwnersOfSelectedDirectoryItem.Where(u => u.UserId != ownerToBeRemoved.UserId).ToList();
            directoryItemServices.UpdateUserDirectoryItems(newOwnersOfSelectedDirectoryItem, selectedDirectoryItem);
        }
        private bool IsUserDirectoryItemselected(object message)
        {
            return SelectedUserDirectoryItem != null;
        }
        private bool IsAnUserSelected(object message)
        {
            return SelectedUser != null;
        }
        #endregion


        
    }
}