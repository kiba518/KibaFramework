using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class ColumnPreImage
    {
        [DataMember]
        public int PreImageId { get; set; }

        [DataMember]
        public int ColumnTemplateId { get; set; }

        [DataMember]
        public string Convert { get; set; }
        [DataMember]
        public string VisibilityConvert { get; set; }

        [DataMember]
        public string ConvertProperty { get; set; }
        [DataMember]
        public string VisibilityConvertProperty { get; set; }

    }
}
