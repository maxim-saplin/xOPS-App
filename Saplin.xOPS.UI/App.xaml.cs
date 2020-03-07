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

            if (Rose)
            {
                ApplyTheme();
            }
        }

        private void ApplyTheme()
        {
            var theme = new Rose();

            foreach (var key in theme.Keys)
            {
                if (Xamarin.Forms.Application.Current.Resources.ContainsKey(key))
                    Xamarin.Forms.Application.Current.Resources[key] = theme[key];
            }
        }

        public bool Rose
        {
            get
            {
                return Resources["Rose"] is bool && (bool)Resources["Rose"];
            }
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
