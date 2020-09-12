using System.Collections.Generic;
using System.IO;
using System.Linq;
using Q1Entity;
namespace Q1
{
    /// <summary>
    /// A helper class to query information about directories
    /// </summary>
    class DirectoryStructure
    {
        #region Methods
        /// <summary>
        /// Gets the directory's top-level content
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
                    items.AddRange(dirs.Select(dir => new DirectoryItem(dir, DirectoryItemType.Folder)));
            }
            catch { }

            #endregion

            #region Get Files

            //try and get files from the folder
            try
            {
                var fs = Directory.GetFiles(fullPath);
                if (fs.Length > 0)
                    items.AddRange(fs.Select(file => new DirectoryItem(file, DirectoryItemType.File)));
            }
            catch { }
            return items;
            #endregion


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
