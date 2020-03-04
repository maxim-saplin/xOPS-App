using System;
using System.Threading.Tasks;
using TinyIoC;

namespace Saplin.xOPS.UI
{
    public static class Pages
    {
        private static TinyIoCContainer container;

        static Pages()
        {
            container = new TinyIoCContainer();

            container.Register<StartPage>().AsSingleton();
            container.Register<OnlineDb>().AsSingleton();
            container.Register<MainPage>().AsSingleton();
            container.Register<About>().AsSingleton();
        }
        
        public static StartPage StartPage => container.Resolve<StartPage>();
        public static OnlineDb OnlineDb => container.Resolve<OnlineDb>();
        public static MainPage MainPage => container.Resolve<MainPage>();
        public static About About => container.Resolve<About>();

        public static async void EagerCreatePages()
        {
            await Task.Run(() =>
            {
                _ = StartPage;
                _ = OnlineDb;
                _ = MainPage;
                _ = About;
            });
        }
    }
}
