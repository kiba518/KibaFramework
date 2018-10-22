using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility 
{
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class IsNullAttribute : System.Attribute
    {
        public string IsNull { get; set; }
        public IsNullAttribute(string isNull)
        {
            this.IsNull = isNull;
        }
    }
}
