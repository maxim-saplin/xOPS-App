using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saplin.xOPS.Extra;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private bool startedBefore = false;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!startedBefore)
            {
                startedBefore = true;
                VmLocator.TestRun.StartTest();
            }
        }

        void AboutLabel_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(Pages.About);
        }

        void Share_Clicked(System.Object sender, System.EventArgs e)
        {
            var share = DependencyService.Get<IShareViewAsImage>();

            if (share != null)
            {
                share.Share(testResults, true, "xOPS CPU Benchmakrk - https://play.google.com/store/apps/details?id=com.Saplin.xOPS");
            }
        }
    }
}
