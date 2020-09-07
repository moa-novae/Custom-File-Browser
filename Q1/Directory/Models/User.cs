using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q1
{
    public class User
    {
        #region Public properties

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }


        public IList<UserDirectoryItem> UserDirectoryItems { get; set; } 
      


        #endregion

        #region Constructor

        public User(string name)
        {
            Name = name;
        }

        #endregion
    }
}
