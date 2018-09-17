using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class TextBlock<T> : INotifyPropertyChanged
    {
        public Action<T> TextChangeCallBack = null;

        public event PropertyChangedEventHandler PropertyChanged;

        private T _Text;

        public T Text
        {
            get { return _Text; }
            set
            {

                _Text = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }
                if (TextChangeCallBack != null)
                {
                    TextChangeCallBack(_Text);
                }
            }
        }

        private System.Windows.Visibility _Visibility;

        public System.Windows.Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                _Visibility = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Visibility"));
                }
            }
        }
    }
}
