using System;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Saplin.xOPS.UI.VirtualPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : Grid
    {
        public About()
        {
            InitializeComponent();

            if (((Saplin.xOPS.UI.App)App.Current).Rose) rose.IsVisible = true;

            version.Text = VmLocator.Options.Version;
        }

        private void BackLabel_Clicked(object sender, EventArgs e)
        {
            Pages.ShowPage(Pages.MainPage);
        }
    }
}