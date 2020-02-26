using System;
using System.Collections.Generic;
using System.Text;

namespace arpos_SM.Models
{
    public class CartDisplay
    {
        public int NOM { get; set; }

        public string NM_BRG { get; set; }

        public string DETAIL { get; set; }

        public string ID_BRG { get; set; }

        public Int32 QTY { get; set; }

        public string SATUAN { get; set; }

        public Int32 HRG_TOTAL { get; set; }

        public int DISCOUNT { get; set; }
    }
}
