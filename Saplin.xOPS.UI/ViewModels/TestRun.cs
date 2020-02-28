
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{
    public class TestRun : BaseViewModel
    {
        Compute compute = new Compute();

        private void ResetValues()
        {
            IntMultiThreaded = IntSingleThreaded
                = FloatMultiThreaded = FloatSingleThreaded = null;

            RaisePropertyChanged(nameof(FloatSingleThreaded));
            RaisePropertyChanged(nameof(IntSingleThreaded));
            RaisePropertyChanged(nameof(FloatMultiThreaded));
            RaisePropertyChanged(nameof(IntMultiThreaded));

            breakTest = false;
        }

        public Command Retry => new Command(StartTest);
        private volatile bool breakTest = false;

        public void StartTest()
        {
            const int iterations = 50 * 1000 * 1000;
            const int threads = 16;

            TestNotStarted = false;

            ResetValues();

            Task.Run(() => {
                try
                {
                    compute.RunFlops64Bit(iterations);
                    FloatSingleThreaded = compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(FloatSingleThreaded)));

                    if (breakTest) return;

                    compute.RunInops32Bit(iterations);
                    IntSingleThreaded = compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(IntSingleThreaded)));

                    if (breakTest) return;

                    compute.RunXopsMultiThreaded(iterations, threads, inops: false, precision64Bit: true);
                    FloatMultiThreaded = compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(FloatMultiThreaded)));

                    if (breakTest) return;

                    compute.RunXopsMultiThreaded(iterations, threads, inops: true);
                    IntMultiThreaded = compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(IntMultiThreaded)));
                 }
                 finally
                 {
                    Device.BeginInvokeOnMainThread(() => TestNotStarted = true);
                 }
            });

        }

        public Command Break => new Command(BreakTest);

        public void BreakTest()
        {
            breakTest = true;
        }

        private bool testNotStarted = true;
        public bool TestNotStarted
        {
            get { return testNotStarted; }
            set { testNotStarted = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(TestStarted)); } 
        }

        public bool TestStarted => !TestNotStarted;

        public double? FloatSingleThreaded { get; private set; }
        public double? FloatMultiThreaded { get; private set; }
        public double? IntSingleThreaded { get; private set; }
        public double? IntMultiThreaded { get; private set; }

    }
}
