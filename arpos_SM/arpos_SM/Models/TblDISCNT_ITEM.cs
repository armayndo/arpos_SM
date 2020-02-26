using SQLite;
using System;

namespace arpos_SM.Models
{
    public class TblDISCNT_ITEM
    {
        [AutoIncrement, PrimaryKey]
        public Int32 NOM { get; set; }

        [MaxLength(5)]
        public string ID_BRG { get; set; }

        public int QTY { get; set; }

        public int HRG_DIS_SATUAN { get; set; }

        [MaxLength(26)]
        public string KETERANGAN { get; set; }
    }
}
