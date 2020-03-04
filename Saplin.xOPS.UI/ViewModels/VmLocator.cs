using System.Threading.Tasks;
using TinyIoC;

namespace Saplin.xOPS.UI.ViewModels
{
    public static class VmLocator
    {
        private static TinyIoCContainer container;

        static VmLocator()
        {
            container = new TinyIoCContainer();

            container.Register<TestRun>().AsSingleton();
            container.Register<Options>().AsSingleton();
            container.Register<L11n>().AsSingleton();
        }

        public static TestRun TestRun => container.Resolve<TestRun>();
        public static Options Options => container.Resolve<Options>();
        public static L11n L11n => container.Resolve<L11n>();

        public static async void EagerCreateViewModels()
        {
            await Task.Run(() => 
            { 
                _ = TestRun;
                _ = Options;
                _ = L11n;
            });
        }
    }
}
