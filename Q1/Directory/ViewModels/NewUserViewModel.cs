using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Q1
{
    public class NewUserViewModel : BaseViewModel
    {
        public RelayCommand CreateNewUserCommand { get; private set; }
        public RelayCommand CloseWindowCommand { get; private set; }
        #region Public User properties
        public String Name { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        
        #endregion


        public NewUserViewModel()
        {
            CreateNewUserCommand = new RelayCommand(CreateNewUser, CanCreateNewUser);
            CloseWindowCommand = new RelayCommand(o => ((Window)o).Close());
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
            using (var db = new DirectoryContext())
            {
                db.Add(new User(Name)
                {
                    Email = Email,
                    Phone = Phone
                });
                db.SaveChanges();

            }

        }
        
    }
}
