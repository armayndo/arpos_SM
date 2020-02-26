using arpos_SM.Models;
using arpos_SM.ViewModels;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace arpos_SM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchDetPage : ContentPage
    {
        public SearchDetPage(string vId, string vNmBrg, string vHM, string vSMin, string vOwn, InventorySearch dataItem)
        {
            InitializeComponent();

            this.Title = vNmBrg;
            lblJual.Text = dataItem.HRG_JUAL;
            lblModal.Text = vHM;
            lblStok.Text = dataItem.STR_STOK + " / " + vSMin;
            lblOwn.Text = vOwn + " / " + dataItem.STR_EXP;
            //lblExp.Text = dataItem.STR_EXP;


            string sat = "";
            if (vHM.Split('/')[1].ToString() == "Pcs")
            {
                sat = "Pcs";
            }
            else
            {
                //sat = "Gr";
                sat = "Pcs";

                //Initializing secondary Axis
                NumericalAxis secondaryAxis = new NumericalAxis();

                secondaryAxis.Title.Text = "Jumlah (Gr)";

                Chart.SecondaryAxis = secondaryAxis;
            }
            //lvSearchDet.BeginRefresh();
            //this.BindingContext = new SearchDetViewModel(this, vId);
            //BindingContext = new SearchDetViewModel(this, vId);

            BindingContext = new SearchDetViewModel(this, vId, sat);

            //Chart.BindingContext = new SearchDetViewModel(this, vId, sat);

            //lvSearchDet.BindingContext = new SearchDetViewModel(this, vId, sat);


            ////

            ////var vm = BindingContext as SearchDetViewModel; //ga bisa
            ////lvSearchDet.ItemsSource = vm.LstDisc;
            //lvSearchDet.EndRefresh();

            //this.BindingContext = new PersonViewModel();

            //test

        }
    }
}