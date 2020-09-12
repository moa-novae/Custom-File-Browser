using System.Windows;
using Q1.Services;
using Q1.State;
using Q1Entity;
namespace Q1
{
    /// <summary>
    /// Interaction logic for EditDirectoryItemView.xaml
    /// </summary>
    public partial class EditDirectoryItemView : Window
    {
        public EditDirectoryItemView(DirectoryItemStateServices directoryItemServices, DirectoryItemState dirState, DirectoryItem selectedDirectoryItem, UserState userState)
        {
            DataContext = new EditDirectoryItemViewModel(directoryItemServices, selectedDirectoryItem, userState);

            InitializeComponent();
        }
    }
}
