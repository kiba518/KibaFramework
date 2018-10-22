using DTO;
using Proxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Utility;

namespace ViewModel
{
    public class VM_PageUserList : BaseViewModel
    {
        TestDataProxy testDataProxy = new TestDataProxy();
       
        #region 属性 
        public DataGrid<User> _DataGrid = new DataGrid<User>();

        public DataGrid<User> DataGrid
        {
            get
            {
                return this._DataGrid;
            }
            set
            {
                this._DataGrid = value;
                OnPropertyChanged();
            }
        }
     
        #endregion

        public VM_PageUserList()
        {
            Static.StaticData.DataGridConfig.Add(testDataProxy.GetDataGridConfig());
            DataGrid.DataGridName = "用户信息列表";
            DataGrid.SkipNumber = 3;
            DataGrid.BindSource(Load, null);
        }

        public void Load(User user)
        {
            int currentPage = DataGrid.CurrentPage;
            int skipNumber = DataGrid.SkipNumber;
            testDataProxy.GeDataGridData(user, currentPage, skipNumber, (list, count, msg) =>
            {
                DataGrid.SetItemsSource(list);
                DataGrid.RecordCount = count;
            });
        } 
        

    }
}
