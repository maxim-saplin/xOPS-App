
using System;
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

            ResetValues();

            var options = VmLocator.Options;

            Task.Run(() => {
                try
                {
                    if (options.Float64Bit) compute.RunFlops64Bit(iterations); else compute.RunFlops32Bit(iterations);
                    FloatSingleThreaded = breakTest ? (double?)null : compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(FloatSingleThreaded)));

                    if (breakTest) return;

                    if (options.Int64Bit) compute.RunInops64Bit(iterations); else compute.RunInops32Bit(iterations);
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
                        VmLocator.QuickComparison.Compare.Execute(new SingleResult()
                        {
                            Value = FloatSingleThreaded.Value,
                            Int = false,
                            MultiThreaded = false
                        });
                    });
                 }
                catch(Exception ex)
                {
                    var i = ex.Message;
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
            compute?.AbortMultiThreadedExecution();
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


        //public Tuple<double, bool, bool> CurrentSelection
        //{
        //    get
        //    {
        //        return new Tuple<double, bool, bool>();
        //    }
        //}

    }
}
