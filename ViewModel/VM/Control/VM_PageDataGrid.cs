using DTO;
using Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class VM_PageDataGrid : BaseViewModel
    {
        public DataGrid<User> TestDataGrid { get; set; }
        TestDataProxy proxy = new TestDataProxy();
        public VM_PageDataGrid()
        {
            TestDataGrid = new DataGrid<User>();
            TestDataGrid.ItemsSource = proxy.GetListDataProxy();
            TestDataGrid.SelectCallBack = (user) => {
                MessageBox(user.Name);
            };
        }  
    }
}
