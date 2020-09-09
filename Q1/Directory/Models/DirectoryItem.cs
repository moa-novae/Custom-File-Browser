using System;
using System.Collections.Generic;

namespace Q1
{
    /// <summary>
    /// Information about a directory item suchas a file
    /// </summary>
    public class DirectoryItem
    {
        #region Public properties
        /// <summary>
        /// Id of item
        /// </summary>
        public int DirectoryItemId { get; set; }

        /// <summary>
        /// Additional Description of the item
        /// </summary>
        public String Notes { get; set; }

        /// <summary>
        /// The type of this item
        /// </summary>
        public DirectoryItemType Type { get; set; }
        /// <summary>
        /// The absolute path to this item
        /// </summary>
        public string FullPath { get; set; }

        public DirectoryItem(string fullPath, DirectoryItemType type)
        {
            FullPath = fullPath;
            Type = type;
        }

        /// <summary>
        /// File name or folder name
        /// </summary>
        public string Name { get { return DirectoryStructure.GetFileFolderName(FullPath); } }

        public List<UserDirectoryItem> UserDirectoryItems { get; set; }

        #endregion

    }
}
