using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace arpos_SM.Models
{
    public class TblPosBill
    {
        [PrimaryKey]
        public Int32 BIL_NO { get; set; }

        [MaxLength(30)]
        public string NM_PEL { get; set; }

        public DateTime BIL_TGL { get; set; }

        public Int32 SPCL_DISC { get; set; }

        public Int32 UANG_TERIMA { get; set; }

        public Int32 UANG_KEMBALI { get; set; }

        [MaxLength(10)]
        public string JENIS_BAYAR { get; set; }

        public Int32 BAYAR_LAIN { get; set; }

        public DateTime KAS_TGL { get; set; }

        [MaxLength(10)]
        public string OPRBY { get; set; }

        //[OneToMany] // One to many relationship with Person
        //public List<TblSold_Item> Sold_Item { get; set; }
    }
}
