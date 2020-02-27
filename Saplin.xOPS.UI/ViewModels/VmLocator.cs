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
        }

        public static TestRun TestRun => container.Resolve<TestRun>();

        static async void EagerCreateViewModels()
        {
            await Task.Run(() => { _ = TestRun; });
        }
    }
}
