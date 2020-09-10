using System;
using System.Windows;
using Q1.Services;
using Q1Entity;
namespace Q1
{
    public class UserFormViewModel : BaseViewModel
    {
        private UserStateServices userServices { get; set; }
        public RelayCommand CreateNewUserCommand { get; private set; }
        public RelayCommand CloseWindowCommand { get; private set; }
        #region Public User properties
        public String Name { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public int? UserId { get; set; }

        #endregion
        // This view model is responsible for both creating users and editing them

        /// <summary>
        /// Constructor for creating new users
        /// </summary>
        /// <param name="us"></param>
        public UserFormViewModel(UserStateServices us)
        {
            userServices = us;
            CreateNewUserCommand = new RelayCommand(CreateNewUser, CanCreateNewUser);
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        /// <summary>
        /// Constructor editing existing new users
        /// </summary>
        /// <param name="us"></param>
        /// <param name="user"></param>
        public UserFormViewModel(UserStateServices us, User user)
            : this(us)
        {
            Name = user.Name;
            Email = user.Email;
            Phone = user.Phone;
            UserId = user.UserId;
        }


        public bool CanCreateNewUser(object message)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return false;
            else
                return true;
        }

        public void CreateNewUser(object message)
        {
            // UserId only has value when editing user
            if (UserId == null)
            {
                User u = new User(Name) { Email = Email, Phone = Phone };
                userServices.Add(u);
            }
            else
            {
                User editedUser = new User(Name) { UserId = (int)UserId, Email = Email, Phone = Phone };
                userServices.Update(editedUser);
            }

            CloseWindow(message);

        }

        public void CloseWindow(object message)
        {
            ((Window)message).Close();
        }

    }
}
