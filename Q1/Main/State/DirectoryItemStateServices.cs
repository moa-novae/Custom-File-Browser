using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using Q1Entity;
using Q1.State;
using System.IO;

namespace Q1.Services
{
    // User services and directoryItemServices can definitely be refactored, 
    // But since there are currently only two services, it is a bit overkill
    public class DirectoryItemStateServices
    {
        #region Constructor 

        public DirectoryItemStateServices(DirectoryItemState s, UserState u)
        {
            directoryItemState = s;
            directoryItemServices = new DirectoryItemServices();
            userState = u;
            // sync local file structure with database
            SyncDbFileStructureToLocal();
        }

        #endregion

        #region Properties

        private UserState userState { get; set; }
        private DirectoryItemState directoryItemState { get; set; }
        private DirectoryItemServices directoryItemServices { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Add directory item based on path
        /// </summary>
        /// <param name="path"></param>
        public void Add(string path)
        {
            // The logic can definitely be refactored since the other overloaded add method uses similar logic
            DirectoryItem newDirectoryItem = directoryItemServices.Add(path);
            // Find the parend directory's path to find the parent node in the directory item tree
            var dir = new DirectoryInfo(path);
            string parentPath = dir.Parent.FullName;
            // find the parent node of the path
            DirectoryTreeNode parentNode = directoryItemState.Tree.GetNode(parentPath);
            // Add the new node as a child of the parent node
            parentNode.Add(new DirectoryTreeNode(path, newDirectoryItem));

        }
        /// <summary>
        /// Add directory items based on paths
        /// </summary>
        /// <param name="paths"></param>
        public void Add(List<string> paths)
        {
            List<DirectoryItem> directoryItems = directoryItemServices.Add(paths);
            foreach(var directoryItem in directoryItems)
            {
                // Find the parend directory's path to find the parent node in the directory item tree
                var dir = new DirectoryInfo(directoryItem.FullPath);
                string parentPath = dir.Parent.FullName;
                // find the parent node of the path
                DirectoryTreeNode parentNode = directoryItemState.Tree.GetNode(parentPath);
                // Add the new node as a child of the parent node
                parentNode.Add(new DirectoryTreeNode(directoryItem.FullPath, directoryItem));
            }
        }

        public void Delete(string path)
        {
            directoryItemServices.Delete(path);
            // I am not exactly sure the best way to delete a tree node and all its children if it has any
            // The easiest thing to do is to just update the entire tree
            // delete operation is quite rare, so I think it is acceptable for this case
            UpdateState();
        }
        public void Delete(List<string> paths)
        {
            directoryItemServices.Delete(paths);
            UpdateState();
        }

        // Get doesn't change state
        public DirectoryItem Get(string path)
        {
            return directoryItemServices.Get(path);
        }

        /// <summary>
        /// updates information of a directory item, not to be used for updating owners
        /// </summary>
        /// <param name="d"></param>
        public void Update(DirectoryItem d)
        {
            directoryItemServices.Update(d);
            var treeNode = directoryItemState.Tree.GetNode(d.FullPath);
            // updates the node
            treeNode.Item = d;
        }

        /// <summary>
        /// Updates the owner of a directory item
        /// </summary>
        /// <param name="updatedOwnerList"></param>
        /// <param name="selectedItem"></param>
        public void UpdateUserDirectoryItems(List<User> updatedOwnerList, DirectoryItem selectedItem)
        {
            List<UserDirectoryItem> newJoinTables = directoryItemServices.UpdateUserDirectoryItems(updatedOwnerList, selectedItem);
            // update current users and the directory items they own
            List<User> oldSelectedItemOwners = new List<User>();
            ObservableCollection<User> newUsersState = new ObservableCollection<User>(userState.CurrentUsers);
            foreach (var user in newUsersState)
            {
                // check if user is owner of the selected item in the new state
                bool isOwner = updatedOwnerList.FirstOrDefault(u => u.UserId == user.UserId) != null;
                // check in the current state, if user is owner
                bool wasOwner = user?.UserDirectoryItems?.FirstOrDefault(udi => udi.DirectoryItemId == selectedItem.DirectoryItemId) != null;
                // if the user becomes a new owner, add the directory item to the user's possession
                if (isOwner && !wasOwner)
                {
                    user.UserDirectoryItems.Add(newJoinTables.FirstOrDefault(udi => udi.UserId == user.UserId));
                }
                if (!isOwner && wasOwner)
                {
                    // remove the join table that connects the selected directory item and owner in question
                    UserDirectoryItem joinTableToBeRemoved = user.UserDirectoryItems.FirstOrDefault(udi => udi.DirectoryItemId == selectedItem.DirectoryItemId);
                    user.UserDirectoryItems.Remove(joinTableToBeRemoved);
                }
            }
            // updates the state
            userState.CurrentUsers = newUsersState;

        }

            /// <summary>
            /// Runs when class is constructed. Uses local file structure as reference, and ensures database is up to date
            /// </summary>
            private void SyncDbFileStructureToLocal()
        {
            ObservableCollection<DirectoryItem> dbDirectoryItems = directoryItemServices.GetAllDirectoryItems();
            List<string> localItemPaths;
            List<string> dbItemPaths;
            List<string> FilesMissingInDb;
            List<string> FilesMissingInLocal;
            string rootPath = directoryItemState.Tree.RootNode.Item.FullPath;
            // find path of items in local directory tree, not including root node
            localItemPaths = directoryItemState.Tree.FlattenedTreeDictionary.Keys.Where(path => path != rootPath).ToList();
            // find path of items stored in db
            dbItemPaths = dbDirectoryItems.Select(item => item.FullPath).ToList();
            // Find files that exist locally but not in db
            FilesMissingInDb = localItemPaths.Except(dbItemPaths).ToList();
            // Find files that exist in db but not locally
            FilesMissingInLocal = dbItemPaths.Except(localItemPaths).ToList();

            // Delete the paths that only exists in db            
            Delete(FilesMissingInLocal);

            // Add the paths missing in db
            Add(FilesMissingInDb);

            // Refresh so only valid directory items left
            dbDirectoryItems = directoryItemServices.GetAllDirectoryItems();

            // fetch information such as owners & notes from db and add to local tree nodes
            foreach (var dbDirectoryItem in dbDirectoryItems)
            {
                // ignore rootnode since it can not be observed
                if (dbDirectoryItem.FullPath != directoryItemState.Tree.RootNode.Item.FullPath)
                {
                    directoryItemState.Tree.GetNode(dbDirectoryItem.FullPath).Item = dbDirectoryItem;
                }
            }
        }

        // On larger applications, you definitely don't want to completely delete state and refresh it
        // It is better to only change the items affected by any database operations
        // For simplicity sake, sometimes this method is used, while acknowledging its downside
        private void UpdateState()
        {
            var DirectoryItemsFromDatabase = directoryItemServices.GetAllDirectoryItems();

            foreach (var dbDirectoryItem in DirectoryItemsFromDatabase)
            {
                // ignore rootnode since it can not be observed
                if (dbDirectoryItem.FullPath != directoryItemState.Tree.RootNode.Item.FullPath)
                {
                    var localTreeNode = directoryItemState.Tree.GetNode(dbDirectoryItem.FullPath);
                    if (localTreeNode != null)
                        directoryItemState.Tree.GetNode(dbDirectoryItem.FullPath).Item = dbDirectoryItem;
                }
            }
        }

        #endregion
    }
}
