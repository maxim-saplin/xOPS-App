using System.Threading.Tasks;
using Saplin.xOPS.UI.VirtualPages;
using TinyIoC;
using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    public static class Pages
    {
        private static TinyIoCContainer container;

        static Pages()
        {
            container = new TinyIoCContainer();

            container.Register<HostPage>().AsSingleton();
            container.Register<StartPage>().AsSingleton();
            container.Register<OnlineDb>().AsSingleton();
            container.Register<MainPage>().AsSingleton();
            container.Register<About>().AsSingleton();
        }

        public static HostPage _HostPage => container.Resolve<HostPage>();
        public static StartPage StartPage => container.Resolve<StartPage>();
        public static OnlineDb OnlineDb => container.Resolve<OnlineDb>();
        public static MainPage MainPage => container.Resolve<MainPage>();
        public static About About => container.Resolve<About>();

        private static Task EagerCreatePages()
        {
            return Task.Run(() =>
            {
                _ = OnlineDb;
                _ = MainPage;
                _ = About;
                _HostPage.HomePage = MainPage;

                Device.BeginInvokeOnMainThread(() =>
                {
                    _HostPage.AddVirtualPage(OnlineDb);
                    _HostPage.AddVirtualPage(MainPage);
                    _HostPage.AddVirtualPage(About);
                });
                //_HostPage.AddVirtualPage(About);
            });
        }

        private static bool init = false;

        public static void Init()
        {
            if (!init)
            {
                _HostPage.AddVirtualPage(StartPage);
                EagerCreatePages();
                init = true;
            }
        }

        public static void ShowPage(Layout page)
        {
            _HostPage.ShowVirtualPage(page);
        }

    }
}
