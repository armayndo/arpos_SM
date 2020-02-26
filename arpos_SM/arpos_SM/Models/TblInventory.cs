using SQLite;
using System;

namespace arpos_SM.Models
{
    public class TblInventory
    {
        [PrimaryKey, MaxLength(5)]
        public string ID_BRG { get; set; }

        [MaxLength(100)]
        public string NM_BRG { get; set; }

        public int STOK_MIN { get; set; }

        public DateTime EXP_TGL { get; set; }

        [MaxLength(5)]
        public string SATUAN { get; set; }

        public int SATUAN_JUAL { get; set; }

        public int HRG_MODAL { get; set; }

        public int HRG_JUAL { get; set; }

        public int STOK { get; set; }

        [MaxLength(10)]
        public string OWNER { get; set; }

        public DateTime LAST_TRN { get; set; }
    }
}
