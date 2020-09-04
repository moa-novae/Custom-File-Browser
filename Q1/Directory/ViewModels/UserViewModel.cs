using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Q1
{
    /// <summary>
    /// The view model for the application main Directory view
    /// </summary>
    public class UserViewModel : BaseViewModel
    {
        #region public properties
        /// <summary>
        /// A list of all directories on the machine
        /// </summary>
        public string AllUsers { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserViewModel()
        {
            using (var db = new DirectoryContext())
            {
                AllUsers = db.Users.OrderBy(u => u.UserId).First().Name;
            }
            
                
        }
        #endregion
    }
}