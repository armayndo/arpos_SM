using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace arpos_SM.Models
{
    [DelimitedRecord(";")]
    public class FileHelperModel
    {

        public string ID_BRG { get; set; }

        public string NM_BRG { get; set; }

        public int STOK_MIN { get; set; }

        //use nuget : FileHelpers
        [FieldConverter(ConverterKind.Date, "dd-MMM-yyyy")]
        //[FieldNullValue(typeof(DateTime),"1900-01-01")]
        public DateTime? EXP_TGL { get; set; }

        [FieldNullValue(typeof(string), "-")]
        public string SATUAN { get; set; }

        public int? SATUAN_JUAL { get; set; }

        public int? HRG_MODAL { get; set; }

        public int? HRG_JUAL { get; set; }

        public int? STOK { get; set; }

        [FieldNullValue(typeof(string), "-")]
        public string OWNER { get; set; }

        [FieldConverter(ConverterKind.Date, "dd-MMM-yyyy")]
        public DateTime? LAST_TRN { get; set; }

        public string DISC_KET { get; set; }

        public int? PROFIT { get; set; }

        public int? PEMBULATAN { get; set; }
    }
}
