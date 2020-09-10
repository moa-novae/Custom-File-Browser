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
            }
        }
        // Overloaded method for accepting list of paths
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
            }
        }
        // Overloaded delete that accepts a list of paths instead of only one path
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
            }
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
            }
        }
    }
}
