using arpos_SM.Asset;
using arpos_SM.Models;
using arpos_SM.ViewModels;
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
    public partial class SearchPage : ContentPage
    {
        private static readonly AsyncLock Locker = new AsyncLock();

        public SearchPage()
        {
            InitializeComponent();
            //actIndicator.IsRunning = true;
            //lvSearch.SetBinding(ListView.SelectedItemProperty, "ID_BRG");

            //bind();
            BindingContext = new SearchViewModel(this, "");
            //actIndicator.IsRunning = false;

        }

        //async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    //((App)App.Current).ResumeAtTodoId = (e.SelectedItem as TodoItem).ID;
        //    //Debug.WriteLine("setting ResumeAtTodoId = " + (e.SelectedItem as TodoItem).ID);
        //    if ((e.SelectedItem as InventorySearch).DETAIL.Contains("[---]") || 1==1)
        //    {
        //        await Navigation.PushAsync(new SearchDetPage((e.SelectedItem as InventorySearch).ID_BRG, (e.SelectedItem as InventorySearch).NM_BRG, (e.SelectedItem as InventorySearch).HRG_MODAL, (e.SelectedItem as InventorySearch).STOK_MIN, (e.SelectedItem as InventorySearch).OWNER));
        //        //DisplayAlert("Item Selected", (e.SelectedItem as InventorySearch).ID_BRG, "Ok");
        //    }
        //}

        async void OnListItemSelected(object sender, ItemTappedEventArgs e)
        {
            InventorySearch dataItem = e.Item as InventorySearch;
            //(e.Item as InventorySearch).ID_BRG;
            if (dataItem.ID_BRG != null)
            {
                await Navigation.PushAsync(new SearchDetPage((e.Item as InventorySearch).ID_BRG, (e.Item as InventorySearch).NM_BRG, (e.Item as InventorySearch).HRG_MODAL, (e.Item as InventorySearch).STOK_MIN, (e.Item as InventorySearch).OWNER, dataItem));
            }
        }

        private async void FilterNames()
        {
            string filter = srcBar.Text;
            //BindingContext = new SearchViewModel(this, "");
            //using (await Locker.LockAsync())
            //{
            //    BindingContext = new SearchViewModel(this, filter);

            //}
            lvSearch.BeginRefresh();
            var vm = BindingContext as SearchViewModel;
            await vm.BindInventory(filter);




            if (string.IsNullOrWhiteSpace(filter))
            {
                //lvSearch.ItemsSource = Iitem;

                lvSearch.ItemsSource = vm.LstInvt.OrderBy(i => i.NM_BRG).Take(100);

            }
            else
            {
                //BindingContext = new SearchViewModel(this);
                //lvSearch.ItemsSource = vm.LstInvt.Where(i => i.NM_BRG.ToLower().Contains("22") && i.NM_BRG.ToLower().Contains("dus"));

                //List<string> filtList = filter.Split(' ').ToList();
                if (filter.Trim().IndexOf(":") == 3 && filter.Trim().Length > 4)
                {
                    string strKey = filter.Trim().Split(':')[0].ToUpper();
                    string strVal = filter.Trim().Split(':')[1];

                    double Num;
                    //bool isNum = double.TryParse(strVal, out Num);
                    if (double.TryParse(strVal, out Num))
                    {
                        if (strKey == "STK")
                        {
                            if (Num == 0)
                            {
                                lvSearch.ItemsSource = vm.LstInvt.Where(i => i.STOK > 1).OrderBy(i => i.NM_BRG);
                            }
                            else
                            {
                                lvSearch.ItemsSource = vm.LstInvt.Where(i => i.STOK > 1 && i.LAST_TRN > System.DateTime.Now.AddDays(-30 * double.Parse(strVal))).OrderBy(i => i.NM_BRG);
                            }

                        }
                        //else if (strKey == "EXP")
                        //{
                        //    lvSearch.ItemsSource = vm.LstInvt.Where(i => i.STOK > 1 && i.LAST_TRN > System.DateTime.Now.AddDays(-30 * double.Parse(strVal))).OrderBy(i => i.NM_BRG);
                        //}
                    }
                }
                else
                {
                    lvSearch.ItemsSource = vm.LstInvt;

                    //string[] filtArr = filter.Trim().Split(' ').ToArray();
                    //if (filtArr.Count() == 1)
                    //{
                    //    lvSearch.ItemsSource = vm.LstInvt.Where(i => i.NM_BRG.ToLower().Contains(filtArr[0]));
                    //}
                    //else if (filtArr.Count() == 2)
                    //{
                    //    lvSearch.ItemsSource = vm.LstInvt.Where(i => i.NM_BRG.ToLower().Contains(filtArr[0]) && i.NM_BRG.ToLower().Contains(filtArr[1]));
                    //}
                    //else if (filtArr.Count() == 3)
                    //{
                    //    lvSearch.ItemsSource = vm.LstInvt.Where(i => i.NM_BRG.ToLower().Contains(filtArr[0]) && i.NM_BRG.ToLower().Contains(filtArr[1]) && i.NM_BRG.ToLower().Contains(filtArr[2]));
                    //}
                    //else if (filtArr.Count() == 4)
                    //{
                    //    lvSearch.ItemsSource = vm.LstInvt.Where(i => i.NM_BRG.ToLower().Contains(filtArr[0]) && i.NM_BRG.ToLower().Contains(filtArr[1]) && i.NM_BRG.ToLower().Contains(filtArr[2]) && i.NM_BRG.ToLower().Contains(filtArr[3]));
                    //}
                    //else if (filtArr.Count() == 5)
                    //{
                    //    lvSearch.ItemsSource = vm.LstInvt.Where(i => i.NM_BRG.ToLower().Contains(filtArr[0]) && i.NM_BRG.ToLower().Contains(filtArr[1]) && i.NM_BRG.ToLower().Contains(filtArr[2]) && i.NM_BRG.ToLower().Contains(filtArr[3]) && i.NM_BRG.ToLower().Contains(filtArr[4]));
                    //}
                    //else if (filtArr.Count() == 6)
                    //{
                    //    lvSearch.ItemsSource = vm.LstInvt.Where(i => i.NM_BRG.ToLower().Contains(filtArr[0]) && i.NM_BRG.ToLower().Contains(filtArr[1]) && i.NM_BRG.ToLower().Contains(filtArr[2]) && i.NM_BRG.ToLower().Contains(filtArr[3]) && i.NM_BRG.ToLower().Contains(filtArr[4]) && i.NM_BRG.ToLower().Contains(filtArr[5]));
                    //}
                    //else
                    //{
                    //    lvSearch.ItemsSource = vm.LstInvt.Where(i => i.NM_BRG.ToLower().Contains(filter.ToLower()));
                    //}
                }

            }
            lvSearch.EndRefresh();



        }
        async void bind()
        {
            using (await Locker.LockAsync())
            {
                BindingContext = new SearchViewModel(this, "");
            }
        }
        void OnSearchBarTextChanged(object sender, TextChangedEventArgs args)
        {
            //using (await Locker.LockAsync())
            //{
            //    if (srcBar.Text.Trim().Length > 4) FilterNames();
            //}
            if (srcBar.Text.Trim().Length > 4) FilterNames();

        }
        void OnSearchBarButtonPressed(object sender, EventArgs args)
        {
            //using (await Locker.LockAsync())
            //{
            //    //BindingContext = new SearchViewModel(this, srcBar.Text.Trim());
            //    //FilterNames();
            //}
            FilterNames();

        }
    }
}