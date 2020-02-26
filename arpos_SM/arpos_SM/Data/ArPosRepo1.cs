using arpos_SM.Asset;
using arpos_SM.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arpos_SM.Data
{
    //use nuget sqlite-net-pcl by Frank A. Krueger
    public class ArPosRepo
    {
        private static readonly AsyncLock Locker = new AsyncLock();

        readonly SQLiteAsyncConnection DBAsyncCon;

        readonly SQLiteConnection DBCon;

        public ArPosRepo(string dbPath)
        {
            DBCon = new SQLiteConnection(dbPath, false);
            DBAsyncCon = new SQLiteAsyncConnection(dbPath,false);
            DBAsyncCon.CreateTableAsync<TblInventory>().Wait();
            DBAsyncCon.CreateTableAsync<TblDISCNT_ITEM>().Wait();
            DBAsyncCon.CreateTableAsync<TblCart0>().Wait();
            DBAsyncCon.CreateTableAsync<TblBill>().Wait();
            DBAsyncCon.CreateTableAsync<TblSold_Item>().Wait();
            DBAsyncCon.CreateTableAsync<TblStok_History>().Wait();
            DBAsyncCon.CreateTableAsync<TblPosBill>().Wait();
            DBAsyncCon.CreateTableAsync<TblPosBillDetail>().Wait();
        }

        public int CheckCount(string tname)
        {
            int result = 0;
            string strQuery = "Select COUNT(*) from " + tname;

            result = DBCon.ExecuteScalar<int>(strQuery);

            return result;
        }

        public Task<int> DeleteAllRecAsync(string tname)
        {
            //return database.DeleteAsync(item);
            return DBAsyncCon.ExecuteAsync("Delete from " + tname);
        }

        public Task<int> SaveItemAsync(TblInventory item)
        {
            //if (item.ID_BRG != "")
            //{
            //    return database.UpdateAsync(item);
            //}
            //else
            //{
            //    return database.InsertAsync(item);
            //}
            return DBAsyncCon.InsertAsync(item);
           
        }

        public Task<int> InsertInvAllSync(List<TblInventory> lstData)
        {
            return DBAsyncCon.InsertAllAsync(lstData);
        }
        public Task<int> InsertInvAllSync(List<TblDISCNT_ITEM> lstData)
        {
            return DBAsyncCon.InsertAllAsync(lstData);
        }
        public Task<int> InsertInvAllSync(List<TblBill> lstData)
        {
            return DBAsyncCon.InsertAllAsync(lstData);
        }
        public Task<int> InsertInvAllSync(List<TblSold_Item> lstData)
        {
            return DBAsyncCon.InsertAllAsync(lstData);
        }
        public Task<int> InsertInvAllSync(List<TblStok_History> lstData)
        {
            return DBAsyncCon.InsertAllAsync(lstData);
        }



        public Task<TblInventory> GetInventoryAsync(string idbrg)
        {
            return DBAsyncCon.Table<TblInventory>().Where(i => i.ID_BRG == idbrg).FirstOrDefaultAsync();
        }

        public async Task<IList<TblInventory>> GetInventories(string IdBrg)
        {
            //using (await Locker.LockAsync())
            //{
            //    return await Database.Table<Movie>().Where(x => x.Id > 0).ToListAsync();
            //}

            //return await database.Table<TblInventory>().Where(x => x.ID_BRG.Contains(IdBrg)).ToListAsync();
            return await DBAsyncCon.Table<TblInventory>().Where(x => x.ID_BRG.Contains(IdBrg)).ToListAsync();
        }

        public async Task<IList<InventorySearch>> GetInventorySearchAsync()
        {
            using (await Locker.LockAsync())
            {
                //return await database.QueryAsync<TblInventory>("SELECT ID_BRG, STOK FROM [TblInventory] WHERE [ID_BRG] like '%'");
                //|| STOK_MIN || '_' || (STOK + (select ifnull(sum(STOK),0) from [TblInventory] WHERE lower(NM_BRG) = lower('ZZT ' || a.NM_BRG)))
                //            || '_' || (STOK + (select ifnull(sum(STOK),0) from [TblInventory] WHERE lower(NM_BRG) = lower('ZZT ' || a.NM_BRG)))
                string strQuery = @"SELECT ID_BRG, NM_BRG, 'Stk:' || CASE SATUAN WHEN 'Gr' then cast(STOK as REAL)/1000 ELSE STOK END || ' ' || CASE SATUAN WHEN 'Gr' then 'Kg' ELSE 'Pcs' END 
            || '  Hrg:' || CASE WHEN length(HRG_JUAL) > 6 then substr(HRG_JUAL,1,length(HRG_JUAL)-6)||','||substr(HRG_JUAL,length(HRG_JUAL)-5,3)||','||substr(HRG_JUAL,length(HRG_JUAL)-2) WHEN length(HRG_JUAL) > 3 then substr(HRG_JUAL,1,length(HRG_JUAL)-3)||','||substr(HRG_JUAL,length(HRG_JUAL)-2) ELSE HRG_JUAL END || 
            CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END || CASE strftime('%Y-%m-%d', EXP_TGL) WHEN '0001-01-01' then '' ELSE '  Exp:'||strftime('%Y-%m-%d', EXP_TGL) END
            || ' ' || (select CASE WHEN COUNT(*) > 0 THEN '[---]' ELSE '' END FROM [TblDISCNT_ITEM] where [ID_BRG] = a.ID_BRG )  as DETAIL
            , CASE WHEN (STOK + (select ifnull(sum(STOK),0) from [TblInventory] WHERE lower(NM_BRG) = lower('ZZT ' || a.NM_BRG))) = 0 THEN 3 WHEN STOK_MIN >= (STOK + (select ifnull(sum(STOK),0) from [TblInventory] WHERE lower(NM_BRG) = lower('ZZT ' || a.NM_BRG)))  THEN 2 ELSE 1 END as STOK, LAST_TRN
            , CASE WHEN strftime('%Y-%m-%d', EXP_TGL) = '0001-01-01' THEN 0 WHEN EXP_TGL < date('NOW') THEN 3 ELSE 0 END as ColorBehav1
            , CASE WHEN length(HRG_MODAL) > 6 then substr(HRG_MODAL,1,length(HRG_MODAL)-6)||','||substr(HRG_MODAL,length(HRG_MODAL)-5,3)||','||substr(HRG_MODAL,length(HRG_MODAL)-2) WHEN length(HRG_MODAL) > 3 then substr(HRG_MODAL,1,length(HRG_MODAL)-3)||','||substr(HRG_MODAL,length(HRG_MODAL)-2) ELSE HRG_MODAL END || 
            CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END as HRG_MODAL
            , CASE SATUAN WHEN 'Gr' then (STOK_MIN/1000) || ' Kg' ELSE STOK_MIN || ' Pcs' END as STOK_MIN
            , OWNER
            , CASE WHEN length(HRG_JUAL) > 6 then substr(HRG_JUAL,1,length(HRG_JUAL)-6)||','||substr(HRG_JUAL,length(HRG_JUAL)-5,3)||','||substr(HRG_JUAL,length(HRG_JUAL)-2) WHEN length(HRG_JUAL) > 3 then substr(HRG_JUAL,1,length(HRG_JUAL)-3)||','||substr(HRG_JUAL,length(HRG_JUAL)-2) ELSE HRG_JUAL END || 
            CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END as HRG_JUAL
            , CASE strftime('%Y-%m-%d', EXP_TGL) WHEN '0001-01-01' then '-' ELSE strftime('%Y-%m-%d', EXP_TGL) END as STR_EXP
            , CASE SATUAN WHEN 'Gr' then cast(STOK as REAL)/1000 ELSE STOK END || ' ' || CASE SATUAN WHEN 'Gr' then 'Kg' ELSE 'Pcs' END as STR_STOK
            FROM [TblInventory] a where NM_BRG = '123'";


                return await DBAsyncCon.QueryAsync<InventorySearch>(strQuery);
            }
        }

        public async Task<IList<InventorySearch>> GetInventorySearchAsync(string strFiltr)
        {
            using (await Locker.LockAsync())
            {
                //return await database.QueryAsync<TblInventory>("SELECT ID_BRG, STOK FROM [TblInventory] WHERE [ID_BRG] like '%'");
                //|| STOK_MIN || '_' || (STOK + (select ifnull(sum(STOK),0) from [TblInventory] WHERE lower(NM_BRG) = lower('ZZT ' || a.NM_BRG)))
                //            || '_' || (STOK + (select ifnull(sum(STOK),0) from [TblInventory] WHERE lower(NM_BRG) = lower('ZZT ' || a.NM_BRG)))
                string strQuery = @"SELECT ID_BRG, NM_BRG, 'Stk:' || CASE SATUAN WHEN 'Gr' then cast(STOK as REAL)/1000 ELSE STOK END || ' ' || CASE SATUAN WHEN 'Gr' then 'Kg' ELSE 'Pcs' END 
            || '  Hrg:' || CASE WHEN length(HRG_JUAL) > 6 then substr(HRG_JUAL,1,length(HRG_JUAL)-6)||','||substr(HRG_JUAL,length(HRG_JUAL)-5,3)||','||substr(HRG_JUAL,length(HRG_JUAL)-2) WHEN length(HRG_JUAL) > 3 then substr(HRG_JUAL,1,length(HRG_JUAL)-3)||','||substr(HRG_JUAL,length(HRG_JUAL)-2) ELSE HRG_JUAL END || 
            CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END || CASE strftime('%Y-%m-%d', EXP_TGL) WHEN '0001-01-01' then '' ELSE '  Exp:'||strftime('%Y-%m-%d', EXP_TGL) END
            || ' ' || (select CASE WHEN COUNT(*) > 0 THEN '[---]' ELSE '' END FROM [TblDISCNT_ITEM] where [ID_BRG] = a.ID_BRG )  as DETAIL
            , CASE WHEN (STOK + (select ifnull(sum(STOK),0) from [TblInventory] WHERE lower(NM_BRG) = lower('ZZT ' || a.NM_BRG))) = 0 THEN 3 WHEN STOK_MIN >= (STOK + (select ifnull(sum(STOK),0) from [TblInventory] WHERE lower(NM_BRG) = lower('ZZT ' || a.NM_BRG)))  THEN 2 ELSE 1 END as STOK, LAST_TRN
            , CASE WHEN strftime('%Y-%m-%d', EXP_TGL) = '0001-01-01' THEN 0 WHEN EXP_TGL < date('NOW') THEN 3 ELSE 0 END as ColorBehav1
            , CASE WHEN length(HRG_MODAL) > 6 then substr(HRG_MODAL,1,length(HRG_MODAL)-6)||','||substr(HRG_MODAL,length(HRG_MODAL)-5,3)||','||substr(HRG_MODAL,length(HRG_MODAL)-2) WHEN length(HRG_MODAL) > 3 then substr(HRG_MODAL,1,length(HRG_MODAL)-3)||','||substr(HRG_MODAL,length(HRG_MODAL)-2) ELSE HRG_MODAL END || 
            CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END as HRG_MODAL
            , CASE SATUAN WHEN 'Gr' then (STOK_MIN/1000) || ' Kg' ELSE STOK_MIN || ' Pcs' END as STOK_MIN
            , OWNER
            , CASE WHEN length(HRG_JUAL) > 6 then substr(HRG_JUAL,1,length(HRG_JUAL)-6)||','||substr(HRG_JUAL,length(HRG_JUAL)-5,3)||','||substr(HRG_JUAL,length(HRG_JUAL)-2) WHEN length(HRG_JUAL) > 3 then substr(HRG_JUAL,1,length(HRG_JUAL)-3)||','||substr(HRG_JUAL,length(HRG_JUAL)-2) ELSE HRG_JUAL END || 
            CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END as HRG_JUAL
            , CASE strftime('%Y-%m-%d', EXP_TGL) WHEN '0001-01-01' then '-' ELSE strftime('%Y-%m-%d', EXP_TGL) END as STR_EXP
            , CASE SATUAN WHEN 'Gr' then cast(STOK as REAL)/1000 ELSE STOK END || ' ' || CASE SATUAN WHEN 'Gr' then 'Kg' ELSE 'Pcs' END as STR_STOK
            FROM [TblInventory] a WHERE 1=1 ";

                if (strFiltr != "")
                {
                    if (strFiltr.Trim().IndexOf(":") == 3 && strFiltr.Split(':').ToArray().Length == 2)
                    {
                        if (strFiltr.Split(':')[0].ToUpper() == "STK")
                        {
                            string strQuery2 = "select * from (" + strQuery + ") where STOK > 1";
                            strQuery = strQuery2;
                        }
                    }
                    else
                    {
                        strQuery += "and ( ";
                        string[] arrFilter = strFiltr.Split(' ').ToArray();
                        for (int i = 0; i < arrFilter.Length; i++)
                        {
                            if (i != arrFilter.Length - 1)
                            {
                                strQuery += "[NM_BRG] like '%" + arrFilter[i] + "%' and ";
                            }
                            else
                            {
                                strQuery += "[NM_BRG] like '%" + arrFilter[i] + "%' ";
                            }
                        }

                        strQuery += ") ";
                    }
                }
                return await DBAsyncCon.QueryAsync<InventorySearch>(strQuery);
            }
        }

        public async Task<IList<InventorySearch>> GetInventorySearchDetAsync(string vIdBrg)
        {
            using (await Locker.LockAsync())
            {
                //return await database.QueryAsync<TblInventory>("SELECT ID_BRG, STOK FROM [TblInventory] WHERE [ID_BRG] like '%'");
                string strQuery = @"SELECT TblDISCNT_ITEM.ID_BRG, 'Hrg:' || CASE WHEN length(HRG_DIS_SATUAN) > 6 then substr(HRG_DIS_SATUAN,1,length(HRG_DIS_SATUAN)-6)||','||substr(HRG_DIS_SATUAN,length(HRG_DIS_SATUAN)-5,3)||','||substr(HRG_DIS_SATUAN,length(HRG_DIS_SATUAN)-2) WHEN length(HRG_DIS_SATUAN) > 3 then substr(HRG_DIS_SATUAN,1,length(HRG_DIS_SATUAN)-3)||','||substr(HRG_DIS_SATUAN,length(HRG_DIS_SATUAN)-2) ELSE HRG_DIS_SATUAN END || CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END as NM_BRG
            , 'Rp ' || CASE SATUAN WHEN 'Gr' then CASE WHEN length((QTY * (HRG_DIS_SATUAN / 1000))) > 6 then substr((QTY * (HRG_DIS_SATUAN / 1000)),1,length((QTY * (HRG_DIS_SATUAN / 1000)))-6)||','||substr((QTY * (HRG_DIS_SATUAN / 1000)),length((QTY * (HRG_DIS_SATUAN / 1000)))-5,3)||','||substr((QTY * (HRG_DIS_SATUAN / 1000)),length((QTY * (HRG_DIS_SATUAN / 1000)))-2) WHEN length((QTY * (HRG_DIS_SATUAN / 1000))) > 3 then substr((QTY * (HRG_DIS_SATUAN / 1000)),1,length((QTY * (HRG_DIS_SATUAN / 1000)))-3)||','||substr((QTY * (HRG_DIS_SATUAN / 1000)),length((QTY * (HRG_DIS_SATUAN / 1000)))-2) ELSE (QTY * (HRG_DIS_SATUAN / 1000)) END  
            ELSE CASE WHEN length((QTY * HRG_DIS_SATUAN)) > 6 then substr((QTY * HRG_DIS_SATUAN),1,length((QTY * HRG_DIS_SATUAN))-6)||','||substr((QTY * HRG_DIS_SATUAN),length((QTY * HRG_DIS_SATUAN))-5,3)||','||substr((QTY * HRG_DIS_SATUAN),length((QTY * HRG_DIS_SATUAN))-2) WHEN length((QTY * HRG_DIS_SATUAN)) > 3 then substr((QTY * HRG_DIS_SATUAN),1,length((QTY * HRG_DIS_SATUAN))-3)||','||substr((QTY * HRG_DIS_SATUAN),length((QTY * HRG_DIS_SATUAN))-2) ELSE (QTY * HRG_DIS_SATUAN) END END || ' [' || KETERANGAN || ']' as DETAIL 
            FROM [TblDISCNT_ITEM] left join TblInventory on TblDISCNT_ITEM.ID_BRG = TblInventory.ID_BRG WHERE TblDISCNT_ITEM.ID_BRG = '" + vIdBrg + "' ORDER By QTY";
                //string strQuery = @"SELECT ID_BRG, HRG_DIS_SATUAN as NM_BRG
                //, KETERANGAN as DETAIL 
                //FROM [TblDISCNT_ITEM]  ";

                return await DBAsyncCon.QueryAsync<InventorySearch>(strQuery);
            }
        }

        public async Task<IList<InventoryTren>> GetInventoryTrenAsync(string vIdBrg, string vSat)
        {
            string strQuery = "";

            //vSat = "Pcs";
            if (vSat == "Pcs")
            {
                strQuery = @"Select A.*, B.*, YYYYMMC as YYYYMMP, CASE STOKADD WHEN 0 THEN NULL ELSE STOKADD END as STOKADD, STOKMIN from 
(
    (
        (
        select strftime('%Y-%m', BIL_TGL) as YYYYMM from TBLBILL group by strftime('%Y-%m', BIL_TGL)
        ) A 
        left join 
        (
        Select strftime('%Y-%m', BIL_TGL) as YYYYMMB, SUM(QTY) as PENJUALAN, SUM(PROFIT) as PROFIT from TBLSOLD_ITEM BSI join TBLBILL BB on BSI.BIL_NO = BB.BIL_NO where ID_BRG = '" + vIdBrg + @"' group by strftime('%Y-%m', BIL_TGL)
        ) B on A.YYYYMM = B.YYYYMMB
    )
    left join
    (
    select strftime('%Y-%m',TGL) as YYYYMMC, sum(STOK_ADD) as STOKADD from TblStok_History where ID_BRG = '" + vIdBrg + @"' and STOK_ADD > 0 group by strftime('%Y-%m',TGL)
    ) C on A.YYYYMM = C.YYYYMMC
)
left join
(
select strftime('%Y-%m',TGL) as YYYYMMD, abs(sum(STOK_ADD)) as STOKMIN from TblStok_History where ID_BRG = '" + vIdBrg + @"' and STOK_ADD < 0 and ket like 'rugi:%' group by strftime('%Y-%m',TGL)
) D on A.YYYYMM = D.YYYYMMD ";
            }
            else if (vSat == "Gr")
            {
                strQuery = @"Select A.*, B.PENJUALAN, B.PROFIT, YYYYMMC as YYYYMMP, CASE STOKADD WHEN 0 THEN NULL ELSE STOKADD END as STOKADD, STOKMIN from 
(
    (
        (
        select strftime('%Y-%m', BIL_TGL) as YYYYMM from TBLBILL group by strftime('%Y-%m', BIL_TGL)
        ) A 
        left join 
        (
        Select strftime('%Y-%m', BIL_TGL) as YYYYMMB, printf('%.2d',SUM(QTY) / 1000) as PENJUALAN, SUM(PROFIT) as PROFIT from TBLSOLD_ITEM BSI join TBLBILL BB on BSI.BIL_NO = BB.BIL_NO where ID_BRG = '" + vIdBrg + @"' group by strftime('%Y-%m', BIL_TGL)
        ) B on A.YYYYMM = B.YYYYMMB
    )
    left join
    (
    select strftime('%Y-%m',TGL) as YYYYMMC, CASE sum(STOK_ADD) WHEN 0 THEN 0 ELSE sum(STOK_ADD) / 1000 END as STOKADD from TblStok_History where ID_BRG = '" + vIdBrg + @"' and STOK_ADD > 0 group by strftime('%Y-%m',TGL)
    ) C on A.YYYYMM = C.YYYYMMC
)
left join
(
select strftime('%Y-%m',TGL) as YYYYMMD, CASE abs(sum(STOK_ADD)) WHEN 0 THEN 0 ELSE abs(sum(STOK_ADD)) / 1000 END as STOKMIN from TblStok_History where ID_BRG = '" + vIdBrg + @"' and STOK_ADD < 0 and ket like 'rugi:%' group by strftime('%Y-%m',TGL)
) D on A.YYYYMM = D.YYYYMMD ";
            }

            using (await Locker.LockAsync())
            {
                return await DBAsyncCon.QueryAsync<InventoryTren>(strQuery);
            }
        }

        //public IList<ListItemClass> GetItemList()
        //{
        //    string strQuery = @"SELECT ID_BRG, NM_BRG FROM [TblInventory] where ID_BRG Not Like 'ZT%' or ID_BRG Not Like 'SY%'";

        //    return DBCon.Query<ListItemClass>(strQuery);
        //}

        public List<ListItemClass> GetItemList()
        {
            List<ListItemClass> result = new List<ListItemClass>();

            string strQuery = @"SELECT ID_BRG, NM_BRG FROM [TblInventory] where ID_BRG Not Like 'ZT%' or ID_BRG Not Like 'SY%'";

            result = DBCon.Query<ListItemClass>(strQuery);

            return result;
        }

        public List<TblInventory> GetInvByName(string nmbrg)
        {
            List<TblInventory> result = new List<TblInventory>();

            string strQuery = @"SELECT * FROM [TblInventory] where UPPER(NM_BRG) = '" + nmbrg.ToUpper() + "'";

            result = DBCon.Query<TblInventory>(strQuery);

            return result;
        }

        public List<InventorySearch> GetInventoryDisc(string vIdBrg)
        {
            List<InventorySearch> result = new List<InventorySearch>();

            string strQuery = @"SELECT TblDISCNT_ITEM.ID_BRG, 'Hrg:' || CASE WHEN length(HRG_DIS_SATUAN) > 6 then substr(HRG_DIS_SATUAN,1,length(HRG_DIS_SATUAN)-6)||','||substr(HRG_DIS_SATUAN,length(HRG_DIS_SATUAN)-5,3)||','||substr(HRG_DIS_SATUAN,length(HRG_DIS_SATUAN)-2) WHEN length(HRG_DIS_SATUAN) > 3 then substr(HRG_DIS_SATUAN,1,length(HRG_DIS_SATUAN)-3)||','||substr(HRG_DIS_SATUAN,length(HRG_DIS_SATUAN)-2) ELSE HRG_DIS_SATUAN END || CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END as NM_BRG
            , 'Rp ' || CASE SATUAN WHEN 'Gr' then CASE WHEN length((QTY * (HRG_DIS_SATUAN / 1000))) > 6 then substr((QTY * (HRG_DIS_SATUAN / 1000)),1,length((QTY * (HRG_DIS_SATUAN / 1000)))-6)||','||substr((QTY * (HRG_DIS_SATUAN / 1000)),length((QTY * (HRG_DIS_SATUAN / 1000)))-5,3)||','||substr((QTY * (HRG_DIS_SATUAN / 1000)),length((QTY * (HRG_DIS_SATUAN / 1000)))-2) WHEN length((QTY * (HRG_DIS_SATUAN / 1000))) > 3 then substr((QTY * (HRG_DIS_SATUAN / 1000)),1,length((QTY * (HRG_DIS_SATUAN / 1000)))-3)||','||substr((QTY * (HRG_DIS_SATUAN / 1000)),length((QTY * (HRG_DIS_SATUAN / 1000)))-2) ELSE (QTY * (HRG_DIS_SATUAN / 1000)) END  
            ELSE CASE WHEN length((QTY * HRG_DIS_SATUAN)) > 6 then substr((QTY * HRG_DIS_SATUAN),1,length((QTY * HRG_DIS_SATUAN))-6)||','||substr((QTY * HRG_DIS_SATUAN),length((QTY * HRG_DIS_SATUAN))-5,3)||','||substr((QTY * HRG_DIS_SATUAN),length((QTY * HRG_DIS_SATUAN))-2) WHEN length((QTY * HRG_DIS_SATUAN)) > 3 then substr((QTY * HRG_DIS_SATUAN),1,length((QTY * HRG_DIS_SATUAN))-3)||','||substr((QTY * HRG_DIS_SATUAN),length((QTY * HRG_DIS_SATUAN))-2) ELSE (QTY * HRG_DIS_SATUAN) END END || ' [' || KETERANGAN || ']' as DETAIL 
            ,TblDISCNT_ITEM.QTY as STOK_MIN ,TblDISCNT_ITEM.HRG_DIS_SATUAN as STOK
            ,KETERANGAN as OWNER
            FROM [TblDISCNT_ITEM] left join TblInventory on TblDISCNT_ITEM.ID_BRG = TblInventory.ID_BRG WHERE TblDISCNT_ITEM.ID_BRG = '" + vIdBrg + "' ORDER By QTY";

            result = DBCon.Query<InventorySearch>(strQuery);

            return result;
        }

        public Task<int> InsertCartSync(TblCart0 lstData)
        {
            return DBAsyncCon.InsertAsync(lstData);
        }

        public int InsertCart(TblCart0 lstData)
        {
            return DBCon.Insert(lstData);
        }

        public async Task<IList<InventorySearch>> GetCartAsync(string vIdBrg)
        {
            using (await Locker.LockAsync())
            {
                //return await database.QueryAsync<TblInventory>("SELECT ID_BRG, STOK FROM [TblInventory] WHERE [ID_BRG] like '%'");
                string strQuery = @"SELECT TblDISCNT_ITEM.ID_BRG, 'Hrg:' || CASE WHEN length(HRG_DIS_SATUAN) > 6 then substr(HRG_DIS_SATUAN,1,length(HRG_DIS_SATUAN)-6)||','||substr(HRG_DIS_SATUAN,length(HRG_DIS_SATUAN)-5,3)||','||substr(HRG_DIS_SATUAN,length(HRG_DIS_SATUAN)-2) WHEN length(HRG_DIS_SATUAN) > 3 then substr(HRG_DIS_SATUAN,1,length(HRG_DIS_SATUAN)-3)||','||substr(HRG_DIS_SATUAN,length(HRG_DIS_SATUAN)-2) ELSE HRG_DIS_SATUAN END || CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END as NM_BRG
            , 'Rp ' || CASE SATUAN WHEN 'Gr' then CASE WHEN length((QTY * (HRG_DIS_SATUAN / 1000))) > 6 then substr((QTY * (HRG_DIS_SATUAN / 1000)),1,length((QTY * (HRG_DIS_SATUAN / 1000)))-6)||','||substr((QTY * (HRG_DIS_SATUAN / 1000)),length((QTY * (HRG_DIS_SATUAN / 1000)))-5,3)||','||substr((QTY * (HRG_DIS_SATUAN / 1000)),length((QTY * (HRG_DIS_SATUAN / 1000)))-2) WHEN length((QTY * (HRG_DIS_SATUAN / 1000))) > 3 then substr((QTY * (HRG_DIS_SATUAN / 1000)),1,length((QTY * (HRG_DIS_SATUAN / 1000)))-3)||','||substr((QTY * (HRG_DIS_SATUAN / 1000)),length((QTY * (HRG_DIS_SATUAN / 1000)))-2) ELSE (QTY * (HRG_DIS_SATUAN / 1000)) END  
            ELSE CASE WHEN length((QTY * HRG_DIS_SATUAN)) > 6 then substr((QTY * HRG_DIS_SATUAN),1,length((QTY * HRG_DIS_SATUAN))-6)||','||substr((QTY * HRG_DIS_SATUAN),length((QTY * HRG_DIS_SATUAN))-5,3)||','||substr((QTY * HRG_DIS_SATUAN),length((QTY * HRG_DIS_SATUAN))-2) WHEN length((QTY * HRG_DIS_SATUAN)) > 3 then substr((QTY * HRG_DIS_SATUAN),1,length((QTY * HRG_DIS_SATUAN))-3)||','||substr((QTY * HRG_DIS_SATUAN),length((QTY * HRG_DIS_SATUAN))-2) ELSE (QTY * HRG_DIS_SATUAN) END END || ' [' || KETERANGAN || ']' as DETAIL 
            FROM [TblDISCNT_ITEM] left join TblInventory on TblDISCNT_ITEM.ID_BRG = TblInventory.ID_BRG WHERE TblDISCNT_ITEM.ID_BRG = '" + vIdBrg + "' ORDER By QTY";
                //string strQuery = @"SELECT ID_BRG, HRG_DIS_SATUAN as NM_BRG
                //, KETERANGAN as DETAIL 
                //FROM [TblDISCNT_ITEM]  ";

                return await DBAsyncCon.QueryAsync<InventorySearch>(strQuery);
            }
        }

        public List<CartDisplay> GetListCart()
        {
            //TblDISCNT_ITEM.ID_BRG, 'Hrg:' || CASE WHEN length(HRG_TOTAL) > 6 then substr(HRG_TOTAL, 1, length(HRG_TOTAL) - 6) || ',' || substr(HRG_TOTAL, length(HRG_TOTAL) - 5, 3) || ',' || substr(HRG_TOTAL, length(HRG_TOTAL) - 2) WHEN length(HRG_TOTAL) > 3 then substr(HRG_TOTAL, 1, length(HRG_TOTAL) - 3) || ',' || substr(HRG_TOTAL, length(HRG_TOTAL) - 2) ELSE HRG_TOTAL END || CASE SATUAN WHEN 'Gr' then '/Kg' ELSE '/Pcs' END as NM_BRG
            //, 'Rp ' || CASE SATUAN WHEN 'Gr' then CASE WHEN length((QTY * (HRG_DIS_SATUAN / 1000))) > 6 then substr((QTY * (HRG_DIS_SATUAN / 1000)), 1, length((QTY * (HRG_DIS_SATUAN / 1000))) - 6) || ',' || substr((QTY * (HRG_DIS_SATUAN / 1000)), length((QTY * (HRG_DIS_SATUAN / 1000))) - 5, 3) || ',' || substr((QTY * (HRG_DIS_SATUAN / 1000)), length((QTY * (HRG_DIS_SATUAN / 1000))) - 2) WHEN length((QTY * (HRG_DIS_SATUAN / 1000))) > 3 then substr((QTY * (HRG_DIS_SATUAN / 1000)), 1, length((QTY * (HRG_DIS_SATUAN / 1000))) - 3) || ',' || substr((QTY * (HRG_DIS_SATUAN / 1000)), length((QTY * (HRG_DIS_SATUAN / 1000))) - 2) ELSE(QTY * (HRG_DIS_SATUAN / 1000)) END
            //ELSE CASE WHEN length((QTY * HRG_DIS_SATUAN)) > 6 then substr((QTY * HRG_DIS_SATUAN), 1, length((QTY * HRG_DIS_SATUAN)) - 6) || ',' || substr((QTY * HRG_DIS_SATUAN), length((QTY * HRG_DIS_SATUAN)) - 5, 3) || ',' || substr((QTY * HRG_DIS_SATUAN), length((QTY * HRG_DIS_SATUAN)) - 2) WHEN length((QTY * HRG_DIS_SATUAN)) > 3 then substr((QTY * HRG_DIS_SATUAN), 1, length((QTY * HRG_DIS_SATUAN)) - 3) || ',' || substr((QTY * HRG_DIS_SATUAN), length((QTY * HRG_DIS_SATUAN)) - 2) ELSE(QTY * HRG_DIS_SATUAN) END END || ' [' || KETERANGAN || ']' as DETAIL
            //, TblDISCNT_ITEM.QTY as STOK_MIN, TblDISCNT_ITEM.HRG_DIS_SATUAN as STOK
            //, KETERANGAN as OWNER

            List<CartDisplay> result = new List<CartDisplay>();
            //, NM_BRG || ' ['|| CASE WHEN SATUAN = 'Gr' THEN (QTY / 1000) || ' Kg' ELSE QTY || ' ' || SATUAN END  ||']' as NM_BRG
            string strQuery = @"SELECT NOM
            , NM_BRG || ' ['|| QTY || ' ' || SATUAN ||']' as NM_BRG
            , '@' || CASE WHEN length(HRG_SATUAN) > 6 then substr(HRG_SATUAN, 1, length(HRG_SATUAN) - 6) || ',' || substr(HRG_SATUAN, length(HRG_SATUAN) - 5, 3) || ',' || substr(HRG_SATUAN, length(HRG_SATUAN) - 2) WHEN length(HRG_SATUAN) > 3 then substr(HRG_SATUAN, 1, length(HRG_SATUAN) - 3) || ',' || substr(HRG_SATUAN, length(HRG_SATUAN) - 2) ELSE HRG_SATUAN END  
            || ' = Rp ' || CASE WHEN length(HRG_TOTAL) > 6 then substr(HRG_TOTAL, 1, length(HRG_TOTAL) - 6) || ',' || substr(HRG_TOTAL, length(HRG_TOTAL) - 5, 3) || ',' || substr(HRG_TOTAL, length(HRG_TOTAL) - 2) WHEN length(HRG_TOTAL) > 3 then substr(HRG_TOTAL, 1, length(HRG_TOTAL) - 3) || ',' || substr(HRG_TOTAL, length(HRG_TOTAL) - 2) ELSE HRG_TOTAL END 
            || CASE WHEN DISCOUNT = 0 THEN '' ELSE ' [' || DISC_KET || ']' END as DETAIL
            , ID_BRG, QTY, SATUAN, HRG_TOTAL, DISCOUNT
            FROM [TblCart0] ORDER By NOM desc";

            result = DBCon.Query<CartDisplay>(strQuery);

            return result;
        }

        public Task<int> DeleteRecCartAsync(string NOM)
        {
            return DBAsyncCon.ExecuteAsync("Delete from TblCart0 where NOM = " + NOM);
        }

        public Task<int> UpdateStok(string ID_BRG, int Jum, string Mode)
        {
            return DBAsyncCon.ExecuteAsync("Update TblInventory set STOK = STOK " + Mode + " " + Jum.ToString() + " where ID_BRG = '" + ID_BRG + "'");
        }

        public int PosGetBilNo()
        {
            int result = 0;
            string strQuery = "Select COUNT(*) from TblBill";
            //strQuery = "Select case when max(BIL_NO) = 0 then 1 else max(BIL_NO) + 1 END as MaxBilNo from TblBill";

            result = DBCon.ExecuteScalar<int>(strQuery);
            if (result != 0)
            {
                strQuery = "Select case when max(BIL_NO) = 0 then 1 else max(BIL_NO) + 1 END as MaxBilNo from TblBill";
                result = DBCon.ExecuteScalar<int>(strQuery);
            }
            else
            {
                result = 1;
            }

            return result;
        }

        public int PosInsertBill(string bilNo, string nmPel, string waktu, string spcDisc, string uBayar, string uKembali, string jenisBayar, string uLain, string userId)
        {
            int result = 0;
            //string strQuery = "insert into TBLBILL (BIL_NO, NM_PEL, BIL_TGL, SPCL_DISC, UANG_TERIMA, UANG_KEMBALI, JENIS_BAYAR, BAYAR_LAIN, OPRBY) ";
            //strQuery += "values (" + bilNo + ",'" + nmPel + "','" + waktu + "'," + spcDisc + "," + uBayar + "," + uKembali + ",'" + jenisBayar + "'," + uLain + ",'" + userId + "')";
            //result = DBCon.Execute(strQuery);

            try
            {
                DBCon.BeginTransaction();
                string strQuery = "insert into TBLBILL (BIL_NO, NM_PEL, BIL_TGL, SPCL_DISC, UANG_TERIMA, UANG_KEMBALI, JENIS_BAYAR, BAYAR_LAIN, OPRBY) ";
                strQuery += "values (" + bilNo + ",'" + nmPel + "','" + waktu + "'," + spcDisc + "," + uBayar + "," + uKembali + ",'" + jenisBayar + "'," + uLain + ",'" + userId + "')";
                result = DBCon.Execute(strQuery);

                //strQuery = "insert into TblSOLD_ITEM (BIL_NO, ID_BRG, NM_BRG, UNIT, QTY, HRG_SATUAN, HRG_TOTAL, DISCOUNT, DISC_KET, HRG_MODAL, PROFIT, PEMBULATAN) SELECT 10, ID_BRG, NM_BRG, SATUAN, QTY, HRG_SATUAN, HRG_TOTAL, DISCOUNT, DISC_KET, HRG_MODAL, PROFIT, PEMBULATAN FROM TblCart0 ";
                strQuery = "insert into TblSOLD_ITEM (BIL_NO, ID_BRG, NM_BRG, UNIT, QTY, HRG_SATUAN, HRG_TOTAL, DISCOUNT, DISC_KET, HRG_MODAL, PROFIT, PEMBULATAN) SELECT " + bilNo + ", ID_BRG, NM_BRG, SATUAN, QTY, HRG_SATUAN, HRG_TOTAL, DISCOUNT, DISC_KET, HRG_MODAL, PROFIT, PEMBULATAN FROM TblCart0 ";
                result = DBCon.Execute(strQuery);

                strQuery = "delete from TBLPOSBILL";
                result = DBCon.Execute(strQuery);

                strQuery = "insert into TBLPOSBILL (BIL_NO, NM_PEL, BIL_TGL, SPCL_DISC, UANG_TERIMA, UANG_KEMBALI, JENIS_BAYAR, BAYAR_LAIN, OPRBY) ";
                strQuery += "values (" + bilNo + ",'" + nmPel + "','" + waktu + "'," + spcDisc + "," + uBayar + "," + uKembali + ",'" + jenisBayar + "'," + uLain + ",'" + userId + "')";
                result = DBCon.Execute(strQuery);

                strQuery = "delete from TBLPOSBILLDETAIL";
                result = DBCon.Execute(strQuery);

                strQuery = "insert into TBLPOSBILLDETAIL (BIL_NO, ID_BRG, NM_BRG, UNIT, QTY, HRG_SATUAN, HRG_TOTAL, DISCOUNT, DISC_KET, HRG_MODAL, PROFIT, PEMBULATAN) SELECT " + bilNo + ", ID_BRG, NM_BRG, SATUAN, QTY, HRG_SATUAN, HRG_TOTAL, DISCOUNT, DISC_KET, HRG_MODAL, PROFIT, PEMBULATAN FROM TblCart0 ";
                result = DBCon.Execute(strQuery);

                strQuery = "delete from TblCart0";
                result = DBCon.Execute(strQuery);

                DBCon.Commit();
            }
            catch (System.Exception ex)
            {
                DBCon.Rollback();
                result = -1;
            }



            return result;
        }

        public List<CartDisplay> PosGetBilList()
        {

            List<CartDisplay> result = new List<CartDisplay>();

            string strQuery = @"SELECT ID as NOM
            , NM_BRG || ' ['|| QTY || ' ' || UNIT ||']' as NM_BRG
            , '@' || CASE WHEN length(HRG_SATUAN) > 6 then substr(HRG_SATUAN, 1, length(HRG_SATUAN) - 6) || ',' || substr(HRG_SATUAN, length(HRG_SATUAN) - 5, 3) || ',' || substr(HRG_SATUAN, length(HRG_SATUAN) - 2) WHEN length(HRG_SATUAN) > 3 then substr(HRG_SATUAN, 1, length(HRG_SATUAN) - 3) || ',' || substr(HRG_SATUAN, length(HRG_SATUAN) - 2) ELSE HRG_SATUAN END  
            || ' = Rp ' || CASE WHEN length(HRG_TOTAL) > 6 then substr(HRG_TOTAL, 1, length(HRG_TOTAL) - 6) || ',' || substr(HRG_TOTAL, length(HRG_TOTAL) - 5, 3) || ',' || substr(HRG_TOTAL, length(HRG_TOTAL) - 2) WHEN length(HRG_TOTAL) > 3 then substr(HRG_TOTAL, 1, length(HRG_TOTAL) - 3) || ',' || substr(HRG_TOTAL, length(HRG_TOTAL) - 2) ELSE HRG_TOTAL END 
            || CASE WHEN DISCOUNT = 0 THEN '' ELSE ' [' || DISC_KET || ']' END as DETAIL
            , ID_BRG, QTY, UNIT as SATUAN, HRG_TOTAL, DISCOUNT
            FROM [TblPosBillDetail] ORDER By ID";

            result = DBCon.Query<CartDisplay>(strQuery);

            return result;
        }

        public List<TblPosBill> PosGetBil()
        {

            List<TblPosBill> result = new List<TblPosBill>();

            string strQuery = @"SELECT BIL_NO, NM_PEL, BIL_TGL, SPCL_DISC, UANG_TERIMA, UANG_KEMBALI, JENIS_BAYAR, BAYAR_LAIN, KAS_TGL, OPRBY FROM [TblPosBill] ";

            result = DBCon.Query<TblPosBill>(strQuery);

            return result;
        }

        public string PosGenBilName()
        {
            string result = "";
            List<TblPosBill> lst = new List<TblPosBill>();

            string strQuery = @"SELECT BIL_NO, NM_PEL, BIL_TGL, SPCL_DISC, UANG_TERIMA, UANG_KEMBALI, JENIS_BAYAR, BAYAR_LAIN, KAS_TGL, OPRBY FROM [TblPosBill] ";

            lst = DBCon.Query<TblPosBill>(strQuery);

            if (lst.Count > 0)
            {
                result = lst[0].BIL_TGL.ToString("yyyyMMdd") + "_" + lst[0].BIL_NO.ToString() + ".png";
            }

            return result;
        }

        public string GetDBUpdateTime()
        {
            string result = "";
            List<TblBill> lst = new List<TblBill>();

            string strQuery = @"SELECT * FROM [TblBill] where BIL_NO = (select max(BIL_NO) from [TblBill] where KAS_TGL not null ) ";

            lst = DBCon.Query<TblBill>(strQuery);

            if (lst.Count > 0)
            {
                result = lst[0].BIL_TGL.ToString("yyyy-MMM-dd");
            }

            return result;
        }


    }
}
