using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Q1Entity
{
    /// <summary>
    /// Interact with database and modifies information about users
    /// </summary>
    public class UserServices
    {
        /// <summary>
        /// Get all users registered
        /// </summary>
        /// <returns>An observable collection of all the users</returns>
        public ObservableCollection<User> GetAllUsers()
        {
            using (var db = new DirectoryContext())
            {
                db.Users.Include(u => u.UserDirectoryItems).ThenInclude(udi => udi.DirectoryItem).Load();
                var temp = db.Users.Local.ToObservableCollection();
                // return the copy so it persists after context is disposed
                return new ObservableCollection<User>(temp);
            }
        }

        /// <summary>
        /// Add a user
        /// </summary>
        /// <param name="u"></param>
        /// <returns>The new user which is added</returns>
        public User Add(User u)
        {
            using (var db = new DirectoryContext())
            {
                db.Add(u);
                db.SaveChanges();
                
            }
            return u;
        }

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>The user requested by id</returns>
        public User Get(int Id)
        {
            using (var db = new DirectoryContext())
            {
                return db.Users.Single(u => u.UserId == Id);
            }
        }

        /// <summary>
        /// Delete a user from the database
        /// </summary>
        /// <param name="u"></param>
        /// <returns>the deleted user</returns>
        public User Delete(User u)
        {
            using (var db = new DirectoryContext())
            {
                User user = new User(null) { UserId = u.UserId };
                db.Users.Attach(user);
                db.Users.Remove(user);
                db.SaveChanges();
               
            }
            return u;
 
        }

        /// <summary>
        /// Update user information. Don't use this for adding users to directoy items
        /// </summary>
        /// <param name="u"></param>
        /// <returns>the updated user</returns>
        public User Update(User u)
        {
            using (var db = new DirectoryContext())
            {
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                
            }
            return u;

        }

    }
}

