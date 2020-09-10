using System.Collections.Generic;

namespace Entity
{
    public class User
    {
        #region Public properties

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }


        public List<UserDirectoryItem> UserDirectoryItems { get; set; }



        #endregion

        #region Constructor

        public User(string name)
        {
            Name = name;
        }

        #endregion
    }
}
