using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Q1.State;
using Q1Entity;

namespace Q1.Services
{
    public class UserStateServices
    {
        #region Constructor

        public UserStateServices(UserState us)
        {
            userState = us;
            userServices = new UserServices();
            // initialize user state
            userState.CurrentUsers = userServices.GetAllUsers();
        }

        #endregion

        #region Properties

        private UserState userState { get; set; }
        private UserServices userServices { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Add a user based on passed in entity
        /// </summary>
        /// <param name="u"></param>
        public void Add(User u)
        {
            userServices.Add(u);
            u.UserDirectoryItems = new List<UserDirectoryItem>();
            userState.CurrentUsers.Add(u);

        }

        /// <summary>
        /// Get a user based on UserId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public User Get(int Id)
        {
            return userServices.Get(Id);

        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="u"></param>
        public void Delete(User u)
        {
            userServices.Delete(u);
            userState.CurrentUsers.Remove(u);

        }

        /// <summary>
        /// Update user inforamtion. Don't use this for adding owned directory items
        /// </summary>
        /// <param name="u"></param>
        public void Update(User u)
        {
            userServices.Update(u);
            User oldUser = userState.CurrentUsers.FirstOrDefault(user => user.UserId == u.UserId);
            // remove the old user
            userState.CurrentUsers.Remove(oldUser);
            // and repalce it with the new one
            userState.CurrentUsers.Add(u);
        }

        #endregion
    }
}

