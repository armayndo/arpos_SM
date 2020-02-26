using arpos_SM.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace arpos_SM.ViewModels
{
    public class SearchDetViewModel : BaseViewModel
    {


        //public SearchDetViewModel()
        //{

        //}

        public SearchDetViewModel(Page page, string vId, string vSat)
        {
            page.Appearing += async (sender, e) => {

                await BindInventory(vId, vSat);
            };
        }

        private IList<InventorySearch> _LstDisc;
        public IList<InventorySearch> LstDisc
        {
            get { return _LstDisc; }
            private set { SetProperty(ref _LstDisc, value); }
        }

        private IList<InventoryTren> _Data;
        public IList<InventoryTren> Data
        {
            get { return _Data; }
            private set { SetProperty(ref _Data, value); }
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

        private async Task BindInventory(string vId, string vSat)
        {


            //LstInvt = await App.Database.GetInventories("BK");
            LstDisc = await App.Database.GetInventorySearchDetAsync(vId);

            Data = await App.Database.GetInventoryTrenAsync(vId, vSat);
            //int x = 2;

        }
    }
}
