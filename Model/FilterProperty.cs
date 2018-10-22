using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class FilterProperty
    {
        public string PropertyName { get; set; }
        public string ConditionStr { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public object PropertyValue { get; set; }
    }
}
