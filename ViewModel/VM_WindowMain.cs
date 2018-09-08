using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class VM_WindowMain : BaseViewModel
    {
        private string _HeaderName = "HeaderName_KibaFramework";
        public string HeaderName
        {
            get { return _HeaderName; }
            set
            {
                _HeaderName = value;
                OnPropertyChanged();
            }
        }
        public VM_WindowMain()
        {  
        }
    }
}
