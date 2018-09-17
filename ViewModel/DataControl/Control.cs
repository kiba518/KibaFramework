using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility;

namespace ViewModel
{
    public class Control<T> : INotifyPropertyChanged
    { 
        public event PropertyChangedEventHandler PropertyChanged;

        public T _DataContent ;
        public T DataContent { get { return _DataContent; } set { _DataContent = value; OnPropertyChanged(); } }

        public Visibility _Visibility;
        public Visibility Visibility { get { return _Visibility; } set { _Visibility = value; OnPropertyChanged(); } }

        public bool _IsReadOnly;
        public bool IsReadOnly { get { return _IsReadOnly; } set { _IsReadOnly = value; OnPropertyChanged(); } }

        public bool _IsEnabled;
        public bool IsEnabled { get { return _IsEnabled; } set { _IsEnabled = value; OnPropertyChanged(); } }

       

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
