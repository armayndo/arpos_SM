using arpos_SM.Asset;
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
    public partial class GenPkPage : ContentPage
    {
        GenPK genPkey = new GenPK();
        public GenPkPage()
        {
            InitializeComponent();
        }

        void OnGenerateClicked(object sender, EventArgs e)
        {
            pk0.Text = genPkey.genPaskey0();
            pk1.Text = genPkey.genPaskey1gen();
            pk2.Text = genPkey.genPaskey2gen();
            pk3.Text = genPkey.genPaskey3gen();
        }
    }
}