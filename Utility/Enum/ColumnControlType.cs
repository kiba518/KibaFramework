using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility 
{
    [Serializable]
    public enum ColumnControlType
    {
        TextBlock = 0,
        TextBox = 1, 
        Image = 2,
        CheckBox = 3,
    } 
}
