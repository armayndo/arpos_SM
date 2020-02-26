using System;

namespace arpos_SM.Models
{
    public class InventorySearch
    {
        public string ID_BRG { get; set; }

        public string NM_BRG { get; set; }

        public string DETAIL { get; set; }
        public int STOK { get; set; }

        public DateTime LAST_TRN { get; set; }

        public int ColorBehav1 { get; set; }
        public int ColorBehav2 { get; set; }
        public string HRG_MODAL { get; set; }
        public string STOK_MIN { get; set; }
        public string OWNER { get; set; }
        public string HRG_JUAL { get; set; }
        public string STR_EXP { get; set; }
        public string STR_STOK { get; set; }
    }
}
