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

            container.Register<L11n>().AsSingleton();
            container.Register<Options>().AsSingleton();
            container.Register<OnlineDb>().AsSingleton();
            container.Register<QuickComparison>().AsSingleton();
            container.Register<TestRun>().AsSingleton();
            container.Register<StressTest>().AsSingleton();
        }

        public static L11n L11n => container.Resolve<L11n>();
        public static TestRun TestRun => container.Resolve<TestRun>();
        public static Options Options => container.Resolve<Options>();
        public static QuickComparison QuickComparison => container.Resolve<QuickComparison>();
        public static OnlineDb OnlineDb => container.Resolve<OnlineDb>();
        public static StressTest StressTest => container.Resolve<StressTest>();

        public static Task EagerCreateViewModels()
        {
            return Task.Run(() => 
            {
                _ = L11n;
                _ = Options;
                _ = OnlineDb;
                _ = QuickComparison;
                _ = TestRun;
                _ = StressTest;
            });
        }
    }
}
