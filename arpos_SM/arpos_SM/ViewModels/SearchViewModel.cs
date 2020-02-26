using arpos_SM.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace arpos_SM.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel(Page page, string strFIltr)
        {
            page.Appearing += async (sender, e) =>
            {

                await BindInventory(strFIltr);

            };
            //page.BindingContextChanged += async (sender, e) =>
            //{
            //    await BindInventory(strFIltr);
            //};
        }

        //private string _ID_BRG { get; set; }

        //public string ID_BRG
        //{

        //    get
        //    {
        //        return _ID_BRG;
        //    }

        //    set
        //    {
        //        _ID_BRG = value;

        //        // When your item is selected, you can open a new "PageDetail" and pass the value
        //        Application.Current.MainPage.Navigation.PushAsync(new SearchDetPage(_ID_BRG)); // If you are in a Navigation page
        //    }
        //}

        private IList<InventorySearch> _LstInvt;
        public IList<InventorySearch> LstInvt
        {
            get { return _LstInvt; }
            private set { SetProperty(ref _LstInvt, value); }
        }

        //private IList<TblInventory> _LstInvt;
        //public IList<TblInventory> LstInvt
        //{
        //    get { return _LstInvt; }
        //    private set { SetProperty(ref _LstInvt, value); }
        //}

        //public string DetailProp
        //{
        //    get
        //    {
        //        return string.Format("{0} - {1} / {2}", Address, City, State);
        //    }
        //}

        //private async Task BindInventory(string strFiltr)
        public async Task BindInventory(string strFiltr)
        {
            if (string.IsNullOrEmpty(strFiltr))
            {
                LstInvt = await App.Database.GetInventorySearchAsync();
            }
            else
            {
                LstInvt = await App.Database.GetInventorySearchAsync(strFiltr);
            }

            //LstInvt = await App.Database.GetInventorySearchAsync();



        }
    }
}
