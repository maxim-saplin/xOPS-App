using System;
using System.Threading;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Saplin.xOPS.UI.VirtualPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : Grid, IAppearing
    {
        public About()
        {
            InitializeComponent();

            if (((Saplin.xOPS.UI.App)App.Current).Rose) rose.IsVisible = true;

            version.Text = VmLocator.Options.Version;

            SizeChanged += (s, e) =>
            {
                this.InvalidateLayout();
                this.ForceLayout();
            };
        }

        private void BackLabel_Clicked(object sender, EventArgs e)
        {
            Pages.ShowPage(Pages.MainPage);
        }

        void IAppearing.OnAppearing()
        {
            //WPF hack. There's issue with labels not being properly fitted into page
            if (Device.RuntimePlatform == Device.WPF)
            {
                sv.Padding = new Thickness(50, 50, 50, 50);

                Device.StartTimer(TimeSpan.FromMilliseconds(10), () => { sv.Padding = new Thickness(0, 0, 0, 0); return false; });
            }
        }

        void version_Clicked(System.Object sender, System.EventArgs e)
        {
            Pages.ShowPage(Pages.DopeTest);
        }
    }
}