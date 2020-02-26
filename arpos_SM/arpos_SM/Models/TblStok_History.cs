using SQLite;
using System;

namespace arpos_SM.Models
{
    public class TblStok_History
    {
        [AutoIncrement, PrimaryKey]
        public Int32 ID { get; set; }

        public DateTime TGL { get; set; }

        [MaxLength(5)]
        public string ID_BRG { get; set; }

        public int STOK_AWAL { get; set; }

        public int STOK_ADD { get; set; }

        public int STOK_AKHIR { get; set; }

        [MaxLength(150)]
        public string KET { get; set; }

        [MaxLength(10)]
        public string OPRBY { get; set; }
    }
}
