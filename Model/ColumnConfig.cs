using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class ColumnConfig 
    {
        [DataMember]
        public List<ColumnPreImage> ColumnPreImage { get; set; } 
        [DataMember]
        public int ColumnId { get; set; }
        public int DataGridId { get; set; }


        [DataMember]
        public string ColumnNameCN { get; set; }

        [DataMember]
        public string ColumnNameEN { get; set; }


        private int _ColumnControlType = 0;

        [DataMember]
        public int ColumnControlType
        {
            get { return _ColumnControlType; }
            set { _ColumnControlType = value; }
        }
         

        private bool _HasEnter = false;//触发回车

        [DataMember]
        public bool HasEnter
        {
            get { return _HasEnter; }
            set { _HasEnter = value; }
        }

        private int _Index = 0;

        [DataMember]
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }


        [DataMember]
        public bool HasAllCheck { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public string ViewConvert { get; set; }

        [DataMember]
        public string DoubleViewColumnNameEN { get; set; }

        [DataMember]
        public bool IsVisibility { get; set; }
        [DataMember]
        public string Pretext { get; set; }

        [DataMember]
        public string DoublePretext { get; set; }
        [DataMember]
        public string HeaderBackground { get; set; }
        [DataMember]
        public string ForegroundConvert { get; set; }
        [DataMember]
        public string DoubleIsVisibility { get; set; }    
    }
}
