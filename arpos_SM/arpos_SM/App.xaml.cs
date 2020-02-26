using arpos_SM.Asset;
using arpos_SM.Data;
using arpos_SM.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace arpos_SM
{
    public partial class App : Application
    {
        string dbPath => FileAccessHelper.GetLocalFilePath("arPosSQLite.db3");

        //public static ArPosRepo arPosRepo { get; private set; }
        public static ArPosRepo arPosRepo;
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        public static int WPixels { get; set; }
        public static int HPixels { get; set; }
        public static float Density { get; set; }
        public static float SclDensity { get; set; }

        public App()
        {
            InitializeComponent();

            arPosRepo = new ArPosRepo(dbPath);

            //MainPage = new MainPage();

            Resources = new ResourceDictionary();
            Resources.Add("primaryGreen", Color.FromHex("91CA47"));
            Resources.Add("primaryDarkGreen", Color.FromHex("6FA22E"));

            var nav = new NavigationPage(new MainPage());
            nav.BarBackgroundColor = (Color)App.Current.Resources["primaryGreen"];
            nav.BarTextColor = Color.White;

            MainPage = nav;
        }

        public static ArPosRepo Database
        {
            get
            {
                if (arPosRepo == null)
                {
                    arPosRepo = new ArPosRepo(DependencyService.Get<IFileHelper>().GetLocalFilePath("arPosSQLite.db3"));
                }
                return arPosRepo;
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
