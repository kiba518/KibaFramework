using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModel
{
    public class CheckBox<T> : Control<T>
    {
        public Action<T> ClickCallBack = null;

        public T _IsChecked;
        public T IsChecked { get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                OnPropertyChanged();
                if (ClickCallBack != null)
                {
                    ClickCallBack(_IsChecked);
                }
            }
        }

        public string _Content;
        public string Content { get { return _Content; } set { _Content = value; OnPropertyChanged(); } }

       
      
      
    }
}
