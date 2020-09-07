using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q1.State
{
    public class UsersStore
    {
        private readonly ObservableCollection<User> currentUsers = new ObservableCollection<User> (UserServices.GetAllUsers());
        public ObservableCollection<User> CurrentUsers { 
            get 
            {
                return currentUsers;
            } 
            set 
            {
                currentUsers.Clear();
                foreach (var user in value)
                    currentUsers.Add(user);

            }
        }

             
        
    }
}
