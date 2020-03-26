using System;
using System.ComponentModel;
using Saplin.xOPS.Extra;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Saplin.xOPS.UI.VirtualPages
{
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : Grid, IAppearing
    {
        public MainPage()
        {
            InitializeComponent();

            VmLocator.TestRun.PropertyChanged += ((s, e) =>
            {
                if (e.PropertyName == nameof(VmLocator.TestRun.ShowQuickComparison))
                {
                    if (!VmLocator.TestRun.ShowQuickComparison)
                    {
                        status.IsVisible = quickComparison.IsVisible = false;
                    }
                    else
                    {
                        status.Opacity = quickComparison.Opacity = 0;
                        status.IsVisible = quickComparison.IsVisible = true;
                        status.FadeTo(1);
                        Device.StartTimer(TimeSpan.FromSeconds(4), () =>
                        {
                            status.FadeTo(0);
                            quickComparison.FadeTo(1);
                            return false;
                        });
                    }
                }
            });
        }

        private bool startedBefore = false;

        public void OnAppearing()
        {
            if (!startedBefore)
            {
                startedBefore = true;
                VmLocator.TestRun.StartTest();
            }
        }

        void AboutLabel_Clicked(System.Object sender, System.EventArgs e)
        {
            Pages.ShowPage(Pages.About);
            VmLocator.OnlineDb.SendPageHit("about");
        }

        void Share_Clicked(System.Object sender, System.EventArgs e)
        {
            var share = DependencyService.Get<IShareViewAsImage>();

            if (share != null)
            {
                share.Share(testResults.Core, true, "xOPS CPU Benchmakrk - https://play.google.com/store/apps/details?id=xcom.saplin.xOPS");
                VmLocator.OnlineDb.SendPageHit("share");
            }
        }

        void QuickComparison_Tapped(System.Object sender, System.EventArgs e)
        {
            if (Pages.OnlineDb != null)
                Pages.ShowPage(Pages.OnlineDb);
        }
    }
}
