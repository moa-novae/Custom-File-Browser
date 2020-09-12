using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Q1Entity;

namespace Q1Service
{
    public partial class SyncLocalDirectoryService : ServiceBase
    {
        /** If the service is enabled while the wpf application is running, and 
         * changes are made to the local directory, there will be errors when the
         * wpf application tries to do any database actions. I am not exactly sure 
         * how to inform wpf that changes are made to the local directory, and it needs
         * to refresh its current state. Perhaps I can also use filesystemWatcher in wpf?
         * I would have to check if db action is valid, and stop the user from doing the action
         * while refreshing the wpf state. 
         * 
         **/

        /// <summary>
        /// Windows service for syncing local file directory with the db
        /// </summary>
        public SyncLocalDirectoryService()
        {
            InitializeComponent();
            directoryItemServices = new DirectoryItemServices(); 
        }

        private DirectoryItemServices directoryItemServices { get; set; }

        protected override void OnStart(string[] args)
        {
            string path = @"C:\WpfTest";
            // Set up all the event listeners for file changes
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(path)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };
            fileSystemWatcher.Created += ItemCreated;
            fileSystemWatcher.Deleted += ItemDeleted;
            fileSystemWatcher.Renamed += ItemRenamed;

            // Check discrepencies between local file structure and db
            // List of paths that exist in db
            List<string> dbItemPaths = directoryItemServices.GetAllDirectoryItems().Select(item => item.FullPath).ToList();
            // List of paths found in local directory
            List <string> localItemPaths = GetAllDirectoryItemsAtPath(path);
            // Find files that exist locally but not in db
            List<string> FilesMissingInDb = localItemPaths.Except(dbItemPaths).ToList();
            // Find files that exist in db but not locally
            List<string> FilesMissingInLocal = dbItemPaths.Except(localItemPaths).ToList();

            // Delete the paths that only exists in db            
            directoryItemServices.Delete(FilesMissingInLocal);

            // Add the paths missing in db
            directoryItemServices.Add(FilesMissingInDb);
        }

        private void ItemRenamed(object sender, RenamedEventArgs e)
        {
            string newPath = e.FullPath;
            string oldPath = e.OldFullPath;
            DirectoryItem directoryItem = directoryItemServices.Get(oldPath);
            directoryItem.FullPath = newPath;
            directoryItemServices.Update(directoryItem);
            
        }

        private void ItemDeleted(object sender, FileSystemEventArgs e)
        {
            string deletedItemPath = e.FullPath;
            directoryItemServices.Delete(deletedItemPath);
            
        }

        private void ItemCreated(object sender, FileSystemEventArgs e)
        {
            string createdItemPath = e.FullPath;
            directoryItemServices.Add(createdItemPath);
        }

        protected override void OnStop()
        {
        }

        private List<string> GetAllDirectoryItemsAtPath(string path)
        {
            List<string> directoryItemPaths = new List<string>();

            // return every item in a folder, including the folder path
            void AddItemPathsToList(string dir)
            {
                //Console.WriteLine("DirSearch..(" + sDir + ")");
                try
                {
                    directoryItemPaths.Add(dir);

                    foreach (string f in Directory.GetFiles(dir))
                    {
                        directoryItemPaths.Add(f);
                    }

                    foreach (string d in Directory.GetDirectories(dir))
                    {
                        AddItemPathsToList(d);
                    }
                }
                catch (Exception excpt)
                {
                    Console.WriteLine(excpt.Message);
                }
            }

            // if it is a folder, traverse the directory tree and add all item paths
            if (Directory.Exists(path))
            {
                AddItemPathsToList(path);

                // remove the root path from the list since we don't want it to be saved into db
                directoryItemPaths.RemoveAll(i => i == path);
            }
            // if file, simply add the file path
            else if (File.Exists(path))
            {
                directoryItemPaths.Add(path);
            }
            // if nothing exists, return empty list
            return directoryItemPaths;
        }
    }
}
