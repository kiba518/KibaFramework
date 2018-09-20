using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility; 
using System.Windows.Data;
using System.Data;
using System.Threading;
using System.Reflection;
using System.Collections.ObjectModel;

namespace ViewModel
{
  
    public class DataGrid<T> : Control<T>
    {
        private Action<T> LoadAction = null;
        public Action<T> SelectCallBack = null;
        private Func<object, bool> DataFilter = null;
       
        #region 分页 
        private volatile int _CurrentPage = 1;
        public int CurrentPage
        {
            get { return _CurrentPage; }
            set
            {
                _CurrentPage = value;
                if (_CurrentPage > PageCount)
                {
                    _CurrentPage = PageCount;
                }
                if (_CurrentPage < 1)
                {
                    _CurrentPage = 1;
                }
                OnPropertyChanged();
            }
        } 
        private int _PageCount = 1;
        public int PageCount  { get {  return _PageCount;  }  set {  _PageCount = value;   OnPropertyChanged();  }  }  
        private int _RecordCount = 0;
        public int RecordCount
        {
            get { return _RecordCount; }
            set
            {
                _RecordCount = value;
                if (_RecordCount <= SkipNumber)
                {
                    PageCount = 1;
                }
                else
                {
                    PageCount = int.Parse(Math.Ceiling((double)RecordCount / (double)SkipNumber).ToString());
                }
                if (_CurrentPage > PageCount)
                {
                    _CurrentPage = PageCount;
                } 
                OnPropertyChanged();
            }
        }
        private int _SkipNumber = 30;
        public int SkipNumber { get { return _SkipNumber; } set { _SkipNumber = value; OnPropertyChanged(); } }
        private TextBox<string> _JumpTextBox = new TextBox<string>();
        public TextBox<string> JumpTextBox
        {
            get { return _JumpTextBox; }
            set { _JumpTextBox = value; OnPropertyChanged(); }
        }
        #region 跳页
        public BaseCommand JumpCommand
        {
            get
            {
                return new BaseCommand(JumpCommand_Executed);
            }
        }
        void JumpCommand_Executed(object send)
        {
            int pagenum = 0;

            if (int.TryParse(JumpTextBox.Text, out pagenum))
            {
                if (pagenum <= PageCount && pagenum > 0)
                {
                    CurrentPage = pagenum;

                    if (LoadAction != null)
                    {
                        LoadAction(Condition);
                    }
                }
                else
                {
                    MessageBox.Show("请正确填写跳转页数。", "提示信息");
                }
            }
            else
            {
                MessageBox.Show("请正确填写跳转页数。", "提示信息");
            }
        }
        #endregion

        #region 上一页

        public BaseCommand PreviousCommand
        {
            get
            {
                return new BaseCommand(PreviousCommand_Executed);
            }
        }
        void PreviousCommand_Executed(object send)
        {
            if (CurrentPage > 1)
            {
                CurrentPage -= 1;
                if (LoadAction != null)
                {
                    LoadAction(Condition);
                }
            }
            else
            {
                MessageBox.Show("已至首页。", "提示信息");
            }
        }
        #endregion

        #region 下一页
        public BaseCommand NextCommand
        {
            get
            {
                return new BaseCommand(NextCommand_Executed);
            }
        }
        void NextCommand_Executed(object send)
        {
            if (CurrentPage < PageCount)
            {
                CurrentPage += 1;

                if (LoadAction != null)
                {
                    LoadAction(Condition);
                }
            }
            else
            {
                MessageBox.Show("已至末页。", "提示信息");
            }
        }

        #endregion

        #endregion

        private ObservableCollection<T> _ItemsSource = new ObservableCollection<T>();
        public ObservableCollection<T> ItemsSource
        {
            get { return _ItemsSource; }
            set
            {
                _ItemsSource = value;
                if (_ItemsSource != null && _ItemsSource.Count > 0 && SelectedItem == null)
                {
                    SelectedItem = _ItemsSource.First();
                }
                OnPropertyChanged(); 
            }
        }
        public void SetItemsSource(List<T> itemSource)
        {
            ItemsSource = new ObservableCollection<T>(itemSource);
        }
        public T _SelectedItem;
        public T SelectedItem
        {
            get { return _SelectedItem; }
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
        private T _Condition = (T)Activator.CreateInstance(typeof(T));
        public T Condition { get { return _Condition; } set { _Condition = value; OnPropertyChanged(); } }

        #region 方法  
        public DataGrid()
        {
        }
        public void BindSource(Action<T> loadAction, T conditionRow)
        {
            LoadAction = loadAction;
            if (LoadAction != null)
            {
                CurrentPage = 1;
                LoadAction(conditionRow);
            }
        }
        public void SetFilter(Func<object, bool>  dataFilter)
        {
            try
            {
                DataFilter = dataFilter;
                _ItemsSourceView = CollectionViewSource.GetDefaultView(_ItemsSource);
                _ItemsSourceView.Filter = new Predicate<object>(DataFilter);
            }
            catch(Exception ex)
            {
               
            }
        } 
        public void Refresh()
        {
            if (_ItemsSourceView == null)
            {
                _ItemsSourceView = CollectionViewSource.GetDefaultView(this.ItemsSource);
            }
            _ItemsSourceView.Refresh();
        }
        #endregion
    }
}
