using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace Q1
{
    /// <summary>
    /// A helper class to query information about directories
    /// </summary>
    class DirectoryStructure
    {
        /// <summary>
        /// Get all logical drives on the computer
        /// </summary>
        /// <returns></returns>
        public static List<DirectoryItem> GetLogicalDrives()
        {
            var items = new List<DirectoryItem>();
            return Directory.GetLogicalDrives().Select(DriveInfo => new DirectoryItem { FullPath = DriveInfo, Type = DirectoryItemType.Drive }).ToList();
        }

        /// <summary>
        /// Gets the directorys top-level content
        /// </summary>
        /// <param name="fullPath">The full path to the directory</param>
        /// <returns></returns>
        public static List<DirectoryItem> GetDirectoryContents(string fullPath)
        {
            var items = new List<DirectoryItem>();
            #region Get directories
            // create a blank list for directories
            var directories = new List<string>();

            //try and get directories from the folder
            try
            {
                var dirs = Directory.GetDirectories(fullPath);
                if (dirs.Length > 0)
                    items.AddRange(dirs.Select(dir => new DirectoryItem { FullPath = dir, Type = DirectoryItemType.Folder }));
            }
            catch { }

            #endregion


            #region Get Files

            //try and get directories from the folder
            try
            {
                var fs = Directory.GetFiles(fullPath);
                if (fs.Length > 0)
                    items.AddRange(fs.Select(file => new DirectoryItem { FullPath = file, Type = DirectoryItemType.File }));
            }
            catch { }
            return items;


            }
        #endregion

        #region Helpers

        /// <summary>
        /// Find the title or folder name from a full path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileFolderName(string path)
        {
            return Path.GetFileName(path);
        }
        #endregion
    }
}
