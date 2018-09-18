using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModel
{
    public class BaseCommand : ICommand
    {
        public Action<object> ExecuteAction;
        public BaseCommand(Action<object> action)
        {
            ExecuteAction = action;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        } 
        public event EventHandler CanExecuteChanged; 
        public void Execute(object parameter)
        { 
            ExecuteAction(parameter); 
        } 
    }
}
