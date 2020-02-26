using arpos_SM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace arpos_SM.InputViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MultipleDataInputView : ContentView
	{
        Int32 vgHrgJual, vgHrgModal;
        string vgSat;
        List<InventorySearch> vgLstDisc;
        
        // public event handler to expose 
        // the Save button's click event
        public EventHandler SaveButtonEventHandler { get; set; }

        // public event handler to expose 
        // the Cancel button's click event
        public EventHandler CancelButtonEventHandler { get; set; }

        // public string to expose the 
        // text Entry input's value
        public MyDataModel MultipleDataResult { get; set; }

        public static readonly BindableProperty ValidationLabelTextProperty =
            BindableProperty.Create(
                nameof(ValidationLabelText),
                typeof(string),
                typeof(MultipleDataInputView),
                string.Empty, BindingMode.OneWay, null,
                (bindable, value, newValue) =>
                {
                    ((MultipleDataInputView)bindable).ValidationLabel.Text = (string)newValue;
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

        //protected override void OnBindingContextChanged()
        //{
        //    base.OnBindingContextChanged();

        //    //BindingContext should not be null at this point
        //    // and you may add your code here.
        //    var tes = BindingContext;
        //}

        public static readonly BindableProperty IsValidationLabelVisibleProperty =
            BindableProperty.Create(
                nameof(IsValidationLabelVisible),
                typeof(bool),
                typeof(MultipleDataInputView),
                false, BindingMode.OneWay, null,
                (bindable, value, newValue) =>
                {
                    if ((bool)newValue)
                    {
                        ((MultipleDataInputView)bindable).ValidationLabel
                            .IsVisible = true;
                    }
                    else
                    {
                        ((MultipleDataInputView)bindable).ValidationLabel
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


        public MultipleDataInputView
            (List<InventorySearch> LstDisc, string title1Text, string hargaJual, string hargaModal,
            string satuan, string satJual,
            double numStepVal, double numMaxValue,
            string saveButtonText, string cancelButtonText)
        {
			InitializeComponent ();

            //BindingContext = (SearchDetViewModel)Parent.BindingContext; //error

           

            vgHrgJual = Int32.Parse(hargaJual);
            vgHrgModal = Int32.Parse(hargaModal);
            vgSat = satuan;
            vgLstDisc = LstDisc;

            lvDiscDet.BeginRefresh();
            //BindingContext = this;
            lvDiscDet.ItemsSource = LstDisc;
            lvDiscDet.EndRefresh();


            // update the Element's textual values
            lblTitle1.Text = title1Text;
            if(satuan=="Pcs")
            {
                lblHarga.Text = "Rp. " + vgHrgJual.ToString("N0") + "/Pcs";
                lblSatJual.Text = "Satuan Jual /1 " + satuan + "\nStok:" + numMaxValue.ToString("N0") + " " + satuan + "";
            }
            else if(satuan=="Gr")
            {
                lblHarga.Text = "Rp. " + vgHrgJual.ToString("N0") + "/Kg";
                lblSatJual.Text = "Satuan Jual /" + satJual + " " + satuan + "\nStok:" + numMaxValue.ToString("N0") + " " + satuan + "";
            }
            
          
            //AgeSlider.Minimum = 0;
            //AgeSlider.Maximum = numMaxValue;
            SaveButton.Text = saveButtonText;
            CancelButton.Text = cancelButtonText;

            numericUpDown.Minimum = 0;
            numericUpDown.Maximum = numMaxValue;

            if (numStepVal == 0)
            {
                numericUpDown.StepValue = 1;
            }
            else
            {
                numericUpDown.StepValue = numStepVal;
            }

            // handling events to expose to public
            SaveButton.Clicked += SaveButton_Clicked;
            CancelButton.Clicked += CancelButton_Clicked;
            //TextEntry1.TextChanged += TextEntry1_TextChanged;
            //TextEntry2.TextChanged += TextEntry2_TextChanged;
            //AgeSlider.ValueChanged += InputEntryOnValueChanged;
            numericUpDown.ValueChanged += NumericUpDown_ValueChanged;

            
            

            MultipleDataResult = new MyDataModel();
        }

        

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            // invoke the event handler if its being subscribed
            SaveButtonEventHandler?.Invoke(this, e);
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            // invoke the event handler if its being subscribed
            CancelButtonEventHandler?.Invoke(this, e);
        }

        private void TextEntry1_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            //MultipleDataResult.FirstName = TextEntry1.Text;
        }

        private void TextEntry2_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MultipleDataResult.LastName = TextEntry2.Text;
        }

        //private void InputEntryOnValueChanged(object sender, ValueChangedEventArgs valueChangedEventArgs)
        //{
        //    InputSliderValueLabel.Text = $"[ { Math.Round(AgeSlider.Value).ToString()} ]";
        //    MultipleDataResult.Age = (int)Math.Round(AgeSlider.Value);
        //}
        private void NumericUpDown_ValueChanged(object sender, Syncfusion.SfNumericUpDown.XForms.ValueEventArgs e)
        {
            bool fsisa = true;
            List<clsHrgDisc> LstHgrDisc = new List<clsHrgDisc>();

            int QTYSisa = int.Parse(numericUpDown.Value.ToString());
            int QTYTmp;

            while((QTYSisa > 0) && fsisa)
            {
                var disQTY = (from s in vgLstDisc
                              where int.Parse(s.STOK_MIN) <= QTYSisa
                              orderby int.Parse(s.STOK_MIN) descending
                              select new clsHrgDisc { QTY = int.Parse(s.STOK_MIN), HrgSat = s.STOK, DiscKet = s.OWNER}).FirstOrDefault();
                //int tes = disQTY.QTY;
                if(disQTY == null)
                //if (int.Parse(disQTY.QTY.ToString()) == 0)
                {
                    fsisa = false;
                    LstHgrDisc.Add(new clsHrgDisc { QTY = QTYSisa, HrgSat = vgHrgJual, DiscKet="" });
                }
                else
                {
                    QTYTmp = QTYSisa;
                    QTYSisa = QTYSisa % disQTY.QTY;
                    LstHgrDisc.Add(new clsHrgDisc { QTY = QTYTmp - QTYSisa, HrgSat = disQTY.HrgSat, DiscKet=disQTY.DiscKet });
                    
                }
                
                
            }
           

            Int32 totHarga = 0;
            string tmpStrQty = "", tmpStrHrgSatuan = "", tmpStrHrgTotal = "", tmpStrDisc = "", tmpStrDisKet = "", tmpStrModal = "", tmpStrProfit = "";
            
            if (vgSat=="Gr")
            {
                //totHarga = (Int32.Parse(numericUpDown.Value.ToString()) * vgHarga)/1000;

                foreach (clsHrgDisc item in LstHgrDisc)
                {
                    totHarga = totHarga + ((item.QTY * item.HrgSat) / 1000);

                    tmpStrQty = tmpStrQty + ";" + item.QTY.ToString();
                    tmpStrHrgSatuan = tmpStrHrgSatuan + ";" + item.HrgSat.ToString();
                    tmpStrHrgTotal = tmpStrHrgTotal + ";" + ((item.QTY * item.HrgSat) / 1000).ToString();
                    tmpStrDisc = tmpStrDisc + ";" + (((item.QTY * vgHrgJual) / 1000) - ((item.QTY * item.HrgSat) / 1000)).ToString();
                    tmpStrDisKet = tmpStrDisKet + ";" + item.DiscKet;
                    tmpStrModal = tmpStrModal + ";" + ((item.QTY * vgHrgModal) / 1000).ToString();
                    tmpStrProfit = tmpStrProfit + ";" + (((item.QTY * item.HrgSat) / 1000) - ((item.QTY * vgHrgModal) / 1000)).ToString();
                }
                tmpStrQty = tmpStrQty.Substring(1);
                tmpStrHrgSatuan = tmpStrHrgSatuan.Substring(1);
                tmpStrHrgTotal = tmpStrHrgTotal.Substring(1);
                tmpStrDisc = tmpStrDisc.Substring(1);
                tmpStrDisKet = tmpStrDisKet.Substring(1);
                tmpStrModal = tmpStrModal.Substring(1);
                tmpStrProfit = tmpStrProfit.Substring(1);
            }
            else
            {
                //totHarga = Int32.Parse(numericUpDown.Value.ToString()) * vgHarga;

                foreach (clsHrgDisc item in LstHgrDisc)
                {
                    totHarga = totHarga + (item.QTY * item.HrgSat);

                    tmpStrQty = tmpStrQty + ";" + item.QTY.ToString();
                    tmpStrHrgSatuan = tmpStrHrgSatuan + ";" + item.HrgSat.ToString();
                    tmpStrHrgTotal = tmpStrHrgTotal + ";" + ((item.QTY * item.HrgSat)).ToString();
                    tmpStrDisc = tmpStrDisc + ";" + (((item.QTY * vgHrgJual)) - ((item.QTY * item.HrgSat))).ToString();
                    tmpStrDisKet = tmpStrDisKet + ";" + item.DiscKet;
                    tmpStrModal = tmpStrModal + ";" + ((item.QTY * vgHrgModal)).ToString();
                    tmpStrProfit = tmpStrProfit + ";" + (((item.QTY * item.HrgSat)) - ((item.QTY * vgHrgModal))).ToString();
                }
                tmpStrQty = tmpStrQty.Substring(1);
                tmpStrHrgSatuan = tmpStrHrgSatuan.Substring(1);
                tmpStrHrgTotal = tmpStrHrgTotal.Substring(1);
                tmpStrDisc = tmpStrDisc.Substring(1);
                tmpStrDisKet = tmpStrDisKet.Substring(1);
                tmpStrModal = tmpStrModal.Substring(1);
                tmpStrProfit = tmpStrProfit.Substring(1);
            }
            
            
            lblTotharga.Text = $"Rp. { totHarga.ToString("N0") }";

            MultipleDataResult.Qty = Int32.Parse(numericUpDown.Value.ToString());
            MultipleDataResult.strQty = tmpStrQty;
            MultipleDataResult.strHrgSatuan = tmpStrHrgSatuan;
            MultipleDataResult.strHrgTotal = tmpStrHrgTotal;
            MultipleDataResult.strDisc = tmpStrDisc;
            MultipleDataResult.strDisKet = tmpStrDisKet;
            MultipleDataResult.strModal = tmpStrModal;
            MultipleDataResult.strProfit = tmpStrProfit;
        }
    }

    public class MyDataModel
    {
        public string strQty { get; set; }
        public string strHrgSatuan { get; set; }
        public string strHrgTotal { get; set; }
        public string strDisc { get; set; }
        public string strDisKet { get; set; }
        public string strModal { get; set; }
        public string strProfit { get; set; }
        public int Qty { get; set; }
    }

    public class clsHrgDisc
    {
        public int QTY { get; set; }
        public Int32 HrgSat { get; set; }
        public Int32 HrgTot { get; set; }
        public int Disc { get; set; }
        public string DiscKet { get; set; }
        public Int32 HrgModal { get; set; }
        public Int32 Profit { get; set; }
    }
}