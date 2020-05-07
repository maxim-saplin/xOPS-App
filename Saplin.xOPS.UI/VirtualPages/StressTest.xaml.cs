using System.Runtime.CompilerServices;
using Saplin.xOPS.Extra;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.VirtualPages
{
    public partial class StressTest : Grid
    {
        bool firstDisplay = true;

        public StressTest()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsVisible) && IsVisible)
            {
                gflopsSeries.IsVisible = true;
                ginopsSeries.IsVisible = true;
                tempSeries.IsVisible = true;

                if (firstDisplay)
                {
                    firstDisplay = false;
                    VmLocator.StressTest.Retry.Execute(null);
                }
            }
        }

        void Back_Clicked(System.Object sender, System.EventArgs e)
        {
            Pages.ShowPage(Pages.MainPage);
        }

        void Share_Clicked(System.Object sender, System.EventArgs e)
        {
            var share = DependencyService.Get<IShareViewAsImage>();

#if HUAWEI
            var url = "https://appgallery.cloud.huawei.com/uowap/index.jsp?#/detailApp/C101914737";
#else
            var url = "https://play.google.com/store/apps/details?id=xcom.saplin.xOPS";
#endif

            if (share != null)
            {
                buttons.IsVisible = false;
                share.Share(this, true, "xOPS CPU Benchmakrk \n" + url);
                VmLocator.OnlineDb.SendPageHit("shareStress");
                buttons.IsVisible = true;
            }
        }
    }
}