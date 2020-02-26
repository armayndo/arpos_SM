using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace arpos_SM.InputViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentInputView : ContentView
    {
        Int32 vgGrandTotal = 0;

        private void PickerLabel_OnTapped(object sender, EventArgs e)
        {
            PickerList.Focus();
        }

        private void PickerList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PickerLabel.Text = PickerList.Items[PickerList.SelectedIndex];
        }

        // public event handler to expose 
        // the Save button's click event
        public EventHandler CloseTrnButtonEventHandler { get; set; }

        // public event handler to expose 
        // the Cancel button's click event
        public EventHandler CancelButtonEventHandler { get; set; }

        // public string to expose the 
        // text Entry input's value
        public PaymentModel PaymentResult { get; set; }

        public static readonly BindableProperty ValidationLabelTextProperty =
            BindableProperty.Create(
                nameof(ValidationLabelText),
                typeof(string),
                typeof(PaymentInputView),
                string.Empty, BindingMode.OneWay, null,
                (bindable, value, newValue) =>
                {
                    ((PaymentInputView)bindable).ValidationLabel.Text = (string)newValue;
                });

        /// <summary>
        /// Gets or Sets the ValidationLabel Text
        /// </summary>
        public string ValidationLabelText
        {
            get
            {
                return (string)GetValue(ValidationLabelTextProperty);
            }
            set
            {
                SetValue(ValidationLabelTextProperty, value);
            }
        }

        public static readonly BindableProperty IsValidationLabelVisibleProperty =
            BindableProperty.Create(
                nameof(IsValidationLabelVisible),
                typeof(bool),
                typeof(PaymentInputView),
                false, BindingMode.OneWay, null,
                (bindable, value, newValue) =>
                {
                    if ((bool)newValue)
                    {
                        ((PaymentInputView)bindable).ValidationLabel
                            .IsVisible = true;
                    }
                    else
                    {
                        ((PaymentInputView)bindable).ValidationLabel
                            .IsVisible = false;
                    }
                });

        /// <summary>
        /// Gets or Sets if the ValidationLabel is visible
        /// </summary>
        public bool IsValidationLabelVisible
        {
            get
            {
                return (bool)GetValue(IsValidationLabelVisibleProperty);
            }
            set
            {
                SetValue(IsValidationLabelVisibleProperty, value);
            }
        }

        public PaymentInputView(string titleText, int grandTotal)
        {
            InitializeComponent();

            vgGrandTotal = grandTotal;

            PickerList.SelectedIndex = 0;
            // update the Element's textual values
            lblTitle.Text = titleText;
            txTotal.Text = grandTotal.ToString("N0",CultureInfo.CurrentCulture.NumberFormat);
            //lblTitle.Text = grandTotal.ToString("N0");
            //txNmPel.Text = grandTotal.ToString("N0",CultureInfo.CurrentCulture);

            // handling events to expose to public
            //SaveButton.Clicked += CloseTrnButton_Clicked;
            CancelButton.Clicked += CancelButton_Clicked;

            PaymentResult = new PaymentModel();

            PaymentResult.strNmPel = "roma";

        }

        private void CloseTrnButton_Clicked(object sender, EventArgs e)
        {
            PaymentResult.strNmPel = "roma";
            // invoke the event handler if its being subscribed
            CloseTrnButtonEventHandler?.Invoke(this, e);
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            // invoke the event handler if its being subscribed
            CancelButtonEventHandler?.Invoke(this, e);
        }

        private void txBayar_TextChanged(object sender, TextChangedEventArgs e)
        {
            //double tesdoub = double.Parse("1,999.99", CultureInfo.InvariantCulture);
            //PaymentResult.uangKembali = Convert.ToInt32(double.Parse("05", CultureInfo.CurrentCulture.NumberFormat));
            //PaymentResult.uangKembali = Convert.ToInt32(double.Parse("5.5", CultureInfo.CurrentCulture));
            //PaymentResult.uangKembali = Convert.ToInt32(double.Parse("5.5", CultureInfo.CurrentCulture.NumberFormat));
            //PaymentResult.uangKembali = Convert.ToInt32(double.Parse("5.5", CultureInfo.InvariantCulture));
            //PaymentResult.uangKembali = Convert.ToInt32(double.Parse("5,5", CultureInfo.CurrentCulture));
            //PaymentResult.uangKembali = Convert.ToInt32(double.Parse("5,5", CultureInfo.CurrentCulture.NumberFormat));

            if(txBayar.Text.Length > 0)
            {
                if (Convert.ToInt32(txBayar.Text) >= vgGrandTotal)
                {
                    txKembali.Text = (Convert.ToInt32(txBayar.Text) - vgGrandTotal).ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
                }
                else
                {
                    txKembali.Text = "0";
                }

                PaymentResult.uangBayar = Convert.ToInt32(double.Parse(txBayar.Text, CultureInfo.CurrentCulture.NumberFormat));
                //PaymentResult.uangKembali = Int32.Parse(txKembali.Text,CultureInfo.InvariantCulture.NumberFormat);
                //PaymentResult.uangKembali = Int32.Parse(txKembali.Text.Replace(",","").Replace(".",""), CultureInfo.InvariantCulture.NumberFormat);
                PaymentResult.uangKembali = Convert.ToInt32(double.Parse(txKembali.Text, CultureInfo.CurrentCulture.NumberFormat));
            }
            else
            {
                txKembali.Text = "";
            }
            
        }

        private void TrnCloseButton_Clicked(object sender, EventArgs e)
        {
            if(PaymentResult.uangBayar < vgGrandTotal)
            {
                ValidationLabel.Text = "Jumlah Uang tidak cukup";
                ValidationLabel.IsVisible = true;
            }
            else
            {
                int maxBilNo = App.Database.PosGetBilNo();

                int queryRst = 0;
                queryRst = App.Database.PosInsertBill(maxBilNo.ToString(), txNmPel.Text, DateTime.Now.ToString(), "0", PaymentResult.uangBayar.ToString(), PaymentResult.uangKembali.ToString(), PickerList.SelectedItem.ToString(), "0", "MKH");

                if (queryRst != -1)
                {
                    lblTitle.Text = "Pembayaran Sukses";
                    lblTitle.TextColor = Color.Green;
                    txNmPel.IsEnabled = false;
                    //PickerList.IsEnabled = false;
                    PickerLabel.IsEnabled = false;
                    txBayar.IsEnabled = false;
                    TrnCloseButton.IsEnabled = false;
                    SaveBill.IsEnabled = true;
                    CancelButton.Text = "Close";
                }
                else
                {
                    lblTitle.Text = "Pembayaran Gagal";
                    lblTitle.TextColor = Color.Red;
                }
            }

            
           
        }
    }

    public class PaymentModel
    {
        public string strNmPel { get; set; }
        public string strPembayaran { get; set; }
        public int spcDisc { get; set; }
        public int uangBayar { get; set; }
        public int uangKembali { get; set; }
    }
}