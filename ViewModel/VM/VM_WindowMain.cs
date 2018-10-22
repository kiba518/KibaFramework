using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ViewModel
{
    public class VM_WindowMain : BaseViewModel
    {
       
        public string _HeaderName = "HeaderName_KibaFramework";
        public string HeaderName { get { return _HeaderName; } set { _HeaderName = value; OnPropertyChanged(); } }
        public Page _FrameSource;
        public Page FrameSource { get { return _FrameSource; } set { _FrameSource = value; OnPropertyChanged(); } }  
        public BaseCommand ChangeFrameSourceCommand
        {
            get
            {
                return new BaseCommand(ChangeFrameSourceCommand_Executed);
            }
        }
        public void ChangeFrameSourceCommand_Executed(object obj)
        {
            string pageName = obj.ToString();
           switch(pageName)
           {
               case "PageMain":
                   FrameSource = new VM_PageMain().UIElement as Page;
                   break;
               case "PageUser":
                   FrameSource = new VM_PageUser().UIElement as Page;
                   break;
                case "PageTextBox":
                    FrameSource = new VM_PageTextBox().UIElement as Page;
                    break;
                case "PageCheckBox":
                    FrameSource = new VM_PageCheckBox().UIElement as Page;
                    break;
                case "PageComboBox":
                    FrameSource = new VM_PageComboBox().UIElement as Page;
                    break;
                case "PageDataGrid":
                    FrameSource = new VM_PageDataGrid().UIElement as Page;
                    break;
                case "PageDataGridMid":
                    FrameSource = new VM_PageDataGridMid().UIElement as Page;
                    break;
                case "PageDataGridAdvanced":
                    FrameSource = new VM_PageDataGridAdvanced().UIElement as Page;
                    break; 
                case "PageUserList":
                    FrameSource = new VM_PageUserList().UIElement as Page;
                    break; 
                    

           }
        }

        public VM_WindowMain()
        {
            FrameSource = new VM_PageMain().UIElement as Page;
        }
    }
}
