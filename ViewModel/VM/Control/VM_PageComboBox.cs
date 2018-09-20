using DTO;
using Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class VM_PageComboBox : BaseViewModel
    {
        public ComboBox<User> TestComboBox { get; set; }
        TestDataProxy proxy = new TestDataProxy();
        public VM_PageComboBox()
        {
            TestComboBox = new ComboBox<User>();
            TestComboBox.SetItemsSource(proxy.GetComboBoxData());
            TestComboBox.SelectCallBack = (user) => {
                MessageBox(user.Name);
            };
        }  
    }
}
