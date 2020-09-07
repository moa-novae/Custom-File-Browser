using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Q1.State;

namespace Q1
{
    public class UserServices
    {
        private UsersStore state { get; set; }
        private void updateState ()
        {
            state.CurrentUsers = GetAllUsers();
        }
        public UserServices(UsersStore userStore)
        {
            state = userStore;
        }
        public static ObservableCollection<User> GetAllUsers()
        {
            using (var db = new DirectoryContext())
            { 
                db.Users.Load();
                var temp = db.Users.Local.ToObservableCollection();
                // return the copy so it persists after context is disposed
                return new ObservableCollection<User>(temp);
            }
        }
        public void Add(User u)
        {
            using (var db = new DirectoryContext())
            {
                db.Add(u);
                db.SaveChanges();
                updateState();
            }
        }
        public User Get(int Id)
        {
            using (var db = new DirectoryContext())
            {
                return db.Users.Single(u => u.UserId == Id);
            }
        }
        public void Delete(int Id)
        {
            using (var db = new DirectoryContext())
            {
                User user = new User(null) { UserId = Id };
                db.Users.Attach(user);
                db.Users.Remove(user);
                db.SaveChanges();
                updateState();
            }
        }
        public void Update(User u)
        {
            using (var db = new DirectoryContext())
            {
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                updateState();
            }

        }

    }
}

