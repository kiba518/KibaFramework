using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModel 
{
    public class Image<T> : Control<T>
    {


        public T _Source;
        public T Source
        {
            get { return _Source; }
            set { _Source = value; OnPropertyChanged(); }
        }

    }
}
