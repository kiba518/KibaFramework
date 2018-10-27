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
using Model;

namespace ViewModel
{
    public interface IDataGrid
    {
        ObservableCollection<Object> ItemsSource { get; }
        Object SelectedItem { get; }
        Object Condition { get; }

        event Action<Object> ItemsSourceChange;
        string DataGridName { get; set; }

        Action<Object, Object, Object> TextChange { get; set; }
        Action<Object, Object, Object, Object, Object> TextLostFocus { get; set; }
        Action<Object> AllCheck { get; set; }
        Action ExcuteItemsSourceChange { get; set; }
        string DetailVisibility { get; set; }
        List<Object> DetailSource { get; set; }
        bool IsBusyDetail { get; set; }

    }

    public class DataGrid<T> : Control<T>, IDataGrid
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
                if (ItemsSourceChange != null)
                {
                    ItemsSourceChange(this);
                }
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
                if (_SelectedItem == null || !_SelectedItem.Equals(value))
                {
                    _SelectedItem = value;
                    if (SelectCallBack != null)
                    {
                        SelectCallBack(_SelectedItem);
                    }
                    OnPropertyChanged();
                }
            }
        }
        private ListCollectionView _ListCollectionView;
        public ListCollectionView ListCollectionView
        {
            get
            {
                _ListCollectionView = this.ItemsSourceView as ListCollectionView;
                return _ListCollectionView;
            }
            set
            {
                _ListCollectionView = value;
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
            typePropertylist = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).ToList();
        }
        public void BindSource(Action<T> loadAction, T conditionRow = default(T))
        {
            LoadAction = loadAction;
            if (LoadAction != null)
            {
                CurrentPage = 1;
                LoadAction(conditionRow);
            }
        }
        public void BindSource(Action loadAction)
        {
            LoadAction = new Action<T>((obj) => {
                loadAction();
            }); ;
            if (LoadAction != null)
            {
                CurrentPage = 1;
                LoadAction(default(T));
            }
        }
        public void ItemsSourceReBind()
        {
            BindSource(LoadAction);
        }
        public void SelectedItemReBind()
        {
            T newitem = (T)Activator.CreateInstance(typeof(T));
            List<System.Reflection.PropertyInfo> plist = typeof(T).GetProperties().ToList();

            foreach (var propertyInfo in plist)
            {
                propertyInfo.SetValue(newitem, propertyInfo.GetValue(SelectedItem));
            }
            SelectedItem = newitem;
        }
        #region 过滤
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
        public List<PropertyInfo> typePropertylist = new List<PropertyInfo>();
        private long usingResource = 0;
        List<FilterProperty> ComparePropertyList = new List<FilterProperty>();
        public List<string> NumberColumn = new List<string>() {
            "decimal",
            "int",
            "int32",
            "int64",
            "long",
            "double",
            "float"
        };

        public BaseCommand FilterCommand
        {
            get
            {
                return new BaseCommand(FilterCommand_Excute);
            }
        }

        private void FilterCommand_Excute(object obj)
        {
            var txt = obj as System.Windows.Controls.TextBox;
            string text = txt.Text;
            bool excuteFilter = false;

            #region 锁检测
            if (1 == Interlocked.Read(ref usingResource))
            {
                return;
            }
            //原始值是0，判断是时候使用原始值，但判断后值为1，进行了设置
            if (0 == Interlocked.Exchange(ref usingResource, 1))
            {
                if (text != null)
                { 
                    if (text == "")
                    {
                        excuteFilter = true;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            excuteFilter = true;
                        }
                        else
                        {

                        }
                    }
                }
            }
            #endregion

            try
            {
                if (excuteFilter)
                { 
                     
                    if (txt.IsFocused)
                    {
                        if (ItemsSource != null && ItemsSource.Count > 0)
                        {
                            if (ComparePropertyList == null)//筛选比较的列
                            {
                                ComparePropertyList = new List<FilterProperty>();
                            }

                            string enColumnName = txt.DataContext.ToString();//属性名  英文名  

                           
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                PropertyInfo filterProperty = typePropertylist.FirstOrDefault(fp => fp.Name == enColumnName);
                                if (filterProperty != null)
                                {
                                    object filterValue;
                                    string conditionStr = GetValueAndCondition(text, out filterValue);
                                    if (!text.StartsWith("=") && !text.StartsWith("!like") && "string" == filterProperty.PropertyType.ToString().Replace("System.Nullable`1[", "").Replace("]", "").Replace("System.", "").ToLower())
                                    {
                                        conditionStr = "like";
                                    }
                                    FilterProperty filterfp = ComparePropertyList.FirstOrDefault(p => p.PropertyName == enColumnName);
                                    if (filterfp == null)
                                    {
                                        FilterProperty fp = new FilterProperty();
                                        fp.PropertyName = enColumnName;
                                        fp.ConditionStr = conditionStr;
                                        fp.PropertyInfo = filterProperty;
                                        fp.PropertyValue = filterValue;
                                        ComparePropertyList.Add(fp);
                                    }
                                    else
                                    {
                                        filterfp.PropertyName = enColumnName;
                                        filterfp.ConditionStr = conditionStr;
                                        filterfp.PropertyValue = filterValue;
                                    }

                                    object value = ChangeValueForType(filterProperty.PropertyType, text);
                                    filterProperty.SetValue(Condition, value, null);

                                }
                            }
                            else
                            {
                                PropertyInfo filterProperty = typePropertylist.FirstOrDefault(fp => fp.Name == enColumnName);
                                if (filterProperty != null)
                                {
                                    FilterProperty fp = ComparePropertyList.FirstOrDefault(p => p.PropertyName == enColumnName);
                                    if (fp != null)
                                    {
                                        ComparePropertyList.Remove(fp);
                                    }
                                    object value = GetDefaultForType(filterProperty.PropertyType);
                                    filterProperty.SetValue(Condition, value, null);
                                }
                            }

                            SetFilterComm(Condition);

                            int rowCount = this.ListCollectionView.Count;//最大行

                            if (rowCount <= 0)
                            {
                                this.ListCollectionView.Remove(this.Condition);

                                this.ListCollectionView.AddNewItem(this.Condition);

                                this.ListCollectionView.CommitNew();//未解决问题 输入= 就会提交事务失败
                                if (this.ListCollectionView.Count != 1)//应对提交事务失败时出现的BUG
                                {
                                    this.ItemsSource.Remove(this.Condition); 
                                }
                            }
                            else if (rowCount == 1)
                            {
                                if (this.Condition.Equals(this.ListCollectionView.GetItemAt(0)))
                                {
                                    this.ListCollectionView.Remove(this.Condition);
                                    this.ListCollectionView.AddNewItem(this.Condition);
                                    this.ListCollectionView.CommitNew();
                                }
                                else
                                {
                                    this.ListCollectionView.Remove(this.Condition);
                                    this.ListCollectionView.CommitNew();
                                }
                            }
                            else
                            {

                                this.ListCollectionView.Remove(this.Condition);
                                this.ListCollectionView.CommitNew();
                            }
                        }
                    }
                }
            } 
            catch (Exception ex)
            {

            }
            finally
            {
                Interlocked.Exchange(ref usingResource, 0);
            }
        }
        public static object GetDefaultForType(Type propertyType)
        {
            if (propertyType.Name == "String")
            {
                return string.Empty;
            }
            else
            {
                return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
            }
        }
        public static object ChangeValueForType(Type type, string text)
        {
            string typename = type.ToString().Replace("System.Nullable`1[", "").Replace("]", "").Replace("System.", "").ToLower();
            switch (typename.ToLower())
            {
                case "decimal":
                    return ToolFunction.ToDec(text);
                case "int":
                case "int32":
                case "int64":
                case "long":
                    return ToolFunction.ToInt(text);
                case "double":
                    return ToolFunction.ToDouble(text);
                case "float":
                    return ToolFunction.ToFloat(text);
                case "string":
                    return text.ToString();
                case "bool":
                case "boolean":
                    bool ret = false;
                    if (text.ToString() == "是")
                    {
                        ret = true;
                    }
                    if (text.ToString() == "1")
                    {
                        ret = true;
                    }
                    if (text.ToString().ToLower() == "true")
                    {
                        ret = true;
                    }
                    if (text.ToString() == "否")
                    {
                        ret = false;
                    }
                    if (text.ToString() == "0")
                    {
                        ret = false;
                    }
                    if (text.ToString().ToLower() == "false")
                    {
                        ret = false;
                    }
                    return ret;
                case "datetime":
                    return ToolFunction.ToDateTime(text);
                default:
                    return text;
            }
        }
        public void SetFilterComm(Object condition)
        {
            if (condition != null)
            {
                #region
                var dataGridData = this;



                #region

                ItemsSourceView.Filter = new Predicate<object>((obj) =>
                {
                    bool isFilter = true;

                    foreach (FilterProperty pinfo in ComparePropertyList) //循环筛选出来需要比较的属性
                    {

                        string columnNameEn = pinfo.PropertyName;
                        var filterValue = pinfo.PropertyValue;//过滤的值
                        string columnType = pinfo.PropertyInfo.PropertyType.ToString().Replace("System.Nullable`1[", "").Replace("]", "").Replace("System.", "").ToLower();


                        if (filterValue != null)
                        {
                            #region 重点内容 这里开始执行真正的比较

                            object rowValue = ToolFunction.GetPropertyValue(obj, pinfo.PropertyInfo);//数据行的值

                            if (rowValue == null)
                            {
                                if (filterValue.ToString() == "")
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                isFilter = CompareValue(columnType, rowValue, filterValue, pinfo.ConditionStr);
                            }
                            #endregion
                        }
                        if (!isFilter)
                        {
                            return isFilter;
                        }
                    }

                    return isFilter;
                });


                #endregion

                #endregion
            }
        }
        public bool CompareValue(string type, object rowValue, object filterValue, string conditionStr)
        {
            bool result = false;
            switch (type.ToLower())
            {
                case "decimal":
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "double":
                case "float":
                    var s_d = ToolFunction.ToDec(rowValue);
                    var t_d = ToolFunction.ToDec(filterValue);
                    switch (conditionStr)
                    {
                        case "=":
                            result = s_d == t_d;
                            break;
                        case ">":
                            result = s_d > t_d;
                            break;
                        case "<":
                            result = s_d < t_d;
                            break;
                        case ">=":
                            result = s_d >= t_d;
                            break;
                        case "<=":
                            result = s_d <= t_d;
                            break;
                        case "!=":
                            result = s_d != t_d;
                            break;
                    }
                    break;
                case "string":
                    var s_s = rowValue.ToString();
                    var t_s = filterValue.ToString();
                    if(string.IsNullOrWhiteSpace(conditionStr))
                    {
                        conditionStr = "like";
                    } 
                    switch (conditionStr)
                    {
                        case "like":
                            result = s_s.ToLower().Contains(t_s.ToLower());
                            break;
                        case "!like":
                            result = !s_s.ToLower().Contains(t_s.ToLower());
                            break;
                        case "=":
                            result = s_s.ToLower() == t_s.ToLower();
                            break;
                    }
                    break;
                case "bool":
                case "boolean":
                    bool s_b = false;
                    if (rowValue.ToString() == "是")
                    {
                        s_b = true;
                    }
                    if (rowValue.ToString() == "1")
                    {
                        s_b = true;
                    }
                    if (rowValue.ToString().ToLower() == "true")
                    {
                        s_b = true;
                    }
                    if (rowValue.ToString() == "否")
                    {
                        s_b = false;
                    }
                    if (rowValue.ToString() == "0")
                    {
                        s_b = false;
                    }
                    if (rowValue.ToString().ToLower() == "false")
                    {
                        s_b = false;
                    }
                    //==================================
                    var t_b = false;
                    if (filterValue.ToString() == "是")
                    {
                        t_b = true;
                    }
                    if (filterValue.ToString() == "1")
                    {
                        t_b = true;
                    }
                    if (filterValue.ToString().ToLower() == "true")
                    {
                        t_b = true;
                    }
                    if (filterValue.ToString() == "否")
                    {
                        t_b = false;
                    }
                    if (filterValue.ToString() == "0")
                    {
                        t_b = false;
                    }
                    if (filterValue.ToString().ToLower() == "false")
                    {
                        t_b = false;
                    }
                    switch (conditionStr)
                    {
                        case "!=":
                            result = s_b != t_b;
                            break;
                        case "=":
                            result = s_b == t_b;
                            break;
                    }
                    break;
                case "datetime":
                    var s_dt = ToolFunction.ToDateTime(rowValue);
                    var t_dt = ToolFunction.ToDateTime(filterValue);
                    switch (conditionStr)
                    {
                        case "=":
                            result = s_dt == t_dt;
                            break;
                        case ">":
                            result = s_dt > t_dt;
                            break;
                        case ">=":
                            result = s_dt >= t_dt;
                            break;
                        case "<=":
                            result = s_dt <= t_dt;
                            break;
                        case "<":
                            result = s_dt < t_dt;
                            break;
                        case "!=":
                            result = s_dt != t_dt;
                            break;
                    }
                    break;

            }
            return result;
        }

        public string GetValueAndCondition(object filterValue, out object actualfilterValue)
        {
            string conditionStr = "=";
            string filterValueStr = filterValue.ToString();
          
            if (filterValueStr.StartsWith(">"))
            {
                conditionStr = ">";
                filterValue = filterValueStr.Replace(">", "");
            }
            if (filterValueStr.StartsWith("<"))
            {
                conditionStr = "<";
                filterValue = filterValueStr.Replace("<", "");
            }
            if (filterValueStr.StartsWith("<="))
            {
                conditionStr = "<=";
                filterValue = filterValueStr.Replace("<=", "");
            }
            if (filterValueStr.StartsWith(">="))
            {
                conditionStr = ">=";
                filterValue = filterValueStr.Replace(">=", "");
            }
            if (filterValueStr.StartsWith("="))
            {
                conditionStr = "=";
                filterValue = filterValueStr.Replace("=", "");
            }
            else if (filterValueStr.StartsWith("!="))
            {
                conditionStr = "!=";
                filterValue = filterValueStr.Replace("!=", "");
            }
            if (filterValueStr.StartsWith("!like"))
            {
                conditionStr = "!like";
                filterValue = filterValueStr.Replace("!like", "");
            }
            else if (filterValueStr.StartsWith("like"))
            {
                conditionStr = "like";
                filterValue = filterValueStr.Replace("like", "");
            }
            actualfilterValue = filterValue;
            return conditionStr;
        }
         
        #endregion
        public void Refresh()
        {
            if (_ItemsSourceView == null)
            {
                _ItemsSourceView = CollectionViewSource.GetDefaultView(this.ItemsSource);
            }
            _ItemsSourceView.Refresh();
        }
        #endregion

        #region IDataGrid 
        ObservableCollection<object> IDataGrid.ItemsSource
        {
            get
            {
                ObservableCollection<Object> ret = new ObservableCollection<Object>();
                foreach (var item in this.ItemsSource)
                {
                    ret.Add(item);
                }
                return ret;
            }
        }

        object IDataGrid.SelectedItem
        {
            get
            {
                return SelectedItem;
            }

        }

        object IDataGrid.Condition
        {
            get
            {
                return Condition;
            }

        }


        public event Action<Object> ItemsSourceChange;
        public void ChangeItemsSource()
        {
            if (ExcuteItemsSourceChange != null)
            {
                ExcuteItemsSourceChange();
            }
        }
        public string DataGridName { get; set; }

        /// <summary>
        /// 英文名,类型,修改值
        /// </summary>
        public Action<Object, Object, Object> TextChange { get; set; }

        public Action<Object, Object, Object, Object, Object> TextLostFocus { get; set; }

        public Action<Object> AllCheck { get; set; }


        public List<Object> _DetailSource;
        public List<Object> DetailSource
        {
            get { return _DetailSource; }
            set
            {
                _DetailSource = value;
                OnPropertyChanged();
            }
        }

        public bool _IsBusyDetail;
        public bool IsBusyDetail
        {
            get { return _IsBusyDetail; }
            set
            {
                _IsBusyDetail = value;
                OnPropertyChanged();
            }
        }

        public string _DetailVisibility;
        public string DetailVisibility
        {
            get { return _DetailVisibility; }
            set
            {
                _DetailVisibility = value;
                OnPropertyChanged();
            }
        }


        public Action ExcuteItemsSourceChange { get; set; }
        #endregion
    }
}
