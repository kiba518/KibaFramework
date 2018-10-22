 
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; 
using System.Windows.Input;
using System.Windows.Media; 
using System.Windows.Shapes;
using System.Windows.Threading;
using Utility;
using ViewModel;
 

namespace KibaFramework
{
      
    public partial class KDataGrid : UserControl , INotifyPropertyChanged
    {
         
        public ProxyHelper proxyHelper = new ProxyHelper();
         
        public volatile int isLoad = 0;
        
        public string DataGridName { get; set; }
       
        public event PropertyChangedEventHandler PropertyChanged;

        public IDataGrid IODataGridSource { get; set; } 
      
        public ObservableCollection<Object> ItemsSource{ get; set; }
         
        public ListCollectionView ListCollectionView { get; set; }

        public ICollectionView ItemsSourceView { get; set; } 
        
        public Object Condition { get; set; }
        
        public DataGridConfig DataGridConfig { get; set; }

        public List<PropertyInfo> TypePropertylist = new List<PropertyInfo>(); 

        public List<ColumnConfig> OrginalIndex = new List<ColumnConfig>();

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public KDataGrid()
        { 
            InitializeComponent();
            this.DataContextChanged += OliveDataGrid_DataContextChanged;  
             
        }

        private void SaveColumn(object sender, EventArgs e)
        {
            var columns = this.dgCon.Columns;
            for (int i = 0; i < columns.Count; i++)
            {
                var columnNameEN = columns[i].SortMemberPath;
                var ColumnConfig = DataGridConfig.ColumnConfig.FirstOrDefault(p => p.ColumnNameEN == columnNameEN);
                if (ColumnConfig != null)
                {
                    ColumnConfig.Index = columns[i].DisplayIndex;
                    ColumnConfig.Width = Convert.ToInt32(columns[i].ActualWidth);
                }
            }

            Task save = new Task(new Action(() =>
            {
                //调用服务保存列设置
                try
                {
                    proxyHelper.Save(DataGridConfig);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            }));
            save.Start();

        }
        public DataGridConfig GetControlConfig()
        {
            DataGridConfig dataGridConfig = ViewModel.Static.StaticData.DataGridConfig.FirstOrDefault(p => p.DataGridTemplateName == DataGridName);
            if (dataGridConfig != null)
            {
                List<ColumnConfig> columnList = dataGridConfig.ColumnConfig;
                columnList = columnList.OrderBy(p => p.Index).ToList();
                dataGridConfig.ColumnConfig = SetIndex(columnList); 
            } 
            return dataGridConfig;
        }
        public List<ColumnConfig> SetIndex(List<ColumnConfig> columnList)
        {
            for (int i = 0; i < columnList.Count; i++)
            {
                columnList[i].Index = i; 
            }
            return columnList;
        }

        #region 基础事件
       
        void OliveDataGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs dpce)
        {
            if (this.DataContext != null && this.DataContext is IDataGrid)
            {
                var orgidg = this.DataContext as IDataGrid; 
                if (isLoad == 0 || DataGridName != orgidg.DataGridName)
                {
                    isLoad = 1;
                    DataGridName = orgidg.DataGridName;
                   
                    DataGridConfig = GetControlConfig();
                    dgCon.ColumnReordered -= SaveColumn;
                    dgCon.ColumnReordered += SaveColumn;
                    dgCon.Unloaded -= SaveColumn;
                    dgCon.Unloaded += SaveColumn;
                    if (!DataGridConfig.HasPaging)
                    {
                        spPaging.Visibility = Visibility.Collapsed;
                    }
                    if (DataGridConfig.HideControlRow)
                    {
                        spHideControlRow.Visibility = Visibility.Collapsed;
                    } 
                }
                orgidg.ItemsSourceChange -= ItemsSourceChange;
                orgidg.ItemsSourceChange += ItemsSourceChange;
                ItemsSourceChange(this.DataContext); 
                orgidg.ExcuteItemsSourceChange = () => {
                    DispatcherHelper.DoMenthodByDispatcher(new Action(() =>
                    {
                        var newOrgidg = this.DataContext as IDataGrid;
                        if (DataGridName != newOrgidg.DataGridName)
                        {
                            DataGridName = newOrgidg.DataGridName;
                           
                            DataGridConfig = GetControlConfig();
                            dgCon.ColumnReordered -= SaveColumn;
                            dgCon.ColumnReordered += SaveColumn;
                            dgCon.Unloaded -= SaveColumn;
                            dgCon.Unloaded += SaveColumn;
                            if (!DataGridConfig.HasPaging)
                            {
                                spPaging.Visibility = Visibility.Collapsed;
                            }
                            if (DataGridConfig.HideControlRow)
                            {
                                spHideControlRow.Visibility = Visibility.Collapsed;
                            }
                        }
                        dgCon.Columns.Clear();
                        ItemsSourceChange(IODataGridSource); 

                    }));

                };
              
            }
        }
         
        public void ItemsSourceChange(Object source)
        {
            try
            {
                DispatcherHelper.DoMenthodByDispatcher(new Action(() =>
                {
                    try
                    {
                        #region

                        IODataGridSource = source as IDataGrid;

                        this.ItemsSource = IODataGridSource.ItemsSource;
                        this.ItemsSourceView = CollectionViewSource.GetDefaultView(ItemsSource);
                        this.ListCollectionView = this.ItemsSourceView as ListCollectionView;

                        
                        this.Condition = IODataGridSource.Condition;

                        if (dgCon.Columns != null && dgCon.Columns.Count > 0)
                        {

                        }
                        else
                        { 

                            #region 刷新不执行创建列

                            TypePropertylist = Condition.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).ToList();
                            OrginalIndex.Clear();
                            int showIndex = 0;


                            Binding FontForeGroundbinding = new Binding(); 
                            FontForeGroundbinding.Mode = BindingMode.TwoWay;
                            FontForeGroundbinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            FontForeGroundbinding.RelativeSource = RelativeSource.Self;
                            FontForeGroundbinding.Path = new PropertyPath("DataContext");
                            if (!string.IsNullOrWhiteSpace(DataGridConfig.RowForegroundConvert))
                            {
                                try
                                {
                                    Assembly assembly = Assembly.GetEntryAssembly();
                                    Type fgtype = assembly.GetType("KibaFramework." + DataGridConfig.RowForegroundConvert.Trim(), true, false);
                                    if (fgtype != null)
                                    {
                                        FontForeGroundbinding.Converter = (IValueConverter)Activator.CreateInstance(fgtype);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                            }
                            #region 序号
                            if (DataGridConfig.FrozenColumnCount > 0)
                            {
                                dgCon.FrozenColumnCount = DataGridConfig.FrozenColumnCount;
                            }
                            if (DataGridConfig.HasNo)
                            { 
                                DataGridTemplateColumn dgcolumnNo = new DataGridTemplateColumn();
                                dgcolumnNo.Header = "序号";
                                dgcolumnNo.Width = 35;
                                dgcolumnNo.Visibility = Visibility.Visible;
                                dgcolumnNo.SortMemberPath = "Header";

                                Binding bindingNo = new Binding("Header");
                                bindingNo.Mode = BindingMode.OneWay;
                                var rs = new RelativeSource();
                                rs.AncestorType = typeof(DataGridRow);
                                rs.Mode = RelativeSourceMode.FindAncestor;
                                bindingNo.RelativeSource = rs;

                                FrameworkElementFactory fefno = new FrameworkElementFactory(typeof(TextBox));
                                fefno.SetBinding(TextBox.TextProperty, bindingNo);
                                fefno.SetValue(TextBox.NameProperty, "no");
                                fefno.SetValue(TextBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                                fefno.SetValue(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                                fefno.SetValue(TextBox.TextAlignmentProperty, TextAlignment.Center);
                                fefno.SetValue(TextBox.CursorProperty, Cursors.Hand);
                                fefno.SetValue(TextBox.IsReadOnlyProperty, true);
                                fefno.SetValue(TextBox.BorderThicknessProperty, new Thickness(0));
                                fefno.SetValue(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
                                fefno.SetValue(TextBox.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
                                fefno.SetValue(TextBox.MarginProperty, new Thickness(0, 0, -10, 0));
                                if (!string.IsNullOrWhiteSpace(DataGridConfig.RowForegroundConvert))
                                {
                                    fefno.SetBinding(TextBox.ForegroundProperty, FontForeGroundbinding);
                                }

                                ControlTemplate ctno = new ControlTemplate(typeof(DataGridCell));
                                ctno.VisualTree = fefno;
                                Style styleRightno = new Style(typeof(DataGridCell));
                                Setter setRightno = new Setter(DataGridCell.TemplateProperty, ctno);
                                styleRightno.Setters.Add(setRightno);
                                dgcolumnNo.CellStyle = styleRightno;
                                dgCon.Columns.Add(dgcolumnNo);

                                var orgcc = new ColumnConfig();
                                orgcc.ColumnNameCN = "序号";
                                orgcc.Index = showIndex;
                                OrginalIndex.Add(orgcc);
                                showIndex++;
                            }
                            #endregion

                            foreach (var cinfo in DataGridConfig.ColumnConfig)
                            { 
                                if (cinfo.IsVisibility == false)
                                {
                                    continue;
                                }
                                var orgcc = cinfo.MapTo<ColumnConfig>();
                                orgcc.Index = showIndex;
                                OrginalIndex.Add(orgcc);   
                                showIndex++;
                                #region 绑定内容
                                Binding binding = new Binding(cinfo.ColumnNameEN);
                                binding.Mode = BindingMode.TwoWay;
                                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                if (!string.IsNullOrWhiteSpace(cinfo.ViewConvert))
                                {
                                    try
                                    {
                                        Assembly assembly = Assembly.GetEntryAssembly();
                                        Type type = assembly.GetType("KibaFramework." + cinfo.ViewConvert.Trim(), true, false);
                                        if (type != null)
                                        {
                                            binding.Converter = (IValueConverter)Activator.CreateInstance(type);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                }
                                else//没有强制转换 执行数字Convert
                                {
                                    PropertyInfo filterProperty = TypePropertylist.FirstOrDefault(fp => fp.Name == cinfo.ColumnNameEN);
                                    if (filterProperty == null)
                                    {
                                        continue;
                                    }
                                    string typename = filterProperty.PropertyType.ToString().Replace("System.Nullable`1[", "").Replace("]", "").Replace("System.", "").ToLower();
                                    if (typename == "decimal" || typename == "int" || typename == "int32" || typename == "int64" || typename == "long" || typename == "double" || typename == "float")
                                    {
                                        binding.Converter = new NumberViewConvert();
                                    }
                                }
                                #endregion
                                DataGridTemplateColumn dgcolumn = new DataGridTemplateColumn();
                                dgcolumn.Header = cinfo.ColumnNameCN;
                                dgcolumn.Width = cinfo.Width;
                                dgcolumn.Visibility = Visibility.Visible;
                                dgcolumn.SortMemberPath = cinfo.ColumnNameEN;
                                
                                FrameworkElementFactory fefParent = new FrameworkElementFactory(typeof(DockPanel));
                                fefParent.SetValue(DockPanel.MarginProperty, new Thickness(2));
                                #region ===================列设置开始===================
                                #region CheckBox
                                if (cinfo.ColumnControlType == ColumnControlType.CheckBox.GetHashCode())
                                {
                                    FrameworkElementFactory fef = new FrameworkElementFactory(typeof(CheckBox));
                                    fef.SetBinding(CheckBox.IsCheckedProperty, binding);
                                    fef.SetValue(CheckBox.NameProperty, "cbCheck");
                                    fef.SetValue(CheckBox.TagProperty, cinfo.ColumnNameEN);
                                    fef.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(this.CellCheck_Checked));
                                    fef.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                                    fef.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                                    fefParent.AppendChild(fef);
                                    if (cinfo.HasAllCheck)
                                    {
                                        if (DataGridConfig.HasFilter)
                                        {
                                            dgcolumn.HeaderStyle = (Style)this.Resources["dgcbAll"];
                                        }
                                        else
                                        {
                                            dgcolumn.HeaderStyle = (Style)this.Resources["dgcbAllNoFilter"];
                                        }
                                    }
                                    else
                                    {
                                        dgcolumn.HeaderStyle = (Style)this.Resources["dgNormal"];
                                    }
                                }
                                #endregion

                                #region Image
                                else if (cinfo.ColumnControlType == ColumnControlType.Image.GetHashCode())
                                {
                                    FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Image));
                                    fef.SetBinding(Image.SourceProperty, binding);
                                    fef.SetValue(Image.VerticalAlignmentProperty, VerticalAlignment.Center);
                                    fefParent.AppendChild(fef);
                                    dgcolumn.HeaderStyle = (Style)this.Resources["dgNormal"];
                                }
                                #endregion

                                #region TextBox
                                else
                                {
                                    
                                    #region 前置图片
                                    if (cinfo.ColumnPreImage != null && cinfo.ColumnPreImage.Count() > 0)
                                    {
                                        foreach (var preimage in cinfo.ColumnPreImage)
                                        {
                                            try
                                            {
                                                if (!string.IsNullOrWhiteSpace(preimage.Convert))
                                                {
                                                    FrameworkElementFactory fefimg = new FrameworkElementFactory(typeof(Image));

                                                    Binding preimgbinding = new Binding();
                                                    preimgbinding.Mode = BindingMode.TwoWay;
                                                    preimgbinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                                  
                                                    if (!string.IsNullOrWhiteSpace(preimage.ConvertProperty))
                                                    { 
                                                        preimgbinding.RelativeSource = RelativeSource.Self;

                                                        preimgbinding.Path = new PropertyPath("DataContext." + preimage.ConvertProperty); 
                                                    }
                                                    else
                                                    {
                                                        preimgbinding.RelativeSource = RelativeSource.Self;
                                                        preimgbinding.Path = new PropertyPath("DataContext"); 
                                                    }
                                                 
                                                    Assembly assembly = Assembly.GetEntryAssembly();
                                                    Type attachType = assembly.GetType("KibaFramework." + preimage.Convert, true, false);
                                                    preimgbinding.Converter = (IValueConverter)Activator.CreateInstance(attachType);
                                                    fefimg.SetBinding(Image.SourceProperty, preimgbinding);

                                                    if (!string.IsNullOrWhiteSpace(preimage.VisibilityConvert))
                                                    {
                                                        Binding preimgVisibilityBinding = new Binding();
                                                        preimgVisibilityBinding.Mode = BindingMode.TwoWay;
                                                        preimgVisibilityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                                       
                                                        if (!string.IsNullOrWhiteSpace(preimage.VisibilityConvertProperty))
                                                        {
                                                            preimgVisibilityBinding.RelativeSource = RelativeSource.Self;

                                                            preimgVisibilityBinding.Path = new PropertyPath("DataContext." + preimage.VisibilityConvertProperty);
                                                        }
                                                        else
                                                        {
                                                            preimgVisibilityBinding.RelativeSource = RelativeSource.Self;
                                                            preimgVisibilityBinding.Path = new PropertyPath("DataContext");
                                                        } 
                                                        Type preimgVisibilityBindingType = assembly.GetType("KibaFramework." + preimage.VisibilityConvert, true, false);
                                                        preimgVisibilityBinding.Converter = (IValueConverter)Activator.CreateInstance(preimgVisibilityBindingType);
                                                        fefimg.SetBinding(Image.VisibilityProperty, preimgVisibilityBinding);
                                                    }

                                                    fefimg.SetValue(Image.VerticalAlignmentProperty, VerticalAlignment.Center);
                                                    fefimg.SetValue(DockPanel.DockProperty, Dock.Left);
                                                    fefimg.SetValue(Image.WidthProperty, 15.00);
                                                    fefimg.SetValue(Image.HeightProperty, 15.00);
                                                    fefimg.SetValue(Image.MarginProperty, new Thickness(3, -1, -5, 0));
                                                    fefimg.SetValue(Image.CursorProperty, Cursors.Hand);
                                                    fefParent.AppendChild(fefimg);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Logger.Error(ex);
                                                continue;
                                            }
                                        }
                                    }
                                    #endregion

                                    FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
                                    stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                                    stackPanel.SetValue(StackPanel.VerticalAlignmentProperty, VerticalAlignment.Center);

                                    FrameworkElementFactory prePanel = new FrameworkElementFactory(typeof(DockPanel)); 
                                    prePanel.SetValue(DockPanel.VerticalAlignmentProperty, VerticalAlignment.Center);

                                    #region 前置文字
                                    if (!string.IsNullOrWhiteSpace(cinfo.Pretext))
                                    { 
                                        FrameworkElementFactory prefef = new FrameworkElementFactory(typeof(TextBox));
                                        prefef.SetValue(TextBox.TextProperty, cinfo.Pretext);
                                        prefef.SetValue(TextBox.TagProperty, cinfo.ColumnNameEN);
                                        prefef.SetValue(TextBox.NameProperty, "Pretext"); 
                                        prefef.SetValue(DockPanel.DockProperty, Dock.Left);
                                        prefef.SetValue(TextBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                                        prefef.SetValue(TextBox.CursorProperty, Cursors.Hand);
                                        prefef.SetValue(TextBox.IsReadOnlyProperty, true);
                                        prefef.SetValue(TextBox.BorderThicknessProperty, new Thickness(0));
                                        prefef.SetValue(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
                                        prefef.SetValue(TextBox.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
                                        prefef.SetValue(TextBox.MarginProperty, new Thickness(0, 0, -10, 0));
                                        if (!string.IsNullOrWhiteSpace(cinfo.ForegroundConvert))
                                        {
                                            Binding tempbinding = new Binding(); 
                                            tempbinding.Mode = BindingMode.TwoWay;
                                            tempbinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                            tempbinding.RelativeSource = RelativeSource.Self;
                                            tempbinding.Path = new PropertyPath("DataContext");
                                            try
                                            { 
                                                Assembly assembly = Assembly.GetEntryAssembly();
                                                Type fgtype = assembly.GetType("KibaFramework." + cinfo.ForegroundConvert.Trim(), true, false);
                                                if (fgtype != null)
                                                {
                                                    tempbinding.Converter = (IValueConverter)Activator.CreateInstance(fgtype);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Logger.Error(ex);
                                            }
                                            prefef.SetBinding(TextBox.ForegroundProperty, tempbinding);
                                        }
                                        else if (!string.IsNullOrWhiteSpace(DataGridConfig.RowForegroundConvert))
                                        {  
                                            prefef.SetBinding(TextBox.ForegroundProperty, FontForeGroundbinding);
                                        }
                                        prePanel.AppendChild(prefef);
                                    }
                                    #endregion

                                    #region Text文本
                                    FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBox));
                                    fef.SetBinding(TextBox.TextProperty, binding);
                                    fef.SetValue(TextBox.TagProperty, cinfo.ColumnNameEN);
                                    fef.SetValue(TextBox.NameProperty, "txtEdit");
                                    fef.SetValue(TextBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                                    fef.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(this.CellEdit_GotFocus));
                                    if (!string.IsNullOrWhiteSpace(cinfo.ForegroundConvert))
                                    {
                                        Binding tempbinding = new Binding();
                                        tempbinding.Mode = BindingMode.TwoWay;
                                        tempbinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                        tempbinding.RelativeSource = RelativeSource.Self;
                                        tempbinding.Path = new PropertyPath("DataContext");
                                        try
                                        {
                                            Assembly assembly = Assembly.GetEntryAssembly();
                                            Type fgtype = assembly.GetType("KibaFramework." + cinfo.ForegroundConvert.Trim(), true, false);
                                            if (fgtype != null)
                                            {
                                                tempbinding.Converter = (IValueConverter)Activator.CreateInstance(fgtype);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error(ex);
                                        }
                                        fef.SetBinding(TextBox.ForegroundProperty, tempbinding);
                                    }
                                    else if (!string.IsNullOrWhiteSpace(DataGridConfig.RowForegroundConvert))
                                    {
                                        fef.SetBinding(TextBox.ForegroundProperty, FontForeGroundbinding); 
                                    }
                                    if (cinfo.HasEnter)
                                    {
                                        fef.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(this.CellTextBox_KeyDown));
                                    } 
                                    if (cinfo.ColumnControlType == ColumnControlType.TextBox.GetHashCode())
                                    {
                                        fef.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(this.CellEdit_TextChanged));
                                        if (cinfo.HasEnter)
                                        {
                                            fef.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(this.CellTextBox_KeyDown));
                                        }
                                    }
                                    else
                                    {
                                        fef.SetValue(TextBox.CursorProperty, Cursors.Hand);
                                        fef.SetValue(TextBox.IsReadOnlyProperty, true);
                                        fef.SetValue(TextBox.BorderThicknessProperty, new Thickness(0));
                                        fef.SetValue(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
                                        fef.SetValue(TextBox.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
                                    }
                                    #endregion
                                    prePanel.AppendChild(fef);
                                    stackPanel.AppendChild(prePanel);
                                     
                                    #region 双显
                                    if (!string.IsNullOrWhiteSpace(cinfo.DoubleViewColumnNameEN))
                                    {
                                        try
                                        {
                                            
                                            #region 双显下划线
                                            FrameworkElementFactory line = new FrameworkElementFactory(typeof(Line));
                                            line.SetValue(Line.StrokeThicknessProperty, 1.00);
                                            line.SetValue(Line.StrokeProperty, new SolidColorBrush(Colors.Black));
                                            line.SetValue(Line.X1Property, 0.00);
                                            line.SetValue(Line.X2Property, 500.00);
                                            line.SetValue(Line.Y1Property, 0.00);
                                            line.SetValue(Line.Y2Property, 0.00);
                                          
                                            if (!string.IsNullOrWhiteSpace(cinfo.DoubleIsVisibility))
                                            { 
                                                Binding DoubleIsVisibilitybinding = new Binding();
                                                DoubleIsVisibilitybinding.Mode = BindingMode.TwoWay;
                                                DoubleIsVisibilitybinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                                DoubleIsVisibilitybinding.RelativeSource = RelativeSource.Self;
                                                DoubleIsVisibilitybinding.Path = new PropertyPath("DataContext");
                                                try
                                                {
                                                    Assembly assembly = Assembly.GetEntryAssembly();
                                                    Type bvtype = assembly.GetType("KibaFramework." + cinfo.DoubleIsVisibility.Trim(), true, false);
                                                    if (bvtype != null)
                                                    {
                                                        DoubleIsVisibilitybinding.Converter = (IValueConverter)Activator.CreateInstance(bvtype);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Error(ex);
                                                }
                                                line.SetBinding(Line.VisibilityProperty, DoubleIsVisibilitybinding); 
                                            }
                                            stackPanel.AppendChild(line);
                                            #endregion 
                                            FrameworkElementFactory preDoublePanel = new FrameworkElementFactory(typeof(DockPanel));
                                            preDoublePanel.SetValue(DockPanel.VerticalAlignmentProperty, VerticalAlignment.Center);
                                            if (!string.IsNullOrWhiteSpace(cinfo.DoubleIsVisibility))
                                            {
                                                Binding DoubleIsVisibilitybinding = new Binding();
                                                DoubleIsVisibilitybinding.Mode = BindingMode.TwoWay;
                                                DoubleIsVisibilitybinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                                DoubleIsVisibilitybinding.RelativeSource = RelativeSource.Self;
                                                DoubleIsVisibilitybinding.Path = new PropertyPath("DataContext");
                                                try
                                                {
                                                    Assembly assembly = Assembly.GetEntryAssembly();
                                                    Type bvtype = assembly.GetType("KibaFramework." + cinfo.DoubleIsVisibility.Trim(), true, false);
                                                    if (bvtype != null)
                                                    {
                                                        DoubleIsVisibilitybinding.Converter = (IValueConverter)Activator.CreateInstance(bvtype);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Error(ex);
                                                }
                                                preDoublePanel.SetBinding(Line.VisibilityProperty, DoubleIsVisibilitybinding);
                                            }
                                            #region 双显前置文字 
                                            if (!string.IsNullOrWhiteSpace(cinfo.DoublePretext))
                                            {
                                                FrameworkElementFactory predfef = new FrameworkElementFactory(typeof(TextBox));
                                                predfef.SetValue(TextBox.TextProperty, cinfo.DoublePretext);
                                                predfef.SetValue(DockPanel.DockProperty, Dock.Left);
                                                predfef.SetValue(TextBox.TagProperty, cinfo.ColumnNameEN);
                                                predfef.SetValue(TextBox.NameProperty, "Pretext");
                                                predfef.SetValue(TextBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                                                predfef.SetValue(TextBox.CursorProperty, Cursors.Hand);
                                                predfef.SetValue(TextBox.IsReadOnlyProperty, true);
                                                predfef.SetValue(TextBox.BorderThicknessProperty, new Thickness(0));
                                                predfef.SetValue(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
                                                predfef.SetValue(TextBox.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
                                                predfef.SetValue(TextBox.MarginProperty, new Thickness(0, 0, -10, 0));
                                                if (!string.IsNullOrWhiteSpace(cinfo.ForegroundConvert))
                                                {
                                                    Binding tempbinding = new Binding();
                                                    tempbinding.Mode = BindingMode.TwoWay;
                                                    tempbinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                                    tempbinding.RelativeSource = RelativeSource.Self;
                                                    tempbinding.Path = new PropertyPath("DataContext");
                                                    try
                                                    {
                                                        Assembly assembly = Assembly.GetEntryAssembly();
                                                        Type fgtype = assembly.GetType("KibaFramework." + cinfo.ForegroundConvert.Trim(), true, false);
                                                        if (fgtype != null)
                                                        {
                                                            tempbinding.Converter = (IValueConverter)Activator.CreateInstance(fgtype);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Logger.Error(ex);
                                                    }
                                                    predfef.SetBinding(TextBox.ForegroundProperty, tempbinding);
                                                }
                                                else if (!string.IsNullOrWhiteSpace(DataGridConfig.RowForegroundConvert))
                                                {
                                                    predfef.SetBinding(TextBox.ForegroundProperty, FontForeGroundbinding); 
                                                }
                                                preDoublePanel.AppendChild(predfef); 
                                            }
                                            #endregion

                                            #region 双显Text文本
                                            Binding doubleBinding = new Binding(cinfo.DoubleViewColumnNameEN);
                                            doubleBinding.Mode = BindingMode.TwoWay;
                                            doubleBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                            FrameworkElementFactory fefDoubleView = new FrameworkElementFactory(typeof(TextBox));
                                            fefDoubleView.SetBinding(TextBox.TextProperty, doubleBinding);
                                            fefDoubleView.SetValue(TextBox.TagProperty, cinfo.DoubleViewColumnNameEN);
                                            fefDoubleView.SetValue(TextBox.NameProperty, "txtEdit");
                                            fefDoubleView.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(this.CellEdit_GotFocus));
                                            if (!string.IsNullOrWhiteSpace(cinfo.ForegroundConvert))
                                            {
                                                Binding tempbinding = new Binding();
                                                tempbinding.Mode = BindingMode.TwoWay;
                                                tempbinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                                tempbinding.RelativeSource = RelativeSource.Self;
                                                tempbinding.Path = new PropertyPath("DataContext");
                                                try
                                                {
                                                    Assembly assembly = Assembly.GetEntryAssembly();
                                                    Type fgtype = assembly.GetType("KibaFramework." + cinfo.ForegroundConvert.Trim(), true, false);
                                                    if (fgtype != null)
                                                    {
                                                        tempbinding.Converter = (IValueConverter)Activator.CreateInstance(fgtype);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Error(ex);
                                                }
                                                fefDoubleView.SetBinding(TextBox.ForegroundProperty, tempbinding);
                                            }
                                            else if (!string.IsNullOrWhiteSpace(DataGridConfig.RowForegroundConvert))
                                            {
                                                fefDoubleView.SetBinding(TextBox.ForegroundProperty, FontForeGroundbinding); 
                                            }
                                            if (cinfo.HasEnter)
                                            {
                                                fefDoubleView.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(this.CellTextBox_KeyDown));
                                            }

                                            if (cinfo.ColumnControlType == ColumnControlType.TextBox.GetHashCode())
                                            {
                                                fefDoubleView.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(this.CellEdit_TextChanged));
                                                if (cinfo.HasEnter)
                                                {
                                                    fefDoubleView.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(this.CellTextBox_KeyDown));
                                                }
                                            }
                                            else
                                            {
                                                fefDoubleView.SetValue(TextBox.CursorProperty, Cursors.Hand);
                                                fefDoubleView.SetValue(TextBox.IsReadOnlyProperty, true);
                                                fefDoubleView.SetValue(TextBox.BorderThicknessProperty, new Thickness(0));
                                                fefDoubleView.SetValue(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
                                                fefDoubleView.SetValue(TextBox.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
                                            } 
                                            preDoublePanel.AppendChild(fefDoubleView);
                                            #endregion
                                            stackPanel.AppendChild(preDoublePanel);
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error(ex);
                                        }
                                    }
                                    #endregion

                                    fefParent.AppendChild(stackPanel);

                                    if (DataGridConfig.HasFilter)
                                    {
                                        dgcolumn.HeaderStyle = (Style)this.Resources["dgchFilter"];
                                    }
                                    else
                                    {
                                        dgcolumn.HeaderStyle = (Style)this.Resources["dgNormal"];
                                    }
                                }
                                #endregion
                                #endregion ===================列设置结束===================
                                ControlTemplate ct = new ControlTemplate(typeof(DataGridCell));
                                ct.VisualTree = fefParent;
                                Style styleRight = new Style(typeof(DataGridCell));
                                Setter setRight = new Setter(DataGridCell.TemplateProperty, ct);
                                styleRight.Setters.Add(setRight);
                                dgcolumn.CellStyle = styleRight;

                                dgCon.Columns.Add(dgcolumn);
                            }
                            #endregion
                        }

                        dgCon.ItemsSource = this.ItemsSource;
                        dgCon.SelectedItem = this.IODataGridSource.SelectedItem;
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
         
        private void AllCheck_Click(object sender, RoutedEventArgs e)
        {
            var cbox = sender as CheckBox;
            if (cbox.Tag != null)
            {
                string cnColumnName = cbox.Tag.ToString();//属性名  是中文名
                var currentColumnConfig = OrginalIndex.FirstOrDefault(p => p.ColumnNameCN == cnColumnName);
                if (currentColumnConfig != null)
                {
                    int row = dgCon.Items.IndexOf(dgCon.CurrentItem);
                    string enColumnName = currentColumnConfig.ColumnNameEN; //找到对应的英文名 
                    PropertyInfo checkProperty = TypePropertylist.FirstOrDefault(fp => fp.Name == enColumnName);
                    if (checkProperty != null)
                    {
                        foreach (var item in this.ItemsSource)
                        {
                            if (this.ListCollectionView.Contains(item))//显示的行 //这里简单处理，默认绑定的选择属性是Bool
                            {
                                checkProperty.SetValue(item, cbox.IsChecked, null);
                            }
                            else//不显示的行
                            {
                                checkProperty.SetValue(item, false, null);
                            }
                        }
                        int count = this.ListCollectionView.Count;
                      
                        MoveRow(row);
                        if (IODataGridSource.AllCheck != null)
                        {
                            this.IODataGridSource.AllCheck(cbox.IsChecked);
                        }
                    }
                }
            }
        }
        private void CellCheck_Checked(object sender, RoutedEventArgs e)
        {
            

        }

        public Object OrgRowDataContext;

        private void CellEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            txt.SelectAll();
            if (txt.IsFocused && !txt.IsReadOnly)
            {
                OrgRowDataContext = txt.DataContext;
                var dcType = txt.DataContext.GetType();
                OrgRowDataContext = Activator.CreateInstance(dcType);
                List<PropertyInfo> pinfolist = dcType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).ToList();
                foreach (var item in pinfolist)
                {
                    item.SetValue(OrgRowDataContext, item.GetValue(txt.DataContext, null), null);
                } 
               
                txt.LostFocus -= CellEdit_LostFocus;
                txt.LostFocus += CellEdit_LostFocus;
            }
        }
        private void CellEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null && txt.Tag != null && txt.DataContext != null)
            {
                string columnNameEN = txt.Tag.ToString();
                var conditionType = Condition.GetType();
                PropertyInfo modifyProperty = TypePropertylist.FirstOrDefault(fp => fp.Name == columnNameEN);

                if (modifyProperty != null)
                {
                    var dataContext = txt.DataContext;

                    var orgValue = modifyProperty.GetValue(OrgRowDataContext, null);
                    object mobj = ChangeValueForType(modifyProperty.PropertyType, txt.Text);
                    if ((orgValue == null && mobj != null) || (mobj.ToString() != orgValue.ToString()))
                    { 
                        if (IODataGridSource.TextLostFocus != null)
                        {
                            this.IODataGridSource.TextLostFocus(columnNameEN, conditionType, mobj, orgValue, OrgRowDataContext);

                        }  
                    }
                }
            }

        }
        private void CellTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var txt = sender as TextBox; 
            if (!txt.IsFocused)
            {
                return;
            }
            if (e.Key == Key.Enter || e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            {
                int typeValue = ColumnControlType.TextBox.GetHashCode();
             
                int rowCount = this.ListCollectionView.Count;//最大行
                int row = dgCon.Items.IndexOf(dgCon.CurrentItem);
                var columnNameEN = dgCon.CurrentColumn.SortMemberPath;

                var currentColumnConfig = OrginalIndex.FirstOrDefault(p => p.ColumnNameEN == columnNameEN);

                if (currentColumnConfig != null)
                {
                    int currentIndex = currentColumnConfig.Index;// 当前列Index
                    int maxIndex = OrginalIndex.Where(p => p.HasEnter && p.ColumnControlType == typeValue).OrderByDescending(p => p.Index).First().Index;//最大列Index
                    int nextColIndex = 0;
                    int nextRowIndex = 0;

                    if (e.Key == Key.Enter)
                    {
                        e.Handled = true;//让全选SelectAll好使
                        if (maxIndex == currentIndex)//下移 最后一列
                        {
                            if (row + 1 >= rowCount)//最后一行 不动
                            {
                                nextColIndex = currentIndex;
                                nextRowIndex = row;
                            }
                            else
                            {

                                nextColIndex = OrginalIndex.Where(p => p.HasEnter && p.ColumnControlType == typeValue).OrderBy(p => p.Index).First().Index;
                                nextRowIndex = row + 1;
                                MoveRow(nextRowIndex);
                            }
                        }
                        else//横移
                        {
                            nextColIndex = OrginalIndex.Where(p => p.Index > currentIndex && p.HasEnter && p.ColumnControlType == typeValue).OrderBy(p => p.Index).First().Index;
                            nextRowIndex = row;
                        }
                    }
                    else if (e.Key == Key.Up)
                    {
                        e.Handled = true;//让全选SelectAll好使
                        nextColIndex = currentIndex;
                        if (row - 1 > -1)//最上一行 不动
                        {
                            nextRowIndex = row - 1;
                        }
                        else
                        {
                            nextRowIndex = 0;
                        }
                        MoveRow(nextRowIndex);
                    }
                    else if (e.Key == Key.Down)
                    {
                        e.Handled = true;//让全选SelectAll好使
                        nextColIndex = currentIndex;
                        if (row + 1 >= rowCount)//最后一行 不动
                        {
                            nextRowIndex = row;
                        }
                        else
                        {
                            nextRowIndex = row + 1;
                        }
                        MoveRow(nextRowIndex);
                    }
                    else if (e.Key == Key.Right)
                    {
                        e.Handled = true;//让全选SelectAll好使
                        nextRowIndex = row;
                        if (maxIndex == currentIndex)
                        {
                            nextColIndex = currentIndex;
                        }
                        else
                        {
                            nextColIndex = OrginalIndex.Where(p => p.Index > currentIndex && p.HasEnter && p.ColumnControlType == typeValue).OrderBy(p => p.Index).First().Index;
                        }
                    }
                    else if (e.Key == Key.Left)
                    {
                        e.Handled = true;//让全选SelectAll好使
                        nextRowIndex = row;

                        var previous = OrginalIndex.Where(p => p.Index < currentIndex && p.HasEnter && p.ColumnControlType == typeValue).OrderByDescending(p => p.Index).FirstOrDefault();

                        if (previous == null)
                        {
                            nextColIndex = currentIndex;
                        }
                        else
                        {
                            nextColIndex = previous.Index;
                        }
                    }

                    dgCon.ScrollIntoView(dgCon.SelectedItem, dgCon.Columns[nextColIndex]);
                    string nextColTextBoxName = "txtEdit";

                    var cell = DataGridHelper.GetCell(dgCon, nextRowIndex, nextColIndex);
                    var txtEdit = DataGridHelper.FindVisualChildByName<TextBox>(cell, nextColTextBoxName);

                    if (txtEdit != null)
                    {
                        TextBox txtTemp = txtEdit as TextBox;
                        txtTemp.Focus();
                    }
                    else
                    {
                        Logger.Info("没找到控件");
                    }
                }
            }
        }

        public void MoveRow(int rowIndex)
        {
            if(rowIndex<0)
            {
                rowIndex = 0;
            }
            DataGridRow row = (DataGridRow)dgCon.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (row != null)
            {
                FocusNavigationDirection focusDirection = FocusNavigationDirection.First;   //可以是其他方向键.它是一个枚举值.
                TraversalRequest request = new TraversalRequest(focusDirection);
                row.MoveFocus(request);
                row.Focus();
                row.IsSelected = true;
            }
             
        }
         
        private void CellEdit_TextChanged(object sender, TextChangedEventArgs e)
        { 
            if (IODataGridSource.TextChange != null)
            {
                var txt = sender as TextBox;
                if (txt.IsFocused)
                {
                    string columnNameEN = txt.Tag.ToString(); 

                    var conditionType = Condition.GetType();
                    PropertyInfo modifyProperty = TypePropertylist.FirstOrDefault(fp => fp.Name == columnNameEN);

                    if (modifyProperty != null)
                    {
                        var mobj = modifyProperty.GetValue(IODataGridSource.SelectedItem, null); 
                        this.IODataGridSource.TextChange(columnNameEN, conditionType, mobj); 
                    }
                }
            }
        }
        private void LoadingRow(object sender, DataGridRowEventArgs e)
        { 
            int index = e.Row.GetIndex();
            e.Row.Header = index + 1;
            e.Row.PreviewMouseLeftButtonDown -= Row_MouseDown;
            e.Row.PreviewMouseLeftButtonDown += Row_MouseDown; 
        }
        void Row_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridRow row = (DataGridRow)sender;
            row.IsSelected = true;
            var orgidg = this.DataContext as IDataGrid;
            var detailVisibility = orgidg.DetailVisibility;
            if (row.DetailsVisibility == Visibility.Collapsed && detailVisibility == "Visible")
            {
                row.DetailsVisibility = Visibility.Visible;
            }
            else
            {
                row.DetailsVisibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region filter
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt.IsFocused)
            {
                if (ItemsSource != null && ItemsSource.Count > 0)
                {
                    if (ComparePropertyList == null)//筛选比较的列
                    {
                        ComparePropertyList = new List<FilterProperty>();
                    }
                   
                    string cnColumnName = txt.Tag.ToString();//属性名  是中文名
                    var currentColumnConfig = DataGridConfig.ColumnConfig.First(p => p.ColumnNameCN == cnColumnName);
                    string enColumnName = currentColumnConfig.ColumnNameEN; //找到对应的英文名 
                   
                     
                    string text = txt.Text;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        PropertyInfo filterProperty = TypePropertylist.FirstOrDefault(fp => fp.Name == enColumnName);
                        if (filterProperty != null)
                        {
                            object filterValue;
                            string conditionStr = GetValueAndCondition(text, out filterValue);
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
                        PropertyInfo filterProperty = TypePropertylist.FirstOrDefault(fp => fp.Name == enColumnName);
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

                    SetFilterComm(text, Condition);
                    
                    int rowCount = this.ListCollectionView.Count;//最大行
                     
                    if (rowCount <= 0)
                    {
                        this.ListCollectionView.Remove(this.Condition);
                        this.ListCollectionView.AddNewItem(this.Condition);
                        this.ListCollectionView.CommitNew();
                        if (this.ListCollectionView.Count != 1)
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


        private long usingResource = 0;

        List<FilterProperty> ComparePropertyList = new List<FilterProperty>();
        public List<string> NoFilterColumn = new List<string>() { 
            "ischecked" , "isselect" ,"netstate"
        };
        public List<string> NumberColumn = new List<string>() {  
            "decimal",
            "int",
            "int32",
            "int64",
            "long",
            "double",
            "float"
        };
        public List<string> ToStringColumn = new List<string>() { 
            "drugserialno"  
        };
        public void SetFilterComm(string objtext, Object condition)
        {
            if (condition != null)
            {
                #region
                var dataGridData = this;
                bool excuteFilter = false;

                #region 锁检测
                if (1 == Interlocked.Read(ref usingResource))
                {
                    return;
                }
                //原始值是0，判断是时候使用原始值，但判断后值为1，进行了设置
                if (0 == Interlocked.Exchange(ref usingResource, 1))
                {
                    if (objtext != null)
                    {
                        string text = objtext.ToString();
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
                        #region
                        //ICollectionView _ItemsSourceView = CollectionViewSource.GetDefaultView(ItemsSource);
                        ItemsSourceView.Filter = new Predicate<object>((obj) =>
                        {
                            bool isFilter = true;

                            foreach (FilterProperty pinfo in ComparePropertyList) //循环筛选出来的属性
                            {

                                string columnNameEn = pinfo.PropertyName;
                                var filterValue = pinfo.PropertyValue;//过滤的值
                                string columnType = pinfo.PropertyInfo.PropertyType.ToString().Replace("System.Nullable`1[", "").Replace("]", "").Replace("System.", "").ToLower();
                                if (ToStringColumn.Contains(columnNameEn.ToLower()))//如果是int decimal等需要字符串匹配的字段 则修改类型
                                {
                                    columnType = "string";
                                }

                                if (filterValue != null)
                                {
                                    #region 重点内容 这里开始执行真正的比较

                                    object rowValue = this.GetPropertyValue(obj, pinfo.PropertyInfo);//数据行的值

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
                                        if (isFilter == false && columnNameEn.ToLower() == "drugartno")
                                        {
                                            var drugArtNoAttach = ToolFunction.GetPropertyValue(obj, "DrugArtNoAttach");
                                            if (drugArtNoAttach != null)
                                            {
                                                isFilter = CompareValue(columnType, drugArtNoAttach, filterValue, pinfo.ConditionStr);
                                            }
                                        }
                                        if (isFilter == false && columnNameEn.ToLower() == "druggenericname")
                                        {
                                            var mnemonicCode = ToolFunction.GetPropertyValue(obj, "MnemonicCode");
                                            if (mnemonicCode != null)
                                            {
                                                isFilter = CompareValue(columnType, mnemonicCode, filterValue, pinfo.ConditionStr);
                                            }
                                        }
                                        if (isFilter == false && columnNameEn.ToLower() == "companyname")
                                        {
                                            var mnemonicCode = ToolFunction.GetPropertyValue(obj, "MnemonicCode");
                                            if (mnemonicCode != null)
                                            {
                                                isFilter = CompareValue(columnType, mnemonicCode, filterValue, pinfo.ConditionStr);
                                            }
                                        }
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
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error(ex);
                }
                finally
                {
                    Interlocked.Exchange(ref usingResource, 0);
                }
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
                    conditionStr = "like";
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

        public object GetPropertyValue(object obj, PropertyInfo info)
        {
            return info.GetValue(obj, null);
        }
        #endregion

        private void btnSaveStyle_Click(object sender, RoutedEventArgs e)
        {
            SaveColumn(sender, e);
        } 
    }

    
}
