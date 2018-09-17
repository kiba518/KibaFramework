using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utility;
 

namespace ViewModel 
{
    public class ListBox<T> : Control<T>
    { 
        public ListBox()
        {
            
        }
   
    
        public Action<T> SelectCallBack = null;


        public List<T> _ItemsSource;
        public List<T> ItemsSource { get { return _ItemsSource; } set { _ItemsSource = value; OnPropertyChanged(); } }

        public T _SelectedItem;
        public T SelectedItem { get { return _SelectedItem; }
            set
            {
                _SelectedItem = value;
                if (SelectCallBack != null)
                {
                    SelectCallBack(_SelectedItem);
                }
                OnPropertyChanged();
            }
        }

        private ICollectionView _ItemsSourceView;
        public ICollectionView ItemsSourceView
        {
            get
            {
                _ItemsSourceView = CollectionViewSource.GetDefaultView(_ItemsSource);
                return _ItemsSourceView;
            }
            set
            {
                _ItemsSourceView = value;
                OnPropertyChanged(); 
            }
        }
      
       


      
    }
}
