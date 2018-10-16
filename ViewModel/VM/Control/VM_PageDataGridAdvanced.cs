using DTO;
using Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class VM_PageDataGridAdvanced : BaseViewModel
    {
        public DataGrid<User> TestDataGrid { get; set; }
        TestDataProxy proxy = new TestDataProxy();
        public VM_PageDataGridAdvanced()
        {
            TestDataGrid = new DataGrid<User>();
            TestDataGrid.SkipNumber = 3;
            TestDataGrid.BindSource(Load,null);  

            
        }

        public void Load(User user)
        {
            int currentPage = TestDataGrid.CurrentPage;
            int skipNumber = TestDataGrid.SkipNumber;
            proxy.GeDataGridData(user, currentPage, skipNumber, (list, count, msg) =>
            {
                TestDataGrid.SetItemsSource(list);
                TestDataGrid.RecordCount = count;
            });
        }

        public BaseCommand FilterCommand
        {
            get
            {
                return new BaseCommand(FilterCommand_Excute);
            }
        }

        private void FilterCommand_Excute(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
