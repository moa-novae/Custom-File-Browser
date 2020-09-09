using System.Windows;
using Q1.State;

namespace Q1
{
    /// <summary>
    /// Interaction logic for EditDirectoryItemView.xaml
    /// </summary>
    public partial class EditDirectoryItemView : Window
    {
        public EditDirectoryItemView(DirectoryItemServices directoryItemServices, DirectoryItemState dirState, DirectoryItem selectedDirectoryItem, UserState userState)
        {
            DataContext = new EditDirectoryItemViewModel(directoryItemServices, dirState, selectedDirectoryItem, userState);

            InitializeComponent();
        }
    }
}
