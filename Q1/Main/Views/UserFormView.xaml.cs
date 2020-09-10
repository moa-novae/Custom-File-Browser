using System.Windows;
using Q1.Services;
using Q1Entity;
namespace Q1
{
    /// <summary>
    /// Interaction logic for AddUserView.xaml
    /// </summary>
    public partial class UserFormView : Window
    {
        // Constructor for creating brand new user
        public UserFormView(UserStateServices userServices)
        {
            InitializeComponent();
            DataContext = new UserFormViewModel(userServices);
        }
        //constructor for editing selected user
        public UserFormView(UserStateServices userServices, User selectedUser)
        {
            InitializeComponent();
            DataContext = new UserFormViewModel(userServices, selectedUser);
        }
    }
}
