using System;
using TinyIoC;

namespace Saplin.xOPS.UI
{
    public static class Pages
    {
        private static TinyIoCContainer container;

        static Pages()
        {
            container = new TinyIoCContainer();

            container.Register<About>().AsSingleton();
            container.Register<MainPage>().AsSingleton();
        }

        public static About About => container.Resolve<About>();
        public static MainPage MainPage => container.Resolve<MainPage>();
    }
}
