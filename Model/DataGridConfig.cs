using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Model
{
    public partial class DataGridConfig
    {
        [DataMember]
        public List<ColumnConfig> ColumnConfig { get; set; }
        [DataMember]
        public int DataGridId { get; set; }
        [DataMember]
        public string DataGridTemplateName { get; set; }
        [DataMember]
        public bool HasFilter { get; set; }
        [DataMember]
        public bool HasPaging { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string RowForegroundConvert { get; set; } 
        [DataMember]
        public bool HasNo { get; set; } 
        [DataMember]
        public int FrozenColumnCount { get; set; }
        [DataMember]
        public bool HideControlRow { get; set; }   
    }
}
