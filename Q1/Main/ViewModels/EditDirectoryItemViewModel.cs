using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using Q1.Services;
using Q1.State;
using Q1Entity;

namespace Q1
{
    /// <summary>
    /// View Model for editing details of a directory item, including owners and notes
    /// </summary>
    public class EditDirectoryItemViewModel : BaseViewModel
    {
        #region Constructor
        public EditDirectoryItemViewModel(DirectoryItemStateServices dirServ, DirectoryItem selectedDirectoryItem, UserState userState)
        {
            SelectedDirectoryItem = selectedDirectoryItem;
            currentUsers = userState.CurrentUsers;
            directoryItemServices = dirServ;
            // Setup commands
            AddOwnerCommand = new RelayCommand(AddOwner, NonOwnerSelected);
            RemoveOwnerCommand = new RelayCommand(RemoveOwner, OwnerSelected);
            SaveDirectoryItemInfoCommand = new RelayCommand(SaveDirectoryInfo);
            CloseWindowCommand = new RelayCommand(CloseWindow);

            // local function for checking if join tables contain the selectedDirectoryItem 
            bool CheckIfJoinContainsDirectoryItem(List<UserDirectoryItem> userDirectoryItems)
            {
                if (userDirectoryItems == null)
                    return false;
                return userDirectoryItems.Any(udi => udi.DirectoryItemId == selectedDirectoryItem.DirectoryItemId);
            }

            // If the DirectoryItem has no owners
            if (currentUsers.Any(u => CheckIfJoinContainsDirectoryItem(u.UserDirectoryItems)))
            {              
                // loop over all jointables and find users who own the directory item
                Owners = new ObservableCollection<User>(currentUsers.Where(u => CheckIfJoinContainsDirectoryItem(u.UserDirectoryItems)));
                List<int> ownersIds = Owners.Select(o => o.UserId).ToList();
                // Users in the users list who is not an owner are non-owners
                // Can't use Except method to filter because Users in Owners have different references than Users in currentUsers
                // select can produce new instances
                NonOwners = new ObservableCollection<User>(currentUsers.Where(u => !ownersIds.Contains(u.UserId)));

            }
            // If the directory has owners
            else
            {
                //Set up empty owners and nonOwners is equal to current user list
                Owners = new ObservableCollection<User>();
                nonOwners = new ObservableCollection<User>(currentUsers);
            }
        }

        #endregion

        #region Properties

        public DirectoryItem SelectedDirectoryItem { get; set; }

        // The setter of all observable collection is done like this to ensure that the UI will update 
        //whenever a new value is set to each observable collection
        public ObservableCollection<User> NonOwners
        {
            get
            {
                return nonOwners;
            }
            set
            {
                nonOwners.Clear();
                foreach (var user in value)
                    nonOwners.Add(user);
            }
        }
        public ObservableCollection<User> Owners
        {
            get
            {
                return owners;
            }
            set
            {
                owners?.Clear();
                foreach (var user in value)
                    owners.Add(user);
            }
        }
        public User SelectedOwner { get; set; }
        public User SelectedNonOwner { get; set; }
        public RelayCommand AddOwnerCommand { get; private set; }
        public RelayCommand RemoveOwnerCommand { get; private set; }
        public RelayCommand SaveDirectoryItemInfoCommand { get; set; }
        public RelayCommand CloseWindowCommand { get; set; }
        private ObservableCollection<User> currentUsers { get; set; } = new ObservableCollection<User>();
        private ObservableCollection<User> nonOwners { get; set; } = new ObservableCollection<User>();
        private ObservableCollection<User> owners { get; set; } = new ObservableCollection<User>();
        private DirectoryItemStateServices directoryItemServices { get; set; }

        #endregion

        #region methods

        private void AddOwner(object message)
        {
            Owners.Add(SelectedNonOwner);
            NonOwners.Remove(SelectedNonOwner);
        }
        private bool OwnerSelected(object message)
        {
            return SelectedOwner != null;
        }
        private void RemoveOwner(object message)
        {
            NonOwners.Add(SelectedOwner);
            Owners.Remove(SelectedOwner);
        }

        private bool NonOwnerSelected(object message)
        {
            return SelectedNonOwner != null;
        }
        private void SaveDirectoryInfo(object message)
        {
            directoryItemServices.Update(SelectedDirectoryItem);
            directoryItemServices.UpdateUserDirectoryItems(owners.ToList(), SelectedDirectoryItem);
            CloseWindow(message);
        }
        // Closes this view model's window
        private void CloseWindow(object message)
        {
            ((Window)message).Close();
        }

        #endregion
    }
}
