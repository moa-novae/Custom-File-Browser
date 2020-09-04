using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q1
{
    public class UserDirectoryItem
    {
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int DirectoryItemId { get; set; }
        public DirectoryItem DirectoryItem { get; set; }
    }
}
