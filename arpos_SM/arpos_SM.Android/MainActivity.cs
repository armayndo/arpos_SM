using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace arpos_SM.Droid
{
    [Activity(Label = "arpos_SM", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTYxNTYwQDMxMzcyZTMzMmUzMFpiTnFkRDdzekZ0V3J3VVl0T2FmY0g1Mi9tYm1MdVRUaHZweExyTFhKaVk9");

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);
        //    if (requestCode == 0)
        //    {
        //        var uri = data.Data;
        //        //string path = GetLocalDownloadPath(uri.ToString());
        //        //System.Diagnostics.Debug.WriteLine("Image path == " + path);

        //        ContentResolver.TakePersistableUriPermission(uri, ActivityFlags.GrantReadUriPermission);
        //    }
        //}

        //protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    if (requestCode == (int)ResultCode.Chooser && resultCode.HasFlag(Result.Ok))
        //    {
        //        var uri = data.Data;
        //        if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
        //        {
        //            ContentResolver.TakePersistableUriPermission(uri, ActivityFlags.GrantReadUriPermission);
        //        }
        //        using (var stream = ContentResolver.OpenInputStream(uri))
        //        {
        //            //Log.Debug("TAG", uri.ToString());
        //        }
        //    }
        //}
    }
}