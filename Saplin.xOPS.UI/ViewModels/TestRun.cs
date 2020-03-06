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

            TestNotStarted = false;
            TestInterrupted = false;

            ResetValues();

            var options = VmLocator.Options;

            Task.Run(() => {
                try
                {
                    compute.RunXops(iterations, inops: false, options.Float64Bit); 
                    FloatSingleThreaded = breakTest ? (double?)null : compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(FloatSingleThreaded)));

                    if (breakTest) return;

                    compute.RunXops(iterations, inops: true, options.Int64Bit);
                    IntSingleThreaded = breakTest ? (double?)null : compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(IntSingleThreaded)));

                    if (breakTest) return;

                    compute.RunXopsMultiThreaded(iterations, options.FloatThreads, inops: false, precision64Bit: options.Float64Bit);
                    FloatMultiThreaded = breakTest ? (double?)null : compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(FloatMultiThreaded)));

                    if (breakTest) return;

                    compute.RunXopsMultiThreaded(iterations, options.IntThreads, inops: true, precision64Bit: options.Int64Bit);
                    IntMultiThreaded = breakTest ? (double?)null : compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        RaisePropertyChanged(nameof(IntMultiThreaded));
                    });
                 }
                 catch{}
                 finally
                 {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        TestNotStarted = true;
                        VmLocator.QuickComparison.Compare.Execute(new SingleResult()
                        {
                            Value = FloatSingleThreaded.Value,
                            Int = false,
                            MultiThreaded = false
                        });
                    });
                 }
            });

        }

        public Command Break => new Command(BreakTest);

        public void BreakTest()
        {
            breakTest = true;
            compute?.BreakExecution();
            TestInterrupted = true;
        }

        private bool testNotStarted = true;
        public bool TestNotStarted
        {
            get { return testNotStarted; }
            set 
            { 
                testNotStarted = value; 
                RaisePropertyChanged(); 
                RaisePropertyChanged(nameof(TestStarted)); 
                RaisePropertyChanged(nameof(ShowQuickComparison));
                RaisePropertyChanged(nameof(ShowRunning));
            } 
        }

        public bool TestStarted => !TestNotStarted;

        private bool testInterrupted = false;
        public bool TestInterrupted
        {
            get { return testInterrupted; }
            set 
            { 
                testInterrupted = value; 
                RaisePropertyChanged(); 
                RaisePropertyChanged(nameof(ShowRunning)); 
            }
        }

        public bool ShowQuickComparison => TestNotStarted && !TestInterrupted;

        public bool ShowRunning => TestStarted && !TestInterrupted;

        public double? FloatSingleThreaded { get; private set; }
        public double? FloatMultiThreaded { get; private set; }
        public double? IntSingleThreaded { get; private set; }
        public double? IntMultiThreaded { get; private set; }

    }
}
