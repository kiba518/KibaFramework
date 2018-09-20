using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class TextBlock<T> : Control<T>
    { 
        public T _Text;
        public T Text
        {
            get { return _Text; }
            set
            {
                _Text = value; 
                OnPropertyChanged();
            }
        } 
    }
}
