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
        private BaseCommand filterCommand;

        public BaseCommand(Action<object> action)
        {
            ExecuteAction = action;
        }

        public BaseCommand(BaseCommand filterCommand)
        {
            this.filterCommand = filterCommand;
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
