using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q1
{
    /// <summary>
    /// Information about a directory item suchas a file
    /// </summary>
    public class DirectoryItem
    {
        #region Public properties

        /// <summary>
        /// Owner of the item
        /// </summary>
        public User Owner { get; set; }

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

        /// <summary>
        /// File name or folder name
        /// </summary>
        public string Name { get { return DirectoryStructure.GetFileFolderName(this.FullPath); } }

        #endregion

    }
}
