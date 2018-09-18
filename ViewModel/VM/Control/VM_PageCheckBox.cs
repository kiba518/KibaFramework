using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class VM_PageCheckBox : BaseViewModel
    {
        public CheckBox<bool> TestCheckBox { get; set; }
       
        public VM_PageCheckBox()
        {
            TestCheckBox = new CheckBox<bool>();
            TestCheckBox.Content = "CheckBox测试";
            TestCheckBox.IsChecked = true;
            TestCheckBox.ClickCallBack = (check) => {
                MessageBox(check.ToString());
            };  
        }  
    }
}
