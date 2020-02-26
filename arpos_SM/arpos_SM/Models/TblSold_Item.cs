using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace arpos_SM.Models
{
    public class TblSold_Item
    {
        [AutoIncrement, PrimaryKey]
        public Int32 ID { get; set; }

        //use nuget : SQLiteNetExtensions.Attributes
        [ForeignKey(typeof(TblBill))]
        public Int32 BIL_NO { get; set; }

        [MaxLength(5)]
        public string ID_BRG { get; set; }

        [MaxLength(100)]
        public string NM_BRG { get; set; }

        [MaxLength(5)]
        public string UNIT { get; set; }

        public int QTY { get; set; }

        public int HRG_SATUAN { get; set; }

        public int HRG_TOTAL { get; set; }

        public int DISCOUNT { get; set; }

        [MaxLength(100)]
        public string DISC_KET { get; set; }

        public int HRG_MODAL { get; set; }

        public int PROFIT { get; set; }

        public int PEMBULATAN { get; set; }

        //[ManyToOne] // Many to one relationship with Vehicle
        //public TblBill TblBill { get; set; }
    }
}
