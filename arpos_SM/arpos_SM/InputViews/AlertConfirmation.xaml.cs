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
    public partial class AlertConfirmation : ContentView
    {

        // public event handler to expose 
        // the Save button's click event
        public EventHandler SaveButtonEventHandler { get; set; }

        // public event handler to expose 
        // the Cancel button's click event
        public EventHandler CancelButtonEventHandler { get; set; }

        // public string to expose the 
        // text Entry input's value
        public MyDataModel2 InputResult { get; set; }

        public static readonly BindableProperty ValidationLabelTextProperty =
            BindableProperty.Create(
                nameof(ValidationLabelText),
                typeof(string),
                typeof(AlertConfirmation),
                string.Empty, BindingMode.OneWay, null,
                (bindable, value, newValue) =>
                {
                    ((AlertConfirmation)bindable).ValidationLabel.Text = (string)newValue;
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
                typeof(AlertConfirmation),
                false, BindingMode.OneWay, null,
                (bindable, value, newValue) =>
                {
                    if ((bool)newValue)
                    {
                        ((AlertConfirmation)bindable).ValidationLabel
                            .IsVisible = true;
                    }
                    else
                    {
                        ((AlertConfirmation)bindable).ValidationLabel
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
        public AlertConfirmation(string titleText, string msgText, string confirmButtonText, string cancelButtonText)
        {
            InitializeComponent();

            lblTitle1.Text = titleText;
            lblMsg.Text = msgText;
            ConfirmButton.Text = confirmButtonText;
            CancelButton.Text = cancelButtonText;

            // handling events to expose to public
            ConfirmButton.Clicked += ConfirmButton_Clicked;
            CancelButton.Clicked += CancelButton_Clicked;

            InputResult = new MyDataModel2();

            InputResult.strQty = "0";
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            // invoke the event handler if its being subscribed
            CancelButtonEventHandler?.Invoke(this, e);
        }

        private void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            InputResult.strQty = "0";
            // invoke the event handler if its being subscribed
            SaveButtonEventHandler?.Invoke(this, e);
        }

        //public class MyDataModel
        //{
        //    public string Result { get; set; }
        //}

        public class MyDataModel2
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
    }
}