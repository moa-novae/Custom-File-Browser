using System.ComponentModel;
using PropertyChanged;

namespace Q1
{
    /// <summary>
    /// A base view model that fires property changed events as needed
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event is fired when any child property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
