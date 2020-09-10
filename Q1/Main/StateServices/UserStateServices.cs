using Q1.State;
using Q1Entity;

namespace Q1.Services
{
    public class UserStateServices
    {
        private UserState state { get; set; }
        private UserServices userServices { get; set; }
        private void updateState()
        {
            state.CurrentUsers = userServices.GetAllUsers();
        }
        public UserStateServices(UserState userState)
        {
            state = userState;
            userServices = new UserServices();
            state.CurrentUsers = userServices.GetAllUsers();
        }

        public void Add(User u)
        {
            userServices.Add(u);
            updateState();

        }
        public User Get(int Id)
        {
            return userServices.Get(Id);

        }
        public void Delete(int Id)
        {
            userServices.Delete(Id);
            updateState();

        }
        public void Update(User u)
        {
            userServices.Update(u);
            updateState();


        }

    }
}

