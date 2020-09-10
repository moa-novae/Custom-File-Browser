using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Q1.State;

namespace Q1
{
    // User services and directoryItemServices can definitely be refactored, 
    // But since there are currently only two services, it is a bit overkill
    public class DirectoryItemServices
    {
        private DirectoryItemState state { get; set; }
        // On larger applications, you definitely don't want to completely delete state and refresh it
        // It is better to only change the items affected by any database operations
        // For simplicity sake, I will udpate the entire state everytime a database change occurs, while acknowledging its downside
        private void UpdateState()
        {
            var DirectoryItemsFromDatabase = GetAllDirectoryItems();

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
        public DirectoryItemServices(DirectoryItemState s)
        {
            state = s;
            // sync local file structure with database
            SyncDbFileStructureToLocal();
        }
        public static ObservableCollection<DirectoryItem> GetAllDirectoryItems()
        {
            using (var db = new DirectoryContext())
            {
                // eager load all related data
                var directoryItems = db.DirectoryItems
                    .Include(d => d.UserDirectoryItems)
                    .ThenInclude(ud => ud.User)
                    .ToList();
                // return the copy so it persists after context is disposed
                return new ObservableCollection<DirectoryItem>(directoryItems);
            }
        }

        public void Add(string path)
        {
            // Find if item at path if file or directory
            FileAttributes attr = File.GetAttributes(path);
            DirectoryItemType type;
            if (attr.HasFlag(FileAttributes.Directory))
            {
                type = DirectoryItemType.Folder;
            }
            else
            {
                type = DirectoryItemType.File;
            }
            using (var db = new DirectoryContext())
            {
                DirectoryItem newDirectoryItem = new DirectoryItem(path, type);
                db.Add(newDirectoryItem);
                db.SaveChanges();
                UpdateState();
            }
        }
        public void Add(List<string> paths)
        {
            using (var db = new DirectoryContext())
            {
                foreach (var path in paths)
                {
                    // Find if item at path if file or directory
                    FileAttributes attr = File.GetAttributes(path);
                    DirectoryItemType type;
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        type = DirectoryItemType.Folder;
                    }
                    else
                    {
                        type = DirectoryItemType.File;
                    }
                    DirectoryItem newDirectoryItem = new DirectoryItem(path, type);
                    db.Add(newDirectoryItem);
                }
                db.SaveChanges();
                UpdateState();
            }
        }

        public void Delete(string path)
        {
            using (var db = new DirectoryContext())
            {
                db.DirectoryItems.Load();
                List<DirectoryItem> directoryItems = db.DirectoryItems.Where(item => item.FullPath == path).ToList();
                foreach (var dI in directoryItems)
                    db.DirectoryItems.Remove(dI);
                db.SaveChanges();
                UpdateState();
            }
        }
        public void Delete(List<string> paths)
        {
            using (var db = new DirectoryContext())
            {
                db.DirectoryItems.Load();

                foreach (var path in paths)
                {
                    List<DirectoryItem> directoryItems = db.DirectoryItems.Where(item => item.FullPath == path).ToList();
                    // delete all dbItems with same path
                    foreach (var dI in directoryItems)
                    {
                        db.DirectoryItems.Remove(dI);
                    }
                }
                db.SaveChanges();
                UpdateState();
            }
        }

        public DirectoryItem Get(int Id)
        {
            using (var db = new DirectoryContext())
            {
                db.DirectoryItems.Load();
                return db.DirectoryItems.Single(u => u.DirectoryItemId == Id);
            }
        }

        public DirectoryItem Update(DirectoryItem d)
        {
            using (var db = new DirectoryContext())
            {
                db.DirectoryItems.Update(d);
                db.SaveChanges();
                UpdateState();
            }
            return d;
        }

        public void UpdateUserDirectoryItems(List<User> updatedList, DirectoryItem selectedItem)
        {
            using (var db = new DirectoryContext())
            {
                var model = db.DirectoryItems
                 .Include(d => d.UserDirectoryItems)
                 .FirstOrDefault(d => d.DirectoryItemId == selectedItem.DirectoryItemId);
                var currentJoinTables = model.UserDirectoryItems;
                // convert List<user> to List<UserDirectoryItems>, which represents what updated join tables should be
                var newJoinTables = updatedList.Select(
                    u => new UserDirectoryItem
                    {
                        UserId = u.UserId,
                        DirectoryItemId = selectedItem.DirectoryItemId
                    });
                db.TryUpdateManyToMany(currentJoinTables, newJoinTables, x => x.UserId);
                db.SaveChanges();
                UpdateState();
            }
        }

        private void SyncDbFileStructureToLocal()
        {
            ObservableCollection<DirectoryItem> dbDirectoryItems = GetAllDirectoryItems();
            List<string> localItemPaths;
            List<string> dbItemPaths;
            List<string> FilesMissingInDb;
            List<string> FilesMissingInLocal;
            // Check if a valid rootpath was passed in
            // If nothing exists at the rootpath, no directory was created

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
            dbDirectoryItems = GetAllDirectoryItems();

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
        // if nothing exists at root, and root is an invalid address, delete all directory items in db




    }
}
