using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using Q1Entity;
using Q1.State;


namespace Q1.Services
{
    // User services and directoryItemServices can definitely be refactored, 
    // But since there are currently only two services, it is a bit overkill
    public class DirectoryItemStateServices
    {
        private DirectoryItemState state { get; set; }
        // On larger applications, you definitely don't want to completely delete state and refresh it
        // It is better to only change the items affected by any database operations
        // For simplicity sake, I will udpate the entire state everytime a database change occurs, while acknowledging its downside
        private void UpdateState()
        {
            var DirectoryItemsFromDatabase = directoryItemServices.GetAllDirectoryItems();

            foreach (var dbDirectoryItem in DirectoryItemsFromDatabase)
            {
                // ignore rootnode since it can not be observed
                if (dbDirectoryItem.FullPath != state.Tree.RootNode.Item.FullPath)
                {
                    var localTreeNode = state.Tree.GetNode(dbDirectoryItem.FullPath);
                    if (localTreeNode != null)
                        state.Tree.GetNode(dbDirectoryItem.FullPath).Item = dbDirectoryItem;
                }
            }
        }
        public DirectoryItemStateServices(DirectoryItemState s)
        {
            state = s;
            directoryItemServices = new DirectoryItemServices();
            // sync local file structure with database
            SyncDbFileStructureToLocal();
        }

        public DirectoryItemServices directoryItemServices { get; set; }


        public void Add(string path)
        {
            directoryItemServices.Add(path);
            UpdateState();

        }
        public void Add(List<string> paths)
        {
            directoryItemServices.Add(paths);
            UpdateState();

        }

        public void Delete(string path)
        {
            directoryItemServices.Delete(path);
            UpdateState();

        }
        public void Delete(List<string> paths)
        {
            directoryItemServices.Delete(paths);
            UpdateState();

        }

        // Get doesn't need to change state
        public DirectoryItem Get(string path)
        {
            return directoryItemServices.Get(path);
        }

        public void Update(DirectoryItem d)
        {
            directoryItemServices.Update(d);
            UpdateState();

        }

        public void UpdateUserDirectoryItems(List<User> updatedList, DirectoryItem selectedItem)
        {
            directoryItemServices.UpdateUserDirectoryItems(updatedList, selectedItem);
            UpdateState();
        }

        private void SyncDbFileStructureToLocal()
        {
            ObservableCollection<DirectoryItem> dbDirectoryItems = directoryItemServices.GetAllDirectoryItems();
            List<string> localItemPaths;
            List<string> dbItemPaths;
            List<string> FilesMissingInDb;
            List<string> FilesMissingInLocal;
            string rootPath = state.Tree.RootNode.Item.FullPath;
            // find path of items in local directory tree, not including root node
            localItemPaths = state.Tree.FlattenedTreeDictionary.Keys.Where(path => path != rootPath).ToList();
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


            // Refresh so only valid left
            dbDirectoryItems = directoryItemServices.GetAllDirectoryItems();

            // fetch information such as owners & notes from db and add to local tree nodes
            foreach (var dbDirectoryItem in dbDirectoryItems)
            {
                // ignore rootnode since it can not be observed
                if (dbDirectoryItem.FullPath != state.Tree.RootNode.Item.FullPath)
                {
                    state.Tree.GetNode(dbDirectoryItem.FullPath).Item = dbDirectoryItem;


                }
            }
        }
        




    }
}
