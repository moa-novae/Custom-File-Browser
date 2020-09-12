using System.Collections.ObjectModel;
using Q1Entity;
namespace Q1.State
{
    // state is reponsible for keep track of who are the users and what do they own
    public class UserState : BaseViewModel
    {
        private ObservableCollection<User> currentUsers = new ObservableCollection<User>();
       
        public ObservableCollection<User> CurrentUsers
        {
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
