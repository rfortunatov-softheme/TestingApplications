using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestWpfApplication
{
    public class PickerItem : INotifyPropertyChanged
    {
        private bool _hide;

        public int Num { get; set; }

        public bool Hide
        {
            get { return _hide; }
            set
            {
                _hide = value;
                OnPropertyChanged();
            }
        }

        public string Text { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}