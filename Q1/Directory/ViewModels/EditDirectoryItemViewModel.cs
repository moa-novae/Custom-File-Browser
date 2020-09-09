using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Q1.State;

namespace Q1
{
    public class EditDirectoryItemViewModel : BaseViewModel
    {
        public EditDirectoryItemViewModel(DirectoryItemServices dirServ, DirectoryItemState dirState, DirectoryItem selectedDirectoryItem, UserState userState)
        {
            SelectedDirectoryItem = selectedDirectoryItem;
            currentUsers = userState.CurrentUsers;
            directoryItemServices = dirServ;
            directoryItemState = dirState;
            // Setup commands
            AddOwnerCommand = new RelayCommand(AddOwner, NonOwnerSelected);
            RemoveOwnerCommand = new RelayCommand(RemoveOwner, OwnerSelected);
            SaveDirectoryItemInfoCommand = new RelayCommand(SaveDirectoryInfo);
            CloseWindowCommand = new RelayCommand(CloseWindow);

            // If the DirectoryItem has no owners
            if (SelectedDirectoryItem.UserDirectoryItems == null)
            {
                //Set up empty owners and nonOwners is equal to current user list
                Owners = new ObservableCollection<User>();
                nonOwners = new ObservableCollection<User>(currentUsers);
            }
            else
            {
                // extrapolate owners of a directory item from jointable between users and directory items
                // UserDirectoryItems is a join table and all users in the join table are owners of the attached directory item
                string selectedDirectoryItemPath = selectedDirectoryItem.FullPath;
                Owners = new ObservableCollection<User>(directoryItemState.Tree.GetNode(selectedDirectoryItemPath).Item.UserDirectoryItems?.Select(joinTable => joinTable.User));
                List<int> ownersIds = Owners.Select(o => o.UserId).ToList();
                // Users in the users list who is not an owner are non-owners
                // Can't use Except method to filter because Users in Owners have different references than Users in currentUsers
                // select can produce new instances
                NonOwners = new ObservableCollection<User>(currentUsers.Where(u => !ownersIds.Contains(u.UserId)));

            }
        }

        private DirectoryItemState directoryItemState { get; set; }
        private ObservableCollection<User> currentUsers { get; set; } = new ObservableCollection<User>();
        private ObservableCollection<User> nonOwners { get; set; } = new ObservableCollection<User>();
        private ObservableCollection<User> owners { get; set; } = new ObservableCollection<User>();
        private DirectoryItemServices directoryItemServices { get; set; }

        public DirectoryItem SelectedDirectoryItem { get; set; }
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

        private void AddOwner(object message)
        {
            Owners.Add(SelectedNonOwner);
            NonOwners.Remove(SelectedNonOwner);
        }
        private bool OwnerSelected(object message)
        {
            return SelectedOwner != null ? true : false;
        }
        private void RemoveOwner(object message)
        {
            NonOwners.Add(SelectedOwner);
            Owners.Remove(SelectedOwner);
        }

        private bool NonOwnerSelected(object message)
        {
            return SelectedNonOwner != null ? true : false;
        }
        private void SaveDirectoryInfo(object message)
        {
            directoryItemServices.Update(SelectedDirectoryItem);
            directoryItemServices.UpdateUserDirectoryItems(owners.ToList(), SelectedDirectoryItem);
            CloseWindow(message);
        }
        private void CloseWindow(object message)
        {
            ((Window)message).Close();
        }


    }
}
