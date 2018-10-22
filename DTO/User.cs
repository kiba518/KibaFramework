using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class User: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool _Check =false;

        public bool Check
        {
            get
            {
                return this._Check;
            }
            set
            {
                this._Check = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Check"));
                }
            }
        }
      
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
       
    }
}
