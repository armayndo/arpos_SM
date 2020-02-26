using arpos_SM.Asset;
using arpos_SM.InputViews;
using arpos_SM.Models;
using Rg.Plugins.Popup.Contracts;
//using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static arpos_SM.InputViews.AlertConfirmation;

namespace arpos_SM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class POSPage : ContentPage
    {
        //SfPopupLayout popUp;

        //Untuk print page ga bisa
        //public droid.Views.ViewGroup AndroidView { get; set; }
        Int32 grandTotal = 0;

        public POSPage()
        {
            InitializeComponent();
            //popUp = new SfPopupLayout();
            ////Bind();
            ////autoComplete.BindingContext = new  ListItemVM();
            ////autoComplete.DataSource = LstInvt;

            //BindingContext = new SearchDetViewModel(this, "KS041");
            ////var vm = BindingContext as SearchDetViewModel;

            autoComplete.DataSource = App.Database.GetItemList();
            autoComplete.DisplayMemberPath = "NM_BRG";

            //List<CartDisplay> LstCart = App.Database.GetListCart();

            RefreshLV();

            ////AddBtn.Clicked += AddBtn_Clicked;
            //clickToShowPopup.Clicked += AddBtn_Clicked;
        }

        private void AddBtn_Clicked(object sender, EventArgs e)
        {
            //popUp.Show();
            
            //popUpLayout.Show();
        }



        //void OnGenerateClicked(object sender, EventArgs e)
        //{
        //    pk0.Text = "test";
        //}

        //public async Task Bind()
        //{
        //    LstInvt = await App.Database.GetItemList();
        //}

        //private IList<ListItemClass> _LstInvt;
        //public IList<ListItemClass> LstInvt
        //{
        //    get { return _LstInvt; }
        //    set { _LstInvt = value; }
        //}

        private void RefreshLV()
        {
            List<CartDisplay> LstCart = App.Database.GetListCart();

            lvCart.BeginRefresh();
            lvCart.ItemsSource = LstCart;
            lvCart.EndRefresh();

            grandTotal = 0;
            Int32 grandTotalTemp = 0;
            Int32 totalDisc = 0, totalDiscTemp = 0;
            int tempPlusMin = 0;

            foreach (var item in LstCart)
            {
                grandTotalTemp = grandTotalTemp + item.HRG_TOTAL;
                totalDiscTemp = totalDiscTemp + item.DISCOUNT;
            }

            //pembulatan
            tempPlusMin = Convert.ToInt32(Math.Round(Convert.ToDouble(grandTotalTemp) / 100, 0) * 100) - grandTotalTemp;
            grandTotalTemp = grandTotalTemp + tempPlusMin;
            grandTotal = grandTotalTemp;
            totalDiscTemp = totalDiscTemp - tempPlusMin;
            totalDisc = totalDiscTemp;

            LblTotal.Text = grandTotal.ToString("N0");
            LblTotHemat.Text = totalDisc.ToString("N0");
            LblPembulatan.Text = tempPlusMin.ToString("N0");
        }

        private async void OpenMultipleDataInputAlertDialogButton_OnClicked(object sender, EventArgs e)
        {
            //InputResultLabel.Text = "Waiting for result...";
            //string NmBrg = autoComplete.SelectedValue.ToString();

            if (autoComplete.SelectedIndex != -1)
            {
                List<TblInventory> lstInventory = App.Database.GetInvByName(autoComplete.SelectedValue.ToString());
                List<InventorySearch> LstDisc =  App.Database.GetInventoryDisc(lstInventory[0].ID_BRG);
                //string NmBrg = autoComplete.SelectedValue.ToString();
                var result = await OpenMultipleDataInputAlertDialog(lstInventory, LstDisc);

                if(result != null)
                {
                    //var resultString = $"-{result.FirstName}-\n-{result.LastName}-\n-{result.Qty}-\n";
                    //var resultString = $"-{result.Qty}-\n";
                    //InputResultLabel.Text = $"{resultString}";

                    for (int i = 0; i < result.strQty.Split(';').Length; i++)
                    {
                        var cartItem = new TblCart0();
                        cartItem.DISCOUNT = Convert.ToInt32(result.strDisc.Split(';')[i]);
                        cartItem.DISC_KET = result.strDisKet.Split(';')[i];
                        cartItem.HRG_MODAL = Convert.ToInt32(result.strModal.Split(';')[i]);
                        cartItem.HRG_SATUAN = Convert.ToInt32(result.strHrgSatuan.Split(';')[i]);
                        cartItem.HRG_TOTAL = Convert.ToInt32(result.strHrgTotal.Split(';')[i]);
                        cartItem.ID_BRG = lstInventory[0].ID_BRG;
                        cartItem.NM_BRG = lstInventory[0].NM_BRG;
                        cartItem.PROFIT = Convert.ToInt32(result.strProfit.Split(';')[i]);
                        cartItem.QTY = Convert.ToInt32(result.strQty.Split(';')[i]);
                        cartItem.SATUAN = lstInventory[0].SATUAN;

                        int hasil = await App.Database.InsertCartSync(cartItem);

                        hasil = await App.Database.UpdateStok(lstInventory[0].ID_BRG, cartItem.QTY, "-");
                        //int tess = tes;

                        autoComplete.Text = "";
                    }

                    RefreshLV();  
                }
            }
        }

        private async Task<MyDataModel> OpenMultipleDataInputAlertDialog(List<TblInventory> LstInv, List<InventorySearch> LstDisc)
        {
            
            //string NmBrg = autoComplete.Text;
            // create the TextInputView
            var inputView = new MultipleDataInputView(LstDisc,
                LstInv[0].NM_BRG, LstInv[0].HRG_JUAL.ToString(), LstInv[0].HRG_MODAL.ToString(),
                LstInv[0].SATUAN, LstInv[0].SATUAN_JUAL.ToString(), LstInv[0].SATUAN_JUAL, LstInv[0].STOK,
                "Add To Cart", "Cancel");

            // create the Transparent Popup Page
            // of type string since we need a string return
            var popup = new InputAlertDialogBase<MyDataModel>(inputView);

            // subscribe to the TextInputView's Button click event
            inputView.SaveButtonEventHandler +=
                (sender, obj) =>
                {
                    // handle validations
                    if (((MultipleDataInputView)sender).MultipleDataResult.Qty == 0)
                    {
                        ((MultipleDataInputView)sender).ValidationLabelText = "QTY tidak boleh 0";
                        ((MultipleDataInputView)sender).IsValidationLabelVisible = true;
                        return;
                    }

                    if (LstInv[0].SATUAN == "Gr")
                    {
                        if (((MultipleDataInputView)sender).MultipleDataResult.Qty % LstInv[0].SATUAN_JUAL != 0)
                        {
                            ((MultipleDataInputView)sender).ValidationLabelText = "Pembelian harus / " + LstInv[0].SATUAN_JUAL.ToString() + " Gr";
                            ((MultipleDataInputView)sender).IsValidationLabelVisible = true;
                            return;
                        }

                        if (((MultipleDataInputView)sender).MultipleDataResult.Qty < 10)
                        {
                            ((MultipleDataInputView)sender).ValidationLabelText = "Minimum pembelian 10 Gr";
                            ((MultipleDataInputView)sender).IsValidationLabelVisible = true;
                            return;
                        }
                    }
                    

                    // if all good then set the Result
                    ((MultipleDataInputView)sender).IsValidationLabelVisible = false;
                    popup.PageClosedTaskCompletionSource.SetResult(((MultipleDataInputView)sender).MultipleDataResult);
                };

            // subscribe to the TextInputView's Button click event
            inputView.CancelButtonEventHandler +=
                (sender, obj) =>
                {
                    popup.PageClosedTaskCompletionSource.SetResult(null);
                };

            // Push the page to Navigation Stack
            //await PopupNavigation.PushAsync(popup);
            await Navigation.PushPopupAsync(popup);

            // await for the user to enter the text input
            var result = await popup.PageClosedTask;

            // Pop the page from Navigation Stack
            //await PopupNavigation.PopAsync();
            await Navigation.PopAsync();

            // return user inserted text value
            return result;
        }

        async void OnListItemSelected(object sender, ItemTappedEventArgs e)
        {
            CartDisplay dataItem = e.Item as CartDisplay;
            //(e.Item as InventorySearch).ID_BRG;
            if (dataItem.NM_BRG != null)
            {
                string tes = dataItem.ID_BRG;
                var result = await OpenAlertDialog(dataItem.NOM, dataItem.NM_BRG);

                if (result != null)
                {
                    int hasil = await App.Database.DeleteRecCartAsync(dataItem.NOM.ToString());

                    hasil = await App.Database.UpdateStok(dataItem.ID_BRG, dataItem.QTY, "+");

                    RefreshLV();
                }
            }
        }

        private async Task<MyDataModel2> OpenAlertDialog(int vNom, string vNmBrg)
        {
            // create the TextInputView
            var inputView = new AlertConfirmation("PERHATIAN","Akan Menghapus : " + vNmBrg + " ?","Delete", "Cancel");

            // create the Transparent Popup Page
            // of type string since we need a string return
            var popup = new InputAlertDialogBase<MyDataModel2>(inputView);

            // subscribe to the TextInputView's Button click event
            inputView.SaveButtonEventHandler +=
                (sender, obj) =>
                {
                    // handle validations
                    if (((AlertConfirmation)sender).InputResult.strQty == "2")
                    {
                        ((AlertConfirmation)sender).ValidationLabelText = "QTY tidak boleh 0";
                        ((AlertConfirmation)sender).IsValidationLabelVisible = true;
                        return;
                    }

                    // if all good then set the Result
                    ((AlertConfirmation)sender).IsValidationLabelVisible = false;
                    //popup.PageClosedTaskCompletionSource.SetResult(((MultipleDataInputView)sender).MultipleDataResult);
                    popup.PageClosedTaskCompletionSource.SetResult(((AlertConfirmation)sender).InputResult);
                };

            // subscribe to the TextInputView's Button click event
            inputView.CancelButtonEventHandler +=
                (sender, obj) =>
                {
                    popup.PageClosedTaskCompletionSource.SetResult(null);
                };

            // Push the page to Navigation Stack
            //await PopupNavigation.PushAsync(popup);
            await Navigation.PushAsync(popup);

            // await for the user to enter the text input
            var result = await popup.PageClosedTask;

            // Pop the page from Navigation Stack
            //await PopupNavigation.PopAsync();
            await Navigation.PopAsync();

            // return user inserted text value
            return result;
        }

        async void OnListItemDelAll()
        {
            var result = await OpenAlertDialogDelAll();

            if (result != null)
            {
                List<CartDisplay> LstCart = App.Database.GetListCart();

                foreach (var item in LstCart)
                {
                    int hasil = await App.Database.DeleteRecCartAsync(item.NOM.ToString());

                    hasil = await App.Database.UpdateStok(item.ID_BRG, item.QTY, "+");
                }

                RefreshLV();
            }
        }

        private async Task<MyDataModel2> OpenAlertDialogDelAll()
        {
            // create the TextInputView
            var inputView = new AlertConfirmation("PERHATIAN", "Akan Menghapus semua barang dalam keranjang ? \nBarang akan dikembalikan ke Inventory", "Yes", "Cancel");

            // create the Transparent Popup Page
            // of type string since we need a string return
            var popup = new InputAlertDialogBase<MyDataModel2>(inputView);

            // subscribe to the TextInputView's Button click event
            inputView.SaveButtonEventHandler +=
                (sender, obj) =>
                {
                    // handle validations
                    if (((AlertConfirmation)sender).InputResult.strQty == "2")
                    {
                        ((AlertConfirmation)sender).ValidationLabelText = "QTY tidak boleh 0";
                        ((AlertConfirmation)sender).IsValidationLabelVisible = true;
                        return;
                    }

                    // if all good then set the Result
                    ((AlertConfirmation)sender).IsValidationLabelVisible = false;
                    //popup.PageClosedTaskCompletionSource.SetResult(((MultipleDataInputView)sender).MultipleDataResult);
                    popup.PageClosedTaskCompletionSource.SetResult(((AlertConfirmation)sender).InputResult);
                };

            // subscribe to the TextInputView's Button click event
            inputView.CancelButtonEventHandler +=
                (sender, obj) =>
                {
                    popup.PageClosedTaskCompletionSource.SetResult(null);
                };

            // Push the page to Navigation Stack
            //await PopupNavigation.PushAsync(popup);
            await Navigation.PushAsync(popup);

            // await for the user to enter the text input
            var result = await popup.PageClosedTask;

            // Pop the page from Navigation Stack
            //await PopupNavigation.PopAsync();
            await Navigation.PopAsync();

            // return user inserted text value
            return result;
        }

        private void eBilBtn_Clicked(object sender, EventArgs e)
        {
            //// The Forms Page that you want to create image
            //var formsView = new arpos_SM.Views.POSPrintPage();

            ////Converting forms page to native view
            //AndroidView = FormsToNativeDroid.ConvertFormsToNative(formsView.Content, new Rectangle(0, 0, 400, 800));

            //// Converting View to BitMap
            //var bitmap = ConvertViewToBitMap(AndroidView);

            //// Saving image in mobile local storage
            //SaveImage(bitmap);
            string nmFile = App.Database.PosGenBilName();
            //var sdCardPath = droid.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/arPOS";
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/arPOS";
            //var fileName = System.IO.Path.Combine(sdCardPath, "20181116.png");
            var fileName = System.IO.Path.Combine(sdCardPath, nmFile);

            //var sdCardPath = droid.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Mandiri Online";
            //var fileName = System.IO.Path.Combine(sdCardPath, "downloadMandiri1535433134411.jpg");

            IDataViewer dataViewer = DependencyService.Get<IDataViewer>();
            ////dataViewer.showPhoto(fileName, AttachmentBytes);
            dataViewer.showPhoto(fileName, dataViewer.ReadAllByteS(fileName));

            //Device.OpenUri(new Uri(fileName)); //untuk .pdf
        }

        //private Bitmap ConvertViewToBitMap(droid.Views.ViewGroup view)
        //{

        //    Bitmap bitmap = Bitmap.CreateBitmap(1000, 1600, Bitmap.Config.Argb8888);
        //    Canvas canvas = new Canvas(bitmap);
        //    canvas.DrawColor(droid.Graphics.Color.White);
        //    view.Draw(canvas);
        //    return bitmap;
        //}

        //private void SaveImage(Bitmap bitmap)
        //{
        //    var sdCardPath = droid.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        //    var fileName = System.IO.Path.Combine(sdCardPath, DateTime.Now.ToFileTime() + ".png");
        //    using (var os = new FileStream(fileName, FileMode.CreateNew))
        //    {
        //        bitmap.Compress(Bitmap.CompressFormat.Png, 95, os);
        //    }

        //    Toast.MakeText(AndroidView.Context, "Image saved Successfully..!  at " + fileName, ToastLength.Long).Show();
        //}

        private async void payBtn_OnClicked(object sender, EventArgs e)
        {

            if (grandTotal != 0)
            {
                var result = await PaymentDialog(grandTotal);

                if (result != null)
                {

                    //for (int i = 0; i < result.strQty.Split(';').Length; i++)
                    //{
                    //    var cartItem = new TblCart0();
                    //    cartItem.DISCOUNT = Convert.ToInt32(result.strDisc.Split(';')[i]);
                    //    cartItem.DISC_KET = result.strDisKet.Split(';')[i];
                    //    cartItem.HRG_MODAL = Convert.ToInt32(result.strModal.Split(';')[i]);
                    //    cartItem.HRG_SATUAN = Convert.ToInt32(result.strHrgSatuan.Split(';')[i]);
                    //    cartItem.HRG_TOTAL = Convert.ToInt32(result.strHrgTotal.Split(';')[i]);
                    //    cartItem.ID_BRG = lstInventory[0].ID_BRG;
                    //    cartItem.NM_BRG = lstInventory[0].NM_BRG;
                    //    cartItem.PROFIT = Convert.ToInt32(result.strProfit.Split(';')[i]);
                    //    cartItem.QTY = Convert.ToInt32(result.strQty.Split(';')[i]);
                    //    cartItem.SATUAN = lstInventory[0].SATUAN;

                    //    int hasil = await App.Database.InsertCartSync(cartItem);

                    //    hasil = await App.Database.UpdateStok(lstInventory[0].ID_BRG, cartItem.QTY, "-");
                    //    //int tess = tes;

                    //    autoComplete.Text = "";
                    //}

                   
                }
                RefreshLV();
            }
        }

        private async Task<PaymentModel> PaymentDialog(int totalBayar)
        {

            //string NmBrg = autoComplete.Text;
            // create the TextInputView
            var inputView = new PaymentInputView("Pembayaran",totalBayar);

            // create the Transparent Popup Page
            // of type string since we need a string return
            var popup = new InputAlertDialogBase<PaymentModel>(inputView);

            // subscribe to the TextInputView's Button click event
            inputView.CloseTrnButtonEventHandler +=
                (sender, obj) =>
                {
                    // handle validations
                    if (((PaymentInputView)sender).PaymentResult.uangBayar < totalBayar)
                    {
                        ((PaymentInputView)sender).ValidationLabelText = "Uang tidak mencukupi";
                        ((PaymentInputView)sender).IsValidationLabelVisible = true;
                        return;
                    }

                    // if all good then set the Result
                    ((PaymentInputView)sender).IsValidationLabelVisible = false;
                    popup.PageClosedTaskCompletionSource.SetResult(((PaymentInputView)sender).PaymentResult);
                };

            // subscribe to the TextInputView's Button click event
            inputView.CancelButtonEventHandler +=
                (sender, obj) =>
                {
                    popup.PageClosedTaskCompletionSource.SetResult(null);
                };

            // Push the page to Navigation Stack
            //await PopupNavigation.PushAsync(popup);
            await Navigation.PushAsync(popup);

            // await for the user to enter the text input
            var result = await popup.PageClosedTask;

            // Pop the page from Navigation Stack
            //await PopupNavigation.PopAsync();
            await Navigation.PopAsync();

            // return user inserted text value
            return result;
        }

        private void btnShareBil_Clicked(object sender, EventArgs e)
        {
            string nmFile = App.Database.PosGenBilName();
            //var sdCardPath = droid.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/arPOS";
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/arPOS";
            //var filepath = System.IO.Path.Combine(sdCardPath, "20181116.png");
            var filepath = System.IO.Path.Combine(sdCardPath, nmFile);

            //var sdCardPath = droid.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Mandiri Online";
            //var fileName = System.IO.Path.Combine(sdCardPath, "downloadMandiri1535433134411.jpg");

            IDataViewer dataViewer = DependencyService.Get<IDataViewer>();
            ////dataViewer.showPhoto(fileName, AttachmentBytes);
            dataViewer.Share(" ", "Terimakasih telah berbelanja di toko MKH, \n\n Barang hanya dapat ditukar dihari yg sama", filepath);
        }

        private void OnListItemDelAll(object sender, EventArgs e)
        {

        }
    }
}