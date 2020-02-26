using SQLite;
using System;

namespace arpos_SM.Models
{
    public class TblCart0
    {
        [AutoIncrement, PrimaryKey]
        public Int32 NOM { get; set; }

        [MaxLength(5)]
        public string ID_BRG { get; set; }

        [MaxLength(100)]
        public string NM_BRG { get; set; }

        public int QTY { get; set; }

        [MaxLength(5)]
        public string SATUAN { get; set; }

        public int HRG_SATUAN { get; set; }

        public Int32 HRG_TOTAL { get; set; }

        public int DISCOUNT { get; set; }

        [MaxLength(100)]
        public string DISC_KET { get; set; }

        public Int32 HRG_MODAL { get; set; }

        public int PROFIT { get; set; }

        public int PEMBULATAN { get; set; }
    }
}
