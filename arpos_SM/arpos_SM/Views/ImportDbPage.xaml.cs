using Android.Content;
using Android.Provider;
using arpos_SM.Asset;
using arpos_SM.Models;
using FileHelpers;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace arpos_SM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportDbPage : ContentPage
    {

        public ImportDbPage()
        {
            InitializeComponent();

            txDbStat.Text = "DB Last Update : " + App.arPosRepo.GetDBUpdateTime();
        }
        FileData fileData = new FileData();

        async void OnBrowse(object sender, EventArgs e)
        {
            try
            {
                //Use nuget : Xamarin.Plugin.FilePicker
                fileData = new FileData();
                fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null) return; // user canceled file picking

                //byte[] data = fileData.DataArray;
                fileName.Text = fileData.FileName;
                //filePath.Text = fileData.FilePath;
                DependencyService.Get<IFileHelper>().GetLocalDownloadPath(fileData.FilePath);
                filePath.Text = DependencyService.Get<IFileHelper>().GetLocalFilePath("DataImport.csv");

                //content://com.android.providers.downloads.documents/document/21
                //string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.txt");
                //txDbStat.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.txt");
                txDbStat.Text = DependencyService.Get<IFileHelper>().GetLocalDownloadPath(fileData.FilePath);
                txDbStat.Text = DependencyService.Get<IFileHelper>().GetLocalFilePath(fileData.FileName);
                
            }
            catch (Exception ex)
            {
                throw ex;
                //ExceptionHandler.ShowException(ex.Message);
            }
        }



        //private string GetRealPathFromURI(Uri contentURI)
        //{
        //    bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

        //    Uri uri = data.Data;
        //    string path = "";

        //    if (!isKitKat)
        //    {
        //        //the usual function
        //        path = GetPathToImage(uri);

        //    }
        //    else
        //    {
        //        bool isdoc = DocumentsContract.IsDocumentUri(this, uri);
        //        if (isdoc)
        //        {

        //            if (IsExternalStorageDocument(uri))
        //            {

        //                //Actually Here i don t know how to handle all possibility.......
        //                //string docId = DocumentsContract.GetDocumentId(uri);
        //                //string[] split = docId.split(':');
        //                //string type = split[0];

        //                //if ("primary".Equals(type)) {
        //                //    return Android.OS.Environment.GetExternalStoragePublicDirectory() + "/" + split[1];
        //                //}

        //            }
        //            else if (IsDownloadsDocument(uri))
        //            {

        //                string id = DocumentsContract.GetDocumentId(uri);
        //                Uri contentUri = ContentUris.WithAppendedId(Uri.Parse("content://downloads/public_downloads"), Convert.ToInt64(id));

        //                path = GetDataColumn(this, contentUri, null, null);

        //            }
        //            else if (IsMediaDocument(uri))
        //            {

        //                string docId = DocumentsContract.GetDocumentId(uri);
        //                string[] split = docId.Split(':');

        //                string type = split[0];

        //                Uri contentUri = null;
        //                if ("image".Equals(type))
        //                {
        //                    contentUri = MediaStore.Images.Media.ExternalContentUri;
        //                }
        //                else if ("video".Equals(type))
        //                {
        //                    contentUri = MediaStore.Video.Media.ExternalContentUri;
        //                }
        //                else if ("audio".Equals(type))
        //                {
        //                    contentUri = MediaStore.Audio.Media.ExternalContentUri;
        //                }

        //                string selection = "_id=?";
        //                string[] selectionArgs = new String[] {
        //        split[1]
        //    };

        //                path = GetDataColumn(this, contentUri, selection, selectionArgs);

        //            }

        //        }
        //    }

        //}

        async void OnUpdateClick(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("PERHATIAN", "Akan Meng-Update Database?", "Yes", "No");
            if (answer)
            {
                UpdateExec();
            }
        }

        async void UpdateExec()
        {
            actIndicator.IsRunning = true;
            //await Task.Delay(5000);
            await ImportDB();
            
            progBar.Progress=1;
            lblStatus.Text = "Update DB Selesai...";
            actIndicator.IsRunning = false;
        }

        async Task ImportDB()
        {
            await App.arPosRepo.DeleteAllRecAsync("TblStok_History");
            await App.arPosRepo.DeleteAllRecAsync("TblSold_Item");
            await App.arPosRepo.DeleteAllRecAsync("TblBill");
            await App.arPosRepo.DeleteAllRecAsync("TblDISCNT_ITEM");
            await App.arPosRepo.DeleteAllRecAsync("TblInventory");

            lblStatus.Text = "Importing DB...";
            await Task.Run(() =>
            {
                //lblStatus.Text = "Prepare Copy File...";
                //CopyFile(Android.App.Application.Context, fileData.FilePath);
                
                ImportDBProcess();
            });
        }

        public void CopyFile(Context context, string strUri)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse(strUri);

            const int bufferSize = 1024;

            using (Stream inputStream = context.ContentResolver.OpenInputStream(uri))
            {
                //using (FileStream outputStream = File.Create(System.IO.Path.Combine(FileSystem.AppDataDirectory, "DataImport.csv")))
                using (FileStream outputStream = File.Create(DependencyService.Get<IFileHelper>().GetLocalFilePath("DataImport.csv")))
                {
                    var buffer = new byte[bufferSize];
                    while (true)
                    {
                        var count = inputStream.Read(buffer, 0, bufferSize);
                        if (count > 0)
                        {
                            outputStream.Write(buffer, 0, count);
                        }

                        if (count < bufferSize) break;
                    }
                }
            }
        }

        public void ImportDBProcess()
        {
            string tableName = "";
            //string root = Android.OS.Environment.DirectoryDownloads;
            var enggine = new FileHelperAsyncEngine<FileHelperModel>();

            //System.IO.DirectoryNotFoundException: 'Could not find a part of the path "/content:/com.android.providers.downloads.documents/document/21"

            //using (enggine.BeginReadFile(fileData.FilePath))
            //using (enggine.BeginReadFile(DependencyService.Get<IFileHelper>().GetLocalDownloadPath(fileData.FileName)))
            //using (enggine.BeginReadFile(System.IO.Path.Combine(FileSystem.AppDataDirectory, "DataImport.csv")))
            //using (enggine.BeginReadFile(DependencyService.Get<IFileHelper>().GetLocalFilePath("DataImport.csv")))
            using (enggine.BeginReadFile(filePath.Text))
            {
                List<TblInventory> tblInventory = new List<TblInventory>();
                List<TblDISCNT_ITEM> tblDISCNT_ITEM = new List<TblDISCNT_ITEM>();
                List<TblBill> tblBill = new List<TblBill>();
                List<TblSold_Item> tblSold_Item = new List<TblSold_Item>();
                List<TblStok_History> tblStok_History = new List<TblStok_History>();

                foreach (FileHelperModel fileHlp in enggine)
                {
                    if (fileHlp.ID_BRG.StartsWith("#TABLE:"))
                    {
                        if (tableName == "")
                        {
                            tableName = fileHlp.ID_BRG.Replace("#TABLE:", "");
                        }
                        else
                        {
                            if (tableName == "INVENTORY") App.arPosRepo.InsertInvAllSync(tblInventory);
                            if (tableName == "DISCNT_ITEM") App.arPosRepo.InsertInvAllSync(tblDISCNT_ITEM);
                            if (tableName == "BILL") App.arPosRepo.InsertInvAllSync(tblBill);
                            if (tableName == "SOLD_ITEM") App.arPosRepo.InsertInvAllSync(tblSold_Item);
                            if (tableName == "STOK_HISTORY") App.arPosRepo.InsertInvAllSync(tblStok_History);
                            tableName = fileHlp.ID_BRG.Replace("#TABLE:", "");
                        }

                    }
                    else
                    {
                        if (tableName == "INVENTORY")
                        {
                            TblInventory barTblInventory = new TblInventory();
                            barTblInventory.ID_BRG = fileHlp.ID_BRG;
                            barTblInventory.NM_BRG = fileHlp.NM_BRG;
                            barTblInventory.STOK_MIN = fileHlp.STOK_MIN;
                            if (fileHlp.EXP_TGL.ToString() != "")
                            {
                                barTblInventory.EXP_TGL = DateTime.Parse(fileHlp.EXP_TGL.ToString());
                            }

                            barTblInventory.SATUAN = fileHlp.SATUAN;
                            if (fileHlp.SATUAN_JUAL.ToString() != "")
                            {
                                barTblInventory.SATUAN_JUAL = int.Parse(fileHlp.SATUAN_JUAL.ToString());
                            }
                            if (fileHlp.HRG_MODAL.ToString() != "")
                            {
                                barTblInventory.HRG_MODAL = int.Parse(fileHlp.HRG_MODAL.ToString());
                            }
                            if (fileHlp.HRG_JUAL.ToString() != "")
                            {
                                barTblInventory.HRG_JUAL = int.Parse(fileHlp.HRG_JUAL.ToString());
                            }
                            if (fileHlp.STOK.ToString() != "")
                            {
                                barTblInventory.STOK = int.Parse(fileHlp.STOK.ToString());
                            }

                            barTblInventory.OWNER = fileHlp.OWNER;
                            if (fileHlp.LAST_TRN.ToString() != "")
                            {
                                barTblInventory.LAST_TRN = DateTime.Parse(fileHlp.LAST_TRN.ToString());
                            }


                            tblInventory.Add(barTblInventory);
                        }
                        else if (tableName == "DISCNT_ITEM")
                        {
                            TblDISCNT_ITEM barTblDIS = new TblDISCNT_ITEM();
                            barTblDIS.ID_BRG = fileHlp.ID_BRG;
                            barTblDIS.KETERANGAN = fileHlp.NM_BRG;
                            barTblDIS.QTY = fileHlp.STOK_MIN;

                            if (fileHlp.HRG_MODAL.ToString() != "")
                            {
                                barTblDIS.HRG_DIS_SATUAN = int.Parse(fileHlp.HRG_MODAL.ToString());
                            }

                            tblDISCNT_ITEM.Add(barTblDIS);
                        }
                        else if (tableName == "BILL")
                        {
                            TblBill barTblBil = new TblBill();
                            barTblBil.BIL_NO = Convert.ToInt32(fileHlp.ID_BRG);
                            barTblBil.NM_PEL = fileHlp.NM_BRG;
                            barTblBil.UANG_TERIMA = fileHlp.STOK_MIN;
                            if (fileHlp.EXP_TGL.ToString() != "")
                            {
                                barTblBil.BIL_TGL = DateTime.Parse(fileHlp.EXP_TGL.ToString());
                            }

                            barTblBil.JENIS_BAYAR = fileHlp.SATUAN;
                            if (fileHlp.SATUAN_JUAL.ToString() != "")
                            {
                                barTblBil.SPCL_DISC = int.Parse(fileHlp.SATUAN_JUAL.ToString());
                            }
                            if (fileHlp.HRG_MODAL.ToString() != "")
                            {
                                barTblBil.UANG_KEMBALI = int.Parse(fileHlp.HRG_MODAL.ToString());
                            }
                            if (fileHlp.HRG_JUAL.ToString() != "")
                            {
                                barTblBil.BAYAR_LAIN = int.Parse(fileHlp.HRG_JUAL.ToString());
                            }
                            barTblBil.OPRBY = fileHlp.OWNER;
                            if (fileHlp.LAST_TRN.ToString() != "")
                            {
                                barTblBil.KAS_TGL = DateTime.Parse(fileHlp.LAST_TRN.ToString());
                            }

                            tblBill.Add(barTblBil);
                        }
                        else if (tableName == "SOLD_ITEM")
                        {
                            TblSold_Item barTblSold = new TblSold_Item();
                            barTblSold.BIL_NO = Convert.ToInt32(fileHlp.ID_BRG);
                            barTblSold.ID_BRG = fileHlp.NM_BRG;
                            barTblSold.QTY = fileHlp.STOK_MIN;

                            barTblSold.NM_BRG = fileHlp.SATUAN;
                            if (fileHlp.SATUAN_JUAL.ToString() != "")
                            {
                                barTblSold.HRG_SATUAN = int.Parse(fileHlp.SATUAN_JUAL.ToString());
                            }
                            if (fileHlp.HRG_MODAL.ToString() != "")
                            {
                                barTblSold.HRG_TOTAL = int.Parse(fileHlp.HRG_MODAL.ToString());
                            }
                            if (fileHlp.HRG_JUAL.ToString() != "")
                            {
                                barTblSold.DISCOUNT = int.Parse(fileHlp.HRG_JUAL.ToString());
                            }
                            if (fileHlp.STOK.ToString() != "")
                            {
                                barTblSold.HRG_MODAL = int.Parse(fileHlp.STOK.ToString());
                            }
                            barTblSold.UNIT = fileHlp.OWNER;
                            barTblSold.DISC_KET = fileHlp.DISC_KET;
                            if (fileHlp.PROFIT.ToString() != "")
                            {
                                barTblSold.PROFIT = int.Parse(fileHlp.PROFIT.ToString());
                            }
                            if (fileHlp.PEMBULATAN.ToString() != "")
                            {
                                barTblSold.PEMBULATAN = int.Parse(fileHlp.PEMBULATAN.ToString());
                            }

                            tblSold_Item.Add(barTblSold);
                        }
                        else if (tableName == "STOK_HISTORY")
                        {
                            TblStok_History barTblSHis = new TblStok_History();
                            barTblSHis.ID_BRG = fileHlp.ID_BRG;
                            barTblSHis.KET = fileHlp.NM_BRG;
                            barTblSHis.STOK_AWAL = fileHlp.STOK_MIN;
                            if (fileHlp.EXP_TGL.ToString() != "")
                            {
                                barTblSHis.TGL = DateTime.Parse(fileHlp.EXP_TGL.ToString());
                            }
                            if (fileHlp.SATUAN_JUAL.ToString() != "")
                            {
                                barTblSHis.STOK_ADD = int.Parse(fileHlp.SATUAN_JUAL.ToString());
                            }
                            if (fileHlp.HRG_MODAL.ToString() != "")
                            {
                                barTblSHis.STOK_AKHIR = int.Parse(fileHlp.HRG_MODAL.ToString());
                            }
                            barTblSHis.OPRBY = fileHlp.OWNER;

                            tblStok_History.Add(barTblSHis);
                        }

                    }
                }

                //Insert untuk jika habis
                if (tableName == "INVENTORY") App.arPosRepo.InsertInvAllSync(tblInventory);
                if (tableName == "DISCNT_ITEM") App.arPosRepo.InsertInvAllSync(tblDISCNT_ITEM);
                if (tableName == "BILL") App.arPosRepo.InsertInvAllSync(tblBill);
                if (tableName == "SOLD_ITEM") App.arPosRepo.InsertInvAllSync(tblSold_Item);
                if (tableName == "STOK_HISTORY") App.arPosRepo.InsertInvAllSync(tblStok_History);

            }
        }
        async void OnSaveClicked(object sender, EventArgs e)
        {


            ////buat test aja
            ////await App.arPosRepo.DeleteAllRecAsync("TblInventory");
            ////var InventoryItem = (TblInventory)BindingContext;
            //TblBill InvItem = new TblBill();
            //List<TblBill> tblBill = new List<TblBill>();
            //InvItem.BIL_NO = 14103;
            ////InvItem.NM_PEL = "Tes Nama barang 1";
            //InvItem.UANG_TERIMA = 21000;
            //InvItem.BIL_TGL = DateTime.Parse("28-Jan-2019");
            //InvItem.JENIS_BAYAR = "CASH";
            //InvItem.SPCL_DISC = 0;
            //InvItem.UANG_KEMBALI = 10000;
            //InvItem.BAYAR_LAIN = 0;
            //InvItem.OPRBY = "SULIS";
            //InvItem.KAS_TGL = DateTime.Parse("28-Jan-2019");
            //tblBill.Add(InvItem);
            //await App.arPosRepo.InsertInvAllSync(tblBill);

            ////yang dipake
            int cbil = App.arPosRepo.CheckCount("TblBill");
            int csold = App.arPosRepo.CheckCount("TblSold_Item");
            string strJum = "TblBill : " + cbil.ToString() + "\n"
                + "TblSold_Item : " + csold.ToString() + "\n"
                + "TblInventory : " + App.arPosRepo.CheckCount("TblStok_History").ToString() + "\n"
                + "TblStok_History : " + App.arPosRepo.CheckCount("TblInventory").ToString() + "\n"
                + "TblDiscnt_Item : " + App.arPosRepo.CheckCount("TblDiscnt_Item").ToString();
            var answer = await DisplayAlert("PERHATIAN", strJum, "Yes", "No");

            //buat test aja
            //TblInventory InvItem = new TblInventory();
            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK002";
            //InvItem.NM_BRG = "Tes Nama barang 2";
            //InvItem.EXP_TGL = DateTime.Parse("28-Jan-2019");
            //InvItem.HRG_JUAL = 1234567;
            //InvItem.HRG_MODAL = 59854;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Gr";
            //InvItem.SATUAN_JUAL = 100;
            //InvItem.STOK = 3500;
            //InvItem.STOK_MIN = 5000;
            ////await App.Database.SaveItemAsync(InvItem);
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK003";
            //InvItem.NM_BRG = "Tes Nama barang 3";
            //InvItem.HRG_JUAL = 12345;
            //InvItem.HRG_MODAL = 45000;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Pcs";
            ////InvItem.SATUAN_JUAL
            //InvItem.STOK = 100;
            //InvItem.STOK_MIN = 50;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK004";
            //InvItem.NM_BRG = "Tes Nama barang 4";
            //InvItem.EXP_TGL = DateTime.Parse("01-Jan-2018");
            //InvItem.HRG_JUAL = 1234;
            //InvItem.HRG_MODAL = 2000;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Pcs";
            ////InvItem.SATUAN_JUAL
            //InvItem.STOK = 10;
            //InvItem.STOK_MIN = 5;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK005";
            //InvItem.NM_BRG = "Tes Nama barang 5";
            //InvItem.EXP_TGL = DateTime.Parse("28-Jan-2019");
            //InvItem.HRG_JUAL = 100000;
            //InvItem.HRG_MODAL = 59854;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Gr";
            //InvItem.SATUAN_JUAL = 100;
            //InvItem.STOK = 3500;
            //InvItem.STOK_MIN = 5000;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK006";
            //InvItem.NM_BRG = "Tes Nama barang 6";
            //InvItem.HRG_JUAL = 55000;
            //InvItem.HRG_MODAL = 45000;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Pcs";
            ////InvItem.SATUAN_JUAL
            //InvItem.STOK = 100;
            //InvItem.STOK_MIN = 50;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK007";
            //InvItem.NM_BRG = "Tes Nama barang 7";
            //InvItem.EXP_TGL = DateTime.Parse("01-Jan-2018");
            //InvItem.HRG_JUAL = 3000;
            //InvItem.HRG_MODAL = 2000;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Pcs";
            ////InvItem.SATUAN_JUAL
            //InvItem.STOK = 10;
            //InvItem.STOK_MIN = 5;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK008";
            //InvItem.NM_BRG = "Tes Nama barang 8";
            //InvItem.EXP_TGL = DateTime.Parse("28-Jan-2019");
            //InvItem.HRG_JUAL = 100000;
            //InvItem.HRG_MODAL = 59854;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Gr";
            //InvItem.SATUAN_JUAL = 100;
            //InvItem.STOK = 3500;
            //InvItem.STOK_MIN = 5000;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK009";
            //InvItem.NM_BRG = "Tes Nama barang 9";
            //InvItem.HRG_JUAL = 55000;
            //InvItem.HRG_MODAL = 45000;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Pcs";
            ////InvItem.SATUAN_JUAL
            //InvItem.STOK = 100;
            //InvItem.STOK_MIN = 50;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK010";
            //InvItem.NM_BRG = "Tes Nama barang 10";
            //InvItem.EXP_TGL = DateTime.Parse("01-Jan-2018");
            //InvItem.HRG_JUAL = 3000;
            //InvItem.HRG_MODAL = 2000;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Pcs";
            ////InvItem.SATUAN_JUAL
            //InvItem.STOK = 10;
            //InvItem.STOK_MIN = 5;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK011";
            //InvItem.NM_BRG = "Tes Nama barang 11";
            //InvItem.EXP_TGL = DateTime.Parse("28-Jan-2019");
            //InvItem.HRG_JUAL = 100000;
            //InvItem.HRG_MODAL = 59854;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Gr";
            //InvItem.SATUAN_JUAL = 100;
            //InvItem.STOK = 3500;
            //InvItem.STOK_MIN = 5000;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //InvItem = new TblInventory();
            //InvItem.ID_BRG = "BK012";
            //InvItem.NM_BRG = "Tes Nama barang 12";
            //InvItem.HRG_JUAL = 55000;
            //InvItem.HRG_MODAL = 45000;
            //InvItem.OWNER = "OWN";
            //InvItem.SATUAN = "Pcs";
            ////InvItem.SATUAN_JUAL
            //InvItem.STOK = 100;
            //InvItem.STOK_MIN = 50;
            //await App.arPosRepo.SaveItemAsync(InvItem);

            //await Navigation.PopAsync();
        }
    }
}