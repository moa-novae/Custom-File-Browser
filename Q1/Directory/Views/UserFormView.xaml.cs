using System.Windows;

namespace Q1
{
    /// <summary>
    /// Interaction logic for AddUserView.xaml
    /// </summary>
    public partial class UserFormView : Window
    {
        // Constructor for creating brand new user
        public UserFormView(UserServices userServices)
        {
            InitializeComponent();
            DataContext = new UserFormViewModel(userServices);
        }
        //constructor for editing selected user
        public UserFormView(UserServices userServices, User selectedUser)
        {
            InitializeComponent();
            DataContext = new UserFormViewModel(userServices, selectedUser);
        }
    }
}
