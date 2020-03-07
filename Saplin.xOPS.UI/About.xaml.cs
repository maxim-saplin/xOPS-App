using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Saplin.xOPS.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : ContentPage
    {
        public About()
        {
            InitializeComponent();

            if (((Saplin.xOPS.UI.App)App.Current).Rose) rose.IsVisible = true;
        }

        private void BackLabel_Clicked(object sender, EventArgs e)
        {
            Pages.About.Navigation.PopModalAsync();
        }
    }
}