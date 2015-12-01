using System.Collections.ObjectModel;

namespace TestWpfApplication
{
    public class PickerContext
    {
        private bool _hideAll;

        public PickerContext()
        {
            Items = new ObservableCollection<PickerItem>();
        }

        public string PropertyDescription { get; set; }

        public bool HideAll
        {
            get { return _hideAll; }
            set
            {
                _hideAll = value;
                foreach (var pickerItem in Items)
                {
                    pickerItem.Hide = value;
                }
            }
        }

        public ObservableCollection<PickerItem> Items { get; set; }
    }
}