using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapsUnofficial.ViewModel.SettingsView
{
    class SettingsMainVM : INotifyPropertyChanged
    {
        private bool _allowOverstretch;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool AllowOverstretch
        {
            get { return _allowOverstretch; }
            set { _allowOverstretch = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowOverstretch"); }
        }

        public SettingsMainVM()
        {

        }
    }
}
