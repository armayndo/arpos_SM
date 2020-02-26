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
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            lblMain.Text = "H : " + App.ScreenHeight.ToString()
                + "\nW : " + App.ScreenWidth.ToString()
                + "\nHP : " + App.HPixels.ToString()
                + "\nWP : " + App.WPixels.ToString()
                + "\nDensity : " + App.Density.ToString()
                + "\nSclDensity : " + App.SclDensity.ToString();
        }

        //async void OnCancelClicked(object sender, EventArgs e)
        //{
        //    await Navigation.PopAsync();
        //}

        async void OnPassKeyClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GenPkPage());
        }

        async void OnUpdateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ImportDbPage());
        }

        async void OnSearchClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchPage());
        }

        async void OnPOSClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new POSPage());
        }
    }
}