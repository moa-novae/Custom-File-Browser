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
    public class DirectoryItemServices
    {

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
        public DirectoryItem Get(string path)
        {
            using (var db = new DirectoryContext())
            {
                db.DirectoryItems.Load();
                return db.DirectoryItems.FirstOrDefault(u => u.FullPath == path);
            }
        }
        public DirectoryItem Update(DirectoryItem d)
        {
            using (var db = new DirectoryContext())
            {
                db.DirectoryItems.Update(d);
                db.SaveChanges();
            }
            return d;
        }

        public List<User> UpdateUserDirectoryItems(List<User> updatedList, DirectoryItem selectedItem)
        {
            using (var db = new DirectoryContext())
            {
                // Get the join tables of the selected directory item
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
                // update the old join tables with the new ones
                db.TryUpdateManyToMany(currentJoinTables, newJoinTables, x => x.UserId);
                db.SaveChanges();
            }
            return updatedList;
        }
    }
}
