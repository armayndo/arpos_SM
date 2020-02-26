using System;
using System.Collections.Generic;
using System.Text;

namespace arpos_SM.Asset
{
    public class GenPK
    {
        string[] ABC = { "0",
                       "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                       "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                       "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4",
                       "5", "6", "7", "8", "9", "a", "b", "c", "d", "e",
                       "f", "g", "h", "i", "j", "k", "l", "m", "n", "o" };

        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

            //byte[] data = new byte[1];
            //using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            //{
            //    crypto.GetNonZeroBytes(data);
            //    data = new byte[maxSize];
            //    crypto.GetNonZeroBytes(data);
            //}
            //StringBuilder result = new StringBuilder(maxSize);
            //foreach (byte b in data)
            //{
            //    result.Append(chars[b % (chars.Length)]);
            //}
            Random rnd = new Random();
            string result = chars[rnd.Next(62)].ToString();

            return result.ToString();
        }

        public string genPaskey0()
        {
            string pKey = "";
            //d+m|mm|d
            pKey = Convert.ToString(DateTime.Now.Day + DateTime.Now.Month) + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            return pKey;
        }

        public string genPaskey1()
        {
            string pKey = "";
            //X|date to Alphabet{+/-}1|X|month to Alphabet with Upper/Lower base on AM/PM
            if (DateTime.Now.Day % 2 != 0) //ganjil
            {
                pKey = pKey + ABC[(DateTime.Now.Day - 1)];
            }
            else
            {
                pKey = pKey + ABC[(DateTime.Now.Day + 1)];
            }

            if (DateTime.Now.TimeOfDay.TotalHours > 12)//PM
            {
                pKey = pKey + "" + ABC[DateTime.Now.Month].ToUpper();
            }
            else
            {
                pKey = pKey + "" + ABC[DateTime.Now.Month].ToLower();
            }

            return pKey;
        }

        public string genPaskey2()
        {
            string pKey = "";
            //X|date to Alphabet{+/-}1|X|month to Alphabet with Upper/Lower base on AM/PM
            if (DateTime.Now.Day % 2 != 0) //ganjil
            {
                //pKey = pKey + ABC[(DateTime.Now.Day - 1)];
                pKey = pKey + ABC[(DateTime.Now.Hour - 1)];
            }
            else
            {
                //pKey = pKey + ABC[(DateTime.Now.Day + 1)];
                pKey = pKey + ABC[(DateTime.Now.Hour + 1)];
            }

            if (DateTime.Now.TimeOfDay.TotalHours > 12)//PM
            {
                pKey = pKey + ABC[(DateTime.Now.Day + DateTime.Now.Month)] + ABC[DateTime.Now.Month].ToUpper();
            }
            else
            {
                pKey = pKey + ABC[(DateTime.Now.Day + DateTime.Now.Month)] + ABC[DateTime.Now.Month].ToLower();
            }

            return pKey;
        }

        public string genPaskey3()
        {
            string pKey = "";
            //X|date to Alphabet{+/-}1|X|month to Alphabet with Upper/Lower base on AM/PM
            if ((DateTime.Now.Day % 2 != 0) && (DateTime.Now.Day >= DateTime.Now.Month)) //ganjil dan tgl >= bln
            {
                pKey = pKey + ABC[(DateTime.Now.Day - DateTime.Now.Month)] + ABC[(DateTime.Now.Day - 1)];
            }
            else if ((DateTime.Now.Day % 2 != 0) && (DateTime.Now.Day < DateTime.Now.Month)) //ganjil dan tgl < bln
            {
                pKey = pKey + ABC[(DateTime.Now.Day)] + ABC[(DateTime.Now.Day - 1)];
            }
            else if ((DateTime.Now.Day % 2 == 0) && (DateTime.Now.Day >= DateTime.Now.Month)) //genap dan tgl >= bln
            {
                pKey = pKey + ABC[(DateTime.Now.Day - DateTime.Now.Month)] + ABC[(DateTime.Now.Day + 1)];
            }
            else if ((DateTime.Now.Day % 2 == 0) && (DateTime.Now.Day < DateTime.Now.Month)) //genap dan tgl < bln
            {
                pKey = pKey + ABC[(DateTime.Now.Day)] + ABC[(DateTime.Now.Day + 1)];
            }

            if (DateTime.Now.TimeOfDay.TotalHours > 12)//PM
            {
                pKey = pKey + ABC[(DateTime.Now.Day + DateTime.Now.Month)] + ABC[DateTime.Now.Month].ToUpper();
            }
            else
            {
                pKey = pKey + ABC[(DateTime.Now.Day + DateTime.Now.Month)] + ABC[DateTime.Now.Month].ToLower();
            }

            return pKey;
        }

        public string genPaskey1gen()
        {
            string pKey = "";
            //X|date to Alphabet{+/-}1|X|month to Alphabet with Upper/Lower base on AM/PM
            if (DateTime.Now.Day % 2 != 0) //ganjil
            {
                pKey = GetUniqueKey(1) + pKey + ABC[(DateTime.Now.Day - 1)];
            }
            else
            {
                pKey = GetUniqueKey(1) + pKey + ABC[(DateTime.Now.Day + 1)];
            }

            if (DateTime.Now.TimeOfDay.TotalHours > 12)//PM
            {
                pKey = pKey + GetUniqueKey(1) + ABC[DateTime.Now.Month].ToUpper();
            }
            else
            {
                pKey = pKey + GetUniqueKey(1) + ABC[DateTime.Now.Month].ToLower();
            }

            return pKey;
        }

        public string genPaskey2gen()
        {
            string pKey = "";
            //X|date to Alphabet{+/-}1|X|month to Alphabet with Upper/Lower base on AM/PM
            if (DateTime.Now.Day % 2 != 0) //ganjil
            {
                //pKey = pKey + GetUniqueKey(1) + ABC[(DateTime.Now.Day - 1)];
                pKey = pKey + GetUniqueKey(1) + ABC[(DateTime.Now.Hour - 1)];
            }
            else
            {
                //pKey = pKey + GetUniqueKey(1) + ABC[(DateTime.Now.Day + 1)];
                pKey = pKey + GetUniqueKey(1) + ABC[(DateTime.Now.Hour + 1)];
            }

            if (DateTime.Now.TimeOfDay.TotalHours > 12)//PM
            {
                pKey = pKey + ABC[(DateTime.Now.Day + DateTime.Now.Month)] + ABC[DateTime.Now.Month].ToUpper();
            }
            else
            {
                pKey = pKey + ABC[(DateTime.Now.Day + DateTime.Now.Month)] + ABC[DateTime.Now.Month].ToLower();
            }

            return pKey;
        }

        public string genPaskey3gen()
        {
            string pKey = "";
            //X|date to Alphabet{+/-}1|X|month to Alphabet with Upper/Lower base on AM/PM
            if ((DateTime.Now.Day % 2 != 0) && (DateTime.Now.Day >= DateTime.Now.Month)) //ganjil dan tgl >= bln
            {
                pKey = pKey + ABC[(DateTime.Now.Day - DateTime.Now.Month)] + ABC[(DateTime.Now.Day - 1)];
            }
            else if ((DateTime.Now.Day % 2 != 0) && (DateTime.Now.Day < DateTime.Now.Month)) //ganjil dan tgl < bln
            {
                pKey = pKey + ABC[(DateTime.Now.Day)] + ABC[(DateTime.Now.Day - 1)];
            }
            else if ((DateTime.Now.Day % 2 == 0) && (DateTime.Now.Day >= DateTime.Now.Month)) //genap dan tgl >= bln
            {
                pKey = pKey + ABC[(DateTime.Now.Day - DateTime.Now.Month)] + ABC[(DateTime.Now.Day + 1)];
            }
            else if ((DateTime.Now.Day % 2 == 0) && (DateTime.Now.Day < DateTime.Now.Month)) //genap dan tgl < bln
            {
                pKey = pKey + ABC[(DateTime.Now.Day)] + ABC[(DateTime.Now.Day + 1)];
            }

            if (DateTime.Now.TimeOfDay.TotalHours > 12)//PM
            {
                pKey = pKey + ABC[(DateTime.Now.Day + DateTime.Now.Month)] + ABC[DateTime.Now.Month].ToUpper();
            }
            else
            {
                pKey = pKey + ABC[(DateTime.Now.Day + DateTime.Now.Month)] + ABC[DateTime.Now.Month].ToLower();
            }

            return pKey;
        }
    }
}
