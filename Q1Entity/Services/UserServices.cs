using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Q1Entity
{
    public class UserServices
    {
        
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
        public User Add(User u)
        {
            using (var db = new DirectoryContext())
            {
                db.Add(u);
                db.SaveChanges();
                
            }
            return u;
        }
        public User Get(int Id)
        {
            using (var db = new DirectoryContext())
            {
                return db.Users.Single(u => u.UserId == Id);
            }
        }
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

