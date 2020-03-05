using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    public partial class App : Application
    {
        public App()
        {
            //VmLocator.EagerCreateViewModels();
            //Pages.EagerCreatePages();
            
            InitializeComponent();

            MainPage = Pages.StartPage;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
