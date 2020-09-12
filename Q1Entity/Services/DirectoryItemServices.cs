using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Q1Entity
{
    // Since database is local, I have elected to use synchronous db operations for simplicity sake
    // if the data is requested through some form of web API, the db operations needs to be async to not block the application
    /// Interact with database and modify information about directory items.
    /// </summary>
    public class DirectoryItemServices
    {
        /// <summary>
        /// Gets information of all the directory items within the root folder. Data is eager loaded
        /// </summary>
        /// <returns>An observable collection of all the directory items within the root foler. The root folder is not included</returns>
        public ObservableCollection<DirectoryItem> GetAllDirectoryItems()
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

        /// <summary>
        /// Add a directory item to db based on path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The directory item added</returns>
        public DirectoryItem Add(string path)
        {
            DirectoryItem newDirectoryItem;
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
                newDirectoryItem = new DirectoryItem(path, type);
                db.Add(newDirectoryItem);
                db.SaveChanges();
            }
            return newDirectoryItem;
        }

        // Overloaded method for accepting list of paths
        /// <summary>
        /// Add multiple directory items to the db with a list of full paths
        /// </summary>
        /// <param name="paths"></param>
        /// <returns>The newly added directory items</returns>
        public List<DirectoryItem> Add(List<string> paths)
        {
            List<DirectoryItem> newDirectoryItems = new List<DirectoryItem>();
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
                    newDirectoryItems.Add(newDirectoryItem);
                    db.Add(newDirectoryItem);
                }
                db.SaveChanges();
            }
            return newDirectoryItems;
        }

        /// <summary>
        /// Delete a directory item from the db based on path
        /// </summary>
        /// <param name="path">Full path of the directory item to be deleted</param>
        /// <returns>The deleted directory item</returns>
        public DirectoryItem Delete(string path)
        {
            List<DirectoryItem> deletedDirectoryItems;
            using (var db = new DirectoryContext())
            {
                db.DirectoryItems.Load();
                // Is a list in case multiple directory items with same path is accidentally saved
                deletedDirectoryItems = db.DirectoryItems.Where(item => item.FullPath == path).ToList();
                foreach (var dI in deletedDirectoryItems)
                    db.DirectoryItems.Remove(dI);
                db.SaveChanges();
            }
            return deletedDirectoryItems.FirstOrDefault();
        }

        // Overloaded delete that accepts a list of paths instead of only one path
        /// <summary>
        /// Delete directory items from a list of full paths
        /// </summary>
        /// <param name="paths"></param>
        /// <returns>A list of deleted directory items</returns>
        public List<DirectoryItem> Delete(List<string> paths)
        {
            List<DirectoryItem> deletedDirectoryItems = new List<DirectoryItem>();
            using (var db = new DirectoryContext())
            {
                db.DirectoryItems.Load();

                foreach (var path in paths)
                {
                    deletedDirectoryItems = db.DirectoryItems.Where(item => item.FullPath == path).ToList();
                    // delete all dbItems with same path
                    foreach (var dI in deletedDirectoryItems)
                    {
                        db.DirectoryItems.Remove(dI);
                    }
                }
                db.SaveChanges();
            }
            return deletedDirectoryItems;
        }

        /// <summary>
        /// Get a directory item based on path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The directory item requested with full path</returns>
        public DirectoryItem Get(string path)
        {
            using (var db = new DirectoryContext())
            {
                db.DirectoryItems.Load();
                return db.DirectoryItems.FirstOrDefault(u => u.FullPath == path);
            }
        }

        /// <summary>
        /// Updates properties of a directory item
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public DirectoryItem Update(DirectoryItem d)
        {
            using (var db = new DirectoryContext())
            {
                db.Entry(d).State = EntityState.Modified;
                db.SaveChanges();
            }
            return d;
        }

        /// <summary>
        /// Assign users to a directory item
        /// </summary>
        /// <param name="updatedList">The up to date owners of the directory item</param>
        /// <param name="selectedItem">The directory item which we need to assign owners for</param>
        /// <returns>Join tables that link the directory item passed in the params to users assigned to it</returns>
        public List<UserDirectoryItem> UpdateUserDirectoryItems(List<User> updatedList, DirectoryItem selectedItem)
        {
            List<UserDirectoryItem> newJoinTables = new List<UserDirectoryItem>();
            using (var db = new DirectoryContext())
            {
                // Get the join tables of the selected directory item
                var model = db.DirectoryItems
                 .Include(d => d.UserDirectoryItems)
                 .FirstOrDefault(d => d.DirectoryItemId == selectedItem.DirectoryItemId);
                var currentJoinTables = model.UserDirectoryItems;
                // convert List<user> to List<UserDirectoryItems>, which represents what updated join tables should be
                newJoinTables = updatedList.Select(
                    u => new UserDirectoryItem
                    {
                        UserId = u.UserId,
                        DirectoryItemId = selectedItem.DirectoryItemId,

                    }).ToList();
                // update the old join tables with the new ones
                db.TryUpdateManyToMany(currentJoinTables, newJoinTables, x => x.UserId);
                db.SaveChanges();
            }

            // add link to users and directoryitems after db save to avoid error => Cannot insert explicit value for identity column in table
            foreach (var table in newJoinTables)
            {
                table.User = updatedList.FirstOrDefault(u => u.UserId == table.UserId);
                table.DirectoryItem = selectedItem;
            }
            return newJoinTables;
        }


    }
}
